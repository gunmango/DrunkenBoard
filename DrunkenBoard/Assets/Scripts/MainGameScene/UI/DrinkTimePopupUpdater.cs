using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DrinkTimePopupUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] nameTexts;
    [SerializeField] private Button startButton;
    [SerializeField] private SimplePopupAnimator animator;
    [SerializeField] private GameObject nameListParent;
    
    public TextMeshProUGUI[] NameTexts => nameTexts;
    public Button StartButton => startButton;
    public SimplePopupAnimator Animator => animator;
    public GameObject NameListParent => nameListParent;
}
