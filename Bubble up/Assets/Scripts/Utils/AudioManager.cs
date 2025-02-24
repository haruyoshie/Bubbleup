using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField]
    private AudioSource _underWaterSfx;

    [SerializeField]
    private AudioSource _extraFx;

    [SerializeField]
    private AudioSource _bubbleDown;

    [SerializeField]
    private List<AudioClip> _waterSounds;

    [SerializeField]
    private AudioClip _popBubble;

    [SerializeField]
    private AudioClip _clic;

    [SerializeField]
    private AudioMixer _mixer;

    [SerializeField]
    private float _targetCutoff = 500f;

    [SerializeField]
    private float _lerpDuration = 1f; 
    
    private float _initialCutoff;

    private Coroutine _lowPassCoroutine;

    private PlayerBehaviour _player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneChanged;
        SceneManager.sceneUnloaded += OnSceneUnLoad;

        _mixer.GetFloat("LowpassCutoff",out _initialCutoff);
        FindAllBtnsAndAddSound();
    }

    private void GameOver(bool state)
    {
        if (!state) return;

        _bubbleDown.Stop();
        _bubbleDown.mute = true;

        _extraFx.PlayOneShot(_popBubble);
    }

    private void OnSceneUnLoad(Scene arg0)
    {
        if (arg0.name == "Game")
        {
            GameManager.Instance.GameOver -= GameOver;
        }
        _bubbleDown.Stop();
        if (_player != null)
        {
            _player.JumpState -= PlayJumpSound;
            _player.JumpState -= AudioMixerFX;
        }
    }

    private void OnSceneChanged(Scene arg0, LoadSceneMode arg1)
    {
        _bubbleDown.Stop();
        _bubbleDown.mute = false;

        if (arg0.name == "Menu")
        {
            _underWaterSfx.Stop();
        }
        else
        {
            GameManager.Instance.GameOver += GameOver;

            if (!_underWaterSfx.isPlaying)
            {
                _underWaterSfx.Play();
            }

            SetPlayerAudio();
        }

        FindAllBtnsAndAddSound();
    }

    private void FindAllBtnsAndAddSound()
    {
        Button[] buttonsOnScene = FindObjectsOfType<Button>(true);

        foreach (Button button in buttonsOnScene)
        {
            button.onClick.AddListener(PlayClic);
        }
    }

    public void PlayClic()
    {
        _extraFx.PlayOneShot(_clic);
    }

    private void SetPlayerAudio()
    {
        _player = FindObjectOfType<PlayerBehaviour>(true);
        _player.JumpState += PlayJumpSound;
        _player.JumpState += AudioMixerFX;
    }

    private void AudioMixerFX(bool state)
    {
        if (_lowPassCoroutine != null)
        {
            StopCoroutine(_lowPassCoroutine);
        }

        _lowPassCoroutine = StartCoroutine(LerpLowPass(state));
    }
    private IEnumerator LerpLowPass(bool state)
    {
        float startValue;
        float endValue;

        if (state)
        {
            startValue = _initialCutoff;
            endValue = _targetCutoff;
        }
        else
        {
            startValue = _targetCutoff;
            endValue = _initialCutoff;
        }

        float elapsedTime = 0f;

        while (elapsedTime < _lerpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _lerpDuration;
            float currentCutoff = Mathf.Lerp(startValue, endValue, t);

            _mixer.SetFloat("LowpassCutoff", currentCutoff);

            yield return null;
        }

        _mixer.SetFloat("LowpassCutoff", endValue);
    }
    private void PlayJumpSound(bool state)
    {
        if (state) return;
        AudioClip audioClip = GetWaterClipRandom(); 
        _extraFx.PlayOneShot(audioClip);
        if (!_bubbleDown.isPlaying)
        {
            WaitToPlayBubbleFx(audioClip);
        }
        else
        {
            _bubbleDown.Stop();
            WaitToPlayBubbleFx(audioClip);
        }

    }

    private async void WaitToPlayBubbleFx(AudioClip clip)
    {
        await Task.Delay((int)(clip.length * 1000));
        _bubbleDown.Play();
    }

    private AudioClip GetWaterClipRandom()
    {
        int randomSound = UnityEngine.Random.Range(0, _waterSounds.Count);
        return _waterSounds[randomSound];

    }
}
