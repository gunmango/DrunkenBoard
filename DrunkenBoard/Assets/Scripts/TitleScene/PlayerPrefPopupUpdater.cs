using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerPrefPopupUpdater : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private Button okButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private RadioButtonGroup paletteButtonGroup;
    
    public TMP_InputField PlayerNameInputField => playerNameInputField;
    public Button OkButton => okButton;
    public Button CloseButton => closeButton;
    public RadioButtonGroup PaletteButtons => paletteButtonGroup;
}
