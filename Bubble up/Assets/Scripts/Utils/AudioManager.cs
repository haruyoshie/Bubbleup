using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField]
    private AudioSource _underWaterSfx;

    [SerializeField]
    private AudioSource _extraFx;

    [SerializeField]
    private List<AudioClip> _waterSounds;

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
    }

    private void OnSceneUnLoad(Scene arg0)
    {
        if (_player != null)
        {
            _player.JumpState -= PlayJumpSound;
        }
    }

    private void OnSceneChanged(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "Menu")
        {
            _underWaterSfx.Stop();
        }
        else
        {
            if (!_underWaterSfx.isPlaying)
            {
                _underWaterSfx.Play();
            }
            SetPlayerAudio();
        }
    }

    private void SetPlayerAudio()
    {
        _player = FindAnyObjectByType<PlayerBehaviour>();
        _player.JumpState += PlayJumpSound;
    }

    private void PlayJumpSound(bool state)
    {
        if (state) return;

        _extraFx.PlayOneShot(GetWaterClipRandom());
    }

    private AudioClip GetWaterClipRandom()
    {
        int randomSound = UnityEngine.Random.Range(0, _waterSounds.Count);
        return _waterSounds[randomSound];

    }
}
