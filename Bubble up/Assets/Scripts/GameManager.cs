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

        if(GameMode.CurrentGameType == GameMode.GameType.Infinite)
        {
            _maxHeightDificult = PlayerPrefs.GetFloat("MaxScore") > _maxHeightDificult ? _maxHeightDificult * 2 : _maxHeightDificult;
        }
    }

    public void ChangeHeight(float height)
    {
        Height?.Invoke(height);
    }
}
