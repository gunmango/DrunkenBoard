using UnityEngine;

public class JoinLobbyPopupSetter : ABasePopup
{
    [SerializeField] private JoinLobbyPopupUpdater updater;
    
    private void Start()
    {
        updater.CloseButton.onClick.AddListener(()=>GameManager.PopupManager.CloseTopPopup());
        updater.JoinLobbyButton.onClick.AddListener(JoinLobby);
    }

    public override void Open()
    {
        updater.gameObject.SetActive(true);
    }

    public override void Close()
    {
        updater.gameObject.SetActive(false);
    }

    private void JoinLobby()
    {
        string lobbyName = updater.LobbyNameInputField.text;
        string hostIp = updater.HostIpInputField.text;
        TitleSceneManager.Instance.JoinLobby(lobbyName, hostIp);
    }
}
