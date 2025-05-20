using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CreateLobbyPopupSetter : ABasePopup
{
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private CreateLobbyPopupUpdater updater;

    private void Start()
    {
        createLobbyButton.onClick.AddListener(()=>GameManager.PopupManager.OpenPopup(this));
        updater.CloseButton.onClick.AddListener(()=>GameManager.PopupManager.CloseTopPopup());
        updater.CreateLobbyButton.onClick.AddListener(CreateLobby);
    }

    public override void Open()
    {
        updater.gameObject.SetActive(true);
    }

    public override void Close()
    {
        updater.gameObject.SetActive(false);
    }

    private void CreateLobby()
    {
        TitleSceneManager.Instance.CreateLobby(updater.LobbyNameInputField.text);
    }
}
