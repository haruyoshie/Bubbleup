using UnityEngine;
using Cinemachine;
using System.Collections;

public class CinemachineZoom : MonoBehaviour
{
    [SerializeField]
    private PlayerBehaviour _player;

    [SerializeField]
    private CinemachineVirtualCamera _virtualCamera;

    [SerializeField]
    private float _maxZoomSize = 5f;

    [SerializeField]
    private float _zoomTime = 2f;

    [SerializeField]
    private float _shakeIncreaseRate = 0.1f;

    [SerializeField]
    private float _maxShakeIntensity = 5f;

    private float _originalZoomSize;
    private bool _isZooming;
    private CinemachineBasicMultiChannelPerlin _noiseComponent;
    private float _currentShakeIntensity;

    private void Start()
    {
        _player.JumpState += GetState;
        _originalZoomSize = _virtualCamera.m_Lens.OrthographicSize;
        _noiseComponent = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (_noiseComponent != null)
        {
            _noiseComponent.m_AmplitudeGain = 0f;
        }
    }

    private void GetState(bool state)
    {
        StopAllCoroutines();
        _isZooming = false;

        if (state)
        {
            ZoomInToFollowObject();
        }
        else
        {
            ZoomOutToOriginalPosition();
        }
    }

    public void ZoomInToFollowObject()
    {
        if (_isZooming) return;
        StartCoroutine(ZoomToDistance(_maxZoomSize, _zoomTime));
        StartCoroutine(ShakeEffect());
    }

    public void ZoomOutToOriginalPosition()
    {
        if (_isZooming) return;
        StopCoroutine(ShakeEffect());
        StartCoroutine(ZoomToDistance(_originalZoomSize, _zoomTime * 0.1f));
        ResetShake();
    }

    private IEnumerator ZoomToDistance(float targetSize, float duration)
    {
        _isZooming = true;

        float elapsedTime = 0f;
        float startSize = _virtualCamera.m_Lens.OrthographicSize;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            _virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, t);
            yield return null;
        }

        _virtualCamera.m_Lens.OrthographicSize = targetSize;
        _isZooming = false;
    }

    private IEnumerator ShakeEffect()
    {
        _currentShakeIntensity = 0f;

        while (_isZooming)
        {
            if (_noiseComponent != null)
            {
                _currentShakeIntensity = Mathf.Min(_currentShakeIntensity + _shakeIncreaseRate * Time.deltaTime, _maxShakeIntensity);
                _noiseComponent.m_AmplitudeGain = _currentShakeIntensity;
            }

            yield return null;
        }
    }

    private void ResetShake()
    {
        if (_noiseComponent != null)
        {
            _noiseComponent.m_AmplitudeGain = 0f;
        }
    }
}