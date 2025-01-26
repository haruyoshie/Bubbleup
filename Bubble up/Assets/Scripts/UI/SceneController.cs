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
    private Button _reloadBtn;

    [SerializeField]
    private GameObject _screenBlocker;

    private void Awake()
    {
        _changeSceneBtn.onClick.AddListener(ChangeScene);
        
        if(_reloadBtn != null)
        {
            _reloadBtn.onClick.AddListener(ReloadScene);
        }

        _screenBlocker.SetActive(false);
    }

    private void ReloadScene()
    {
        _screenBlocker.SetActive(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ChangeScene()
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