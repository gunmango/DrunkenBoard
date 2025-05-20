using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyPopupSetter : ABasePopup
{
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private JoinLobbyPopupUpdater updater;
    
    private void Start()
    {
        joinLobbyButton.onClick.AddListener(()=>GameManager.PopupManager.OpenPopup(this));
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
