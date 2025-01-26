using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameStateController : MonoBehaviour
{
    [SerializeField]
    private GameObject _pauseContainer;

    [SerializeField]
    private GameObject _gameOverContainer;

    [SerializeField] 
    private GameObject _menuBtn;

    [SerializeField] 
    private GameObject _screenBlocker;

    [SerializeField]
    private Button _pauseBtn;

    [SerializeField]
    private Button _continueBtn;

    [SerializeField]
    private PlayerBehaviour _playerBehaviour;

    [SerializeField]
    private GameObject _tutorial;

    private void Start()
    {
        Time.timeScale = 1;

        _pauseContainer.SetActive(false);
        _gameOverContainer.SetActive(false);
        _menuBtn.SetActive(false);
        _screenBlocker.SetActive(false);

        _pauseBtn.onClick.AddListener(() => SetTimeScaleAndObjectState(_pauseContainer, true));
        _continueBtn.onClick.AddListener(() => SetTimeScaleAndObjectState(_pauseContainer, false));

        _playerBehaviour.JumpState += TutorialState;

        GameManager.Instance.GameOver += GameOver;
    }

    private void TutorialState(bool obj)
    {
        if (obj)
        {
            _tutorial.SetActive(false);
            _playerBehaviour.JumpState -= TutorialState;
        }
    }

    private async void GameOver(bool state)
    {
        await Task.Delay(1000);
        SetTimeScaleAndObjectState(_gameOverContainer, state);
    }

    private void SetTimeScaleAndObjectState(GameObject @object, bool state)
    {
        Time.timeScale = !state ? 1 : 0;
        @object?.SetActive(state);
        _menuBtn?.SetActive(state);
        _screenBlocker?.SetActive(state);
    }
}
