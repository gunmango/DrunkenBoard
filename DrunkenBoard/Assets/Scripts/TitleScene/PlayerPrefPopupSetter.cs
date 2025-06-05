using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
 
public class PlayerPrefPopupSetter : MonoBehaviour, IBasePopup
{
    [SerializeField] private CreateLobbyPopupSetter createLobbyPopup;
    [SerializeField] private JoinLobbyPopupSetter joinLobbyPopup;
    
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button joinLobbyButton;
    
    [SerializeField] private PlayerPrefPopupUpdater updater;
    
    private IBasePopup _nextPopup = null;
    
    private void Start()
    {
        createLobbyButton.onClick.AddListener(() =>
        {
            _nextPopup = createLobbyPopup;
            GameManager.PopupManager.OpenPopup(this);
        });
        joinLobbyButton.onClick.AddListener(()=>
        {
            _nextPopup = joinLobbyPopup;
            GameManager.PopupManager.OpenPopup(this);
        });
        
        updater.CloseButton.onClick.AddListener(()=>GameManager.PopupManager.CloseTopPopup());
        updater.OkButton.onClick.AddListener(TryOpenNextPopup);
    }

    public void Open(PopupDataBase data = null)
    {
        updater.gameObject.SetActive(true);
        updater.PaletteButtons.ResetSelectedButton();
    }

    public void Close()
    {
        updater.gameObject.SetActive(false);
    }

    private void TryOpenNextPopup()
    {
        if (updater.PaletteButtons.SelectedIndex == -1)
            return;

        if (updater.PlayerNameInputField.text == string.Empty)
            return;
        
        string playerName = updater.PlayerNameInputField.text;
        PaletteRadioButton button = updater.PaletteButtons.SelectedButton as PaletteRadioButton;
        if (button == null)
        {
            Debug.LogError("PaletteRadioButton is null");
            return;
        }
        EPlayerColor playerColor = button.PlayerColor;
        
        GameManager.Instance.LocalPlayerName = playerName;
        GameManager.Instance.LocalPlayerColor = playerColor;

        GameManager.PopupManager.OpenPopup(_nextPopup);
    }
}
