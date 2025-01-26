using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _currentScore;

    [SerializeField]
    private TextMeshProUGUI _finalScore;

    private float _score;

    private void Start()
    {
        GameManager.Instance.Height += UpdateScore;
        GameManager.Instance.GameOver += SaveScore;
    }

    private void SaveScore(bool state)
    {
        if(GameMode.CurrentGameType == GameMode.GameType.Story)
        {
            GameManager.Instance.Height -= UpdateScore;
            GameManager.Instance.GameOver -= SaveScore;
            return;
        }

        if (!state) return;

        string score = _score.ToString("F1");

        if (_score > PlayerPrefs.GetFloat("MaxScore", 0))
        {
            PlayerPrefs.SetFloat("MaxScore", _score);
            _finalScore.SetText($"Nuevo Record: {score}m");
        }
        else
        {
            _finalScore.SetText($"Altura alcanzada: {score}m");
        }
    }

    private void UpdateScore(float value)
    {
        _currentScore.SetText($"Puntaje = {value.ToString("F1")}m");
        _score = value;
    }
}