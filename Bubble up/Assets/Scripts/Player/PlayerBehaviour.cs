using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform _sprite;

    [SerializeField]
    private CircleCollider2D _collider;

    [SerializeField]
    private float _maxLife = 10;

    [SerializeField]
    private float _jumpForce = 1;

    [SerializeField]
    private float _maxTimeForce;

    [SerializeField]
    private float _jumpTime;

    [SerializeField]
    private AnimationCurve _animationCurve;

    [SerializeField]
    private InputActionReference _jump;

    [SerializeField]
    private float _maxFallSpeed;

    [SerializeField]
    private float _maxHorizontalSpeed;

    [SerializeField]
    private float _limitDistnaceToFail = 20f;

    private Vector2 _screenBounds; 

    private float _jumpTimeEnergy;
    private float _currentLife;
    private float _currentHeight;
    private float _heightRecord;
    private float _currentFallSpeed;
    private float _currentHorizontalSpeed;

    private Coroutine _chargin;
    private Coroutine _movingDown;
    private Coroutine _movingUp;

    private void Awake()
    {
        _jump.action.performed += Jump;
        _currentLife = _maxLife;
    }
    private void Start()
    {
        GameManager.Instance.Height += (float value) =>_currentHeight = value;
        Camera mainCamera = Camera.main;
        _screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        SetSpeeds();
    }

    private void SetSpeeds()
    {
        _currentFallSpeed = Mathf.Lerp(1, _maxFallSpeed, _currentHeight / GameManager.Instance.MaxHeightDificult);
        _currentHorizontalSpeed = Mathf.Lerp(1, _maxHorizontalSpeed, _currentHeight / GameManager.Instance.MaxHeightDificult);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.GameOverState)
        {
            return;
        }

        if (context.ReadValueAsButton())
        {
            if(_movingDown != null)
            {
                StopCoroutine(_movingDown);
                _movingDown = null;
            }
            if(_movingUp != null)
            {
                StopCoroutine(_movingUp);
                _movingUp = null;
            }
            _chargin = StartCoroutine(ChargeTimeEnergy());
        }
        else
        {
            if(_chargin != null)
            {
                StopCoroutine(_chargin);
                _chargin = null;
            }
            _movingUp = StartCoroutine(MoveBubble());
            CheckLife();
        }
    }

    private IEnumerator ChargeTimeEnergy()
    {
        _jumpTimeEnergy = 0;

        while (true)
        {
            if (_jumpTimeEnergy <= _maxTimeForce)
            {
                _jumpTimeEnergy += Time.deltaTime;
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
        float totalJumpTime = Mathf.Clamp(_jumpTimeEnergy, 0, _maxTimeForce) * _jumpForce;

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

        if(_heightRecord > _currentHeight)
        {
            GameManager.Instance.ChangeHeight(_heightRecord);
            SetSpeeds();
        }

        _movingDown = StartCoroutine(MoveDownWithDynamicDirection());
    }

    private void CheckLife()
    {
        StopCoroutine(LifeScale());
        _currentLife -= _jumpTimeEnergy * _jumpForce;
        StartCoroutine(LifeScale());
        if( _currentLife < 0)
        {
            GameOver();
        }
    }

    public void ChangeLife(float value)
    {
        _currentLife += value;
        CheckLife();
    }

    private IEnumerator LifeScale()
    {
        Vector3 startScale = _sprite.transform.localScale;
        float scale = _currentLife / _maxLife;
        Vector3 targetScale = new Vector3(scale, scale, scale);

        float elapsedTime = 0f;
        while (elapsedTime < _jumpTime)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsedTime / _jumpTime);

            _sprite.transform.localScale = Vector3.Lerp(startScale, targetScale, progress);

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
            float horizontalDisplacement = currentDirection * _currentHorizontalSpeed * Time.deltaTime;
            float newX = transform.position.x + horizontalDisplacement;
            float newY = transform.position.y - (_currentFallSpeed * Time.deltaTime);

            newX = Mathf.Clamp(newX, -_screenBounds.x + transform.localScale.x / 2, _screenBounds.x - transform.localScale.x / 2);

            transform.position = new Vector3(newX, newY, transform.position.z);

            if (transform.position.y <= _currentHeight - _limitDistnaceToFail)
            {
                GameOver();
            }
            if (newX <= -_screenBounds.x + transform.localScale.x / 2 || newX >= _screenBounds.x - transform.localScale.x / 2)
            {
                GameOver();
            }

            yield return null;
        }
    }

    private void GameOver()
    {
        StopAllCoroutines();
        GameManager.Instance.GameOver.Invoke(true);
        Debug.Log("GameOver");
    }
}