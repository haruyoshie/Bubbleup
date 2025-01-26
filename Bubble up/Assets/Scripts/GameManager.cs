using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public event Action<float> Height;
    public Action<bool> GameOver;

    [SerializeField]
    private float _maxHeightDificult;
    [SerializeField]
    private bool _gameOverState;

    public GameObject gameOverUI;

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
        GameOver += (bool value)=> _gameOverState = value;
    }

    public void ChangeHeight(float height)
    {
        Height?.Invoke(height);
    }

    public void RestarScene()
    {
        SceneManager.LoadScene(0);
    }
    
}
