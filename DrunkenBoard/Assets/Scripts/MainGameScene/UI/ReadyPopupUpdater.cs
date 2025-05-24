using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReadyPopupUpdater : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private TextMeshProUGUI originalText;
    [SerializeField] private Button startButton;
    
    public Button StartButton => startButton;

    public TextMeshProUGUI CreateText(string text)
    {
        TextMeshProUGUI textMeshProUGUI = Instantiate(originalText, content);
        textMeshProUGUI.text = text;
        return textMeshProUGUI;
    }
}
