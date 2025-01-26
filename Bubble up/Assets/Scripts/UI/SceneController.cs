using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    [SerializeField]
    private string _sceneToLoad;

    [SerializeField]
    private Button _changeSceneBtn;

    [SerializeField]
    private Button _storyModeBtn;

    [SerializeField]
    private Button _infiniteModeBtn;

    [SerializeField]
    private Button _reloadBtn;

    [SerializeField]
    private Button _quit;

    [SerializeField]
    private GameObject _screenBlocker;

    private void Awake()
    {
        CheckBtnsAndAddLiseners();

        _screenBlocker.SetActive(false);
    }

    private void CheckBtnsAndAddLiseners()
    {
        if (_changeSceneBtn != null)
        {
            _changeSceneBtn.onClick.AddListener(ChangeScene);
        }

        if (_reloadBtn != null)
        {
            _reloadBtn.onClick.AddListener(ReloadScene);
        }
        if (_infiniteModeBtn != null)
        {
            _infiniteModeBtn.onClick.AddListener(() => GameMode.CurrentGameType = GameMode.GameType.Infinite);
            _infiniteModeBtn.onClick.AddListener(ChangeScene);
        }
        if (_storyModeBtn != null)
        {
            _storyModeBtn.onClick.AddListener(() => GameMode.CurrentGameType = GameMode.GameType.Story);
            _storyModeBtn.onClick.AddListener(ChangeScene);
        }
        if (_quit != null)
        {
            _quit.onClick.AddListener(Quit);
        }
    }

    private void Quit()
    {
        Application.Quit();
    }

    private void ReloadScene()
    {
        _screenBlocker.SetActive(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ChangeScene()
    {
        _screenBlocker.SetActive(true);
        StartCoroutine(LoadSceneCoroutine());
    }

    private IEnumerator LoadSceneCoroutine()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_sceneToLoad);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}