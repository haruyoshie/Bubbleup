using TMPro;
using UnityEngine;

public class MaxScoreUI : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI _maxScore;
    void Start()
    {
        _maxScore.SetText(PlayerPrefs.GetFloat("MaxScore", 0).ToString("F1")+"m");
    }
}
