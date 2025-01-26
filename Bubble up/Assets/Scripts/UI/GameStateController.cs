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

    private void Start()
    {
        Time.timeScale = 1;

        _pauseContainer.SetActive(false);
        _gameOverContainer.SetActive(false);
        _menuBtn.SetActive(false);
        _screenBlocker.SetActive(false);

        _pauseBtn.onClick.AddListener(() => SetTimeScaleAndObjectState(_pauseContainer, true));
        _continueBtn.onClick.AddListener(() => SetTimeScaleAndObjectState(_pauseContainer, false));

        GameManager.Instance.GameOver += GameOver;
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
        _menuBtn.SetActive(state);
        _screenBlocker.SetActive(state);
    }
}
