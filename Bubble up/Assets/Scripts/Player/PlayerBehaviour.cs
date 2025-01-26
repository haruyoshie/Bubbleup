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
    private AnimationCurve _zigzagCurve;

    [SerializeField]
    private InputActionReference _jump;

    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private Transform _arrow;

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
        _stats.CurrentFallSpeed = Mathf.Lerp(_stats.JumpForce / 10, _stats.MaxFallSpeed, _currentHeight / GameManager.Instance.MaxHeightDificult);
        _stats.CurrentHorizontalSpeed = Mathf.Lerp(_stats.JumpForce / 10, _stats.MaxHorizontalSpeed, _currentHeight / GameManager.Instance.MaxHeightDificult);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.GameOverState)
        {
            return;
        }

        CheckHeigthRecord();

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

        CheckHeigthRecord();

        _movingDown = StartCoroutine(MoveDownWithSmoothZigzag());
    }

    private void CheckHeigthRecord()
    {
        _heightRecord = transform.position.y;

        if (_heightRecord > _currentHeight)
        {
            GameManager.Instance.ChangeHeight(_heightRecord);
            SetSpeeds();
        }
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
    private IEnumerator MoveDownWithSmoothZigzag()
    {
        float elapsedTime = 0f;
        float directionSwitchTime = 0f;
        int horizontalDirection = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
        float startYPosition = transform.position.y;

        while (true)
        {
            float horizontalDisplacement = horizontalDirection * _stats.CurrentHorizontalSpeed * Time.deltaTime;
            float verticalDisplacement = -_stats.CurrentFallSpeed * Time.deltaTime;

            float curveValue = _zigzagCurve.Evaluate(elapsedTime / _jumpTime);
            horizontalDisplacement *= curveValue;

            Vector3 newPosition = transform.position + new Vector3(horizontalDisplacement, verticalDisplacement, 0);

            newPosition.x = Mathf.Clamp(newPosition.x, -_screenBounds.x, _screenBounds.x);

            transform.position = newPosition;

            directionSwitchTime += Time.deltaTime;
            if (directionSwitchTime >= _stats.ZigZagSwitchInterval)
            {
                directionSwitchTime = 0f;
                elapsedTime = 0f;
                horizontalDirection *= -1;
            }
            if(transform.position.y < startYPosition - _stats.LimitDistnaceToFail)
            {
                ChangeLife(-0.00000000001f);
            }

            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }



    public void GameOver()
    {
        StopAllCoroutines();
        _animator.SetTrigger("Die");
        GameManager.Instance.GameOver.Invoke(true);
        Debug.Log("GameOver");
    }

    private void OnDestroy()
    {
        CheckAndStopCoroutines();
        _jump.action.performed -= Jump;
    }
}