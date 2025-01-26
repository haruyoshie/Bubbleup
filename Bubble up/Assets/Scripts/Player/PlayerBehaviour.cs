using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerBehaviour : MonoBehaviour
{
    public event Action<bool> JumpState;

    [SerializeField]
    private PlayerStats _stats;

    [SerializeField]
    private float _maxTimeForce;

    [SerializeField]
    private float _jumpTime;

    [SerializeField]
    private AnimationCurve _animationCurve;

    [SerializeField]
    private InputActionReference _jump;

    [SerializeField]
    private Animator _animator;

    private Vector2 _screenBounds;

    private float _currentHeight;
    private float _heightRecord;

    private Coroutine _charging;
    private Coroutine _movingDown;
    private Coroutine _movingUp;

    private void Awake()
    {
        _jump.action.performed += Jump;
        _stats.CurrentLife = _stats.MaxLife;
    }
    private void Start()
    {
        GameManager.Instance.Height += (float value) => _currentHeight = value;
        Camera mainCamera = Camera.main;
        _screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        SetSpeeds();
    }

    private void SetSpeeds()
    {
        _stats.CurrentFallSpeed = Mathf.Lerp(_stats.JumpForce/1.5f, _stats.MaxFallSpeed, _currentHeight / GameManager.Instance.MaxHeightDificult);
        _stats.CurrentHorizontalSpeed = Mathf.Lerp(_stats.JumpForce / 2, _stats.MaxHorizontalSpeed, _currentHeight / GameManager.Instance.MaxHeightDificult);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.GameOverState)
        {
            return;
        }

        if (context.ReadValueAsButton())
        {
            _charging = StartCoroutine(ChargeTimeEnergy());
        }
        else
        {
            CheckAndStopCoroutines();
            _movingUp = StartCoroutine(MoveBubble());
            CheckLife();
        }

        JumpState?.Invoke(context.ReadValueAsButton());
    }

    private void CheckAndStopCoroutines()
    {
        if (_movingDown != null)
        {
            StopCoroutine(_movingDown);
            _movingDown = null;
        }
        if (_movingUp != null)
        {
            StopCoroutine(_movingUp);
            _movingUp = null;
        }
        if (_charging != null)
        {
            StopCoroutine(_charging);
            _charging = null;
        }
    }

    private IEnumerator ChargeTimeEnergy()
    {
        _stats.JumpTimeEnergy = 0;

        while (true)
        {
            if (_stats.JumpTimeEnergy <= _maxTimeForce)
            {
                _stats.JumpTimeEnergy += Time.deltaTime;
                yield return null;
            }
            else
            {
                yield return false;
            }
        }
    }

    private IEnumerator MoveBubble()
    {
        float elapsedTime = 0f;
        float totalJumpTime = Mathf.Clamp(_stats.JumpTimeEnergy, 0, _maxTimeForce) * _stats.JumpForce;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + new Vector3(0, totalJumpTime, 0);

        while (elapsedTime < _jumpTime)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsedTime / _jumpTime);
            float curveValue = _animationCurve.Evaluate(progress);

            transform.position = Vector3.Lerp(startPosition, targetPosition, curveValue);

            yield return null;
        }
        _heightRecord = transform.position.y;

        if (_heightRecord > _currentHeight)
        {
            GameManager.Instance.ChangeHeight(_heightRecord);
            SetSpeeds();
        }

        _movingDown = StartCoroutine(MoveDownWithDynamicDirection());
    }

    private void CheckLife()
    {
        StopCoroutine(LifeScale());
        _stats.CurrentLife -= (_stats.JumpTimeEnergy * _stats.JumpForce) / 2;
        StartCoroutine(LifeScale());
        if (_stats.CurrentLife <= 0.2f)
        {
            GameOver();
        }
    }

    public void ChangeLife(float value)
    {
        _stats.CurrentLife += value;
        CheckLife();
    }

    private IEnumerator LifeScale()
    {
        Vector3 startScale = _stats.Sprite.transform.localScale;
        float scale = _stats.CurrentLife / _stats.MaxLife;
        scale = Mathf.Clamp(scale,0.2f,1);
        Vector3 targetScale = new Vector3(scale, scale, scale);

        float elapsedTime = 0f;
        while (elapsedTime < _jumpTime)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsedTime / _jumpTime);

            _stats.Sprite.transform.localScale = Vector3.Lerp(startScale, targetScale, progress);

            yield return null;
        }
    }
    private IEnumerator MoveDownWithDynamicDirection()
    {
        float currentDirection = 0f;

        Vector3 startPosition = transform.position;

        float leftDistance = Mathf.Abs(startPosition.x - (-_screenBounds.x));
        float rightDistance = Mathf.Abs(startPosition.x - _screenBounds.x);

        float horizontalBias = 0f;
        if (leftDistance < rightDistance)
        {
            horizontalBias = -1f;
        }
        else if (rightDistance < leftDistance)
        {
            horizontalBias = 1f;
        }
        else
        {
            horizontalBias = UnityEngine.Random.Range(0f, 1f) < 0.5f ? 1f : -1f;
        }

        float heightFactor = Mathf.Clamp01(_currentHeight / GameManager.Instance.MaxHeightDificult);
        float randomDirection = UnityEngine.Random.Range(-1f, 1f);
        currentDirection = Mathf.Lerp(randomDirection, horizontalBias, heightFactor);

        while (true)
        {
            float horizontalDisplacement = currentDirection * _stats.CurrentHorizontalSpeed * Time.deltaTime;
            float newX = transform.position.x + horizontalDisplacement;
            float newY = transform.position.y - (_stats.CurrentFallSpeed * Time.deltaTime);

            newX = Mathf.Clamp(newX, -_screenBounds.x + transform.localScale.x / 2, _screenBounds.x - transform.localScale.x / 2);

            transform.position = new Vector3(newX, newY, transform.position.z);

            if (transform.position.y <= startPosition.y - _stats.LimitDistnaceToFail)
            {
                ChangeLife(-0.0000000001f);
            }
            //if (newX <= -_screenBounds.x + transform.localScale.x / 2 || newX >= _screenBounds.x - transform.localScale.x / 2)
            //{
            //    GameOver();
            //}

            yield return null;
        }
    }

    public void GameOver()
    {
        StopAllCoroutines();
        _animator.SetTrigger("Die");
        GameManager.Instance.GameOver.Invoke(true);
        GameManager.Instance.gameOverUI.SetActive(true);
        Debug.Log("GameOver");
    }
}