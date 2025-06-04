using UnityEngine;

public class CreateLobbyPopupSetter : ABasePopup
{
    [SerializeField] private CreateLobbyPopupUpdater updater;

    private void Start()
    {
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
