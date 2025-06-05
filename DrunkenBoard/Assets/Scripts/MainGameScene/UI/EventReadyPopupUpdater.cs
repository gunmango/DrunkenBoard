using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventReadyPopupUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI eventNameText;
    [SerializeField] private Button startButton;
    [SerializeField] private SimplePopupAnimator animator;
    public TextMeshProUGUI EventNameText => eventNameText;
    public Button StartButton => startButton;
    public SimplePopupAnimator Animator => animator;
}
