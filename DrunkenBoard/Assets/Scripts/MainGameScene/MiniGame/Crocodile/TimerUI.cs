using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    public TextMeshProUGUI timerText;

    public void UpdateTimer(float seconds)
    {
        timerText.text = Mathf.CeilToInt(seconds).ToString();
    }

    public void Hide() => timerText.gameObject.SetActive(false);
    public void Show() => timerText.gameObject.SetActive(true);
}