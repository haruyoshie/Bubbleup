using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public event Action<float> Height;
    public Action<bool> GameOver;

    [SerializeField]
    private float _maxHeightDificult;

    [SerializeField]
    private bool _gameOverState;

    [SerializeField]
    private GameObject _score;

    [SerializeField]
    private GameObject _recordScore;

    [SerializeField]
    private GameObject _intro;

    [SerializeField]
    private GameObject _finalScreen;

    public float MaxHeightDificult 
    { 
        get => _maxHeightDificult;
    }
    public bool GameOverState 
    { 
        get => _gameOverState; 
    }

    private void Awake()
    {
        Instance = this;
        GameOver += (bool value) => _gameOverState = value;

        OnInfiniteMode();
        OnStoryMode();
    }

    private void OnInfiniteMode()
    {
        if (GameMode.CurrentGameType == GameMode.GameType.Infinite)
        {
            _maxHeightDificult = PlayerPrefs.GetFloat("MaxScore") > _maxHeightDificult ? _maxHeightDificult * 1.5f : _maxHeightDificult;
            _intro.SetActive(false);
        }
    }

    private void OnStoryMode()
    {
        if (GameMode.CurrentGameType == GameMode.GameType.Story)
        {
            _score.SetActive(false);
            _recordScore.SetActive(false);
            _intro.SetActive(true);
            Height += OnStoryModeEnds;
        }
    }

    private void OnStoryModeEnds(float height)
    {
        if(height >= MaxHeightDificult)
        {
            _finalScreen.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void ChangeHeight(float height)
    {
        Height?.Invoke(height);
    }
}
