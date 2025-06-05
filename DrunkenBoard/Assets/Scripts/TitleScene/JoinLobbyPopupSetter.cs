using UnityEngine;

public class JoinLobbyPopupSetter : MonoBehaviour, IBasePopup
{
    [SerializeField] private JoinLobbyPopupUpdater updater;
    
    private void Start()
    {
        updater.CloseButton.onClick.AddListener(()=>GameManager.PopupManager.CloseTopPopup());
        updater.JoinLobbyButton.onClick.AddListener(JoinLobby);
    }

    public void Open(PopupDataBase data = null)
    {
        updater.gameObject.SetActive(true);
    }

    public void Close()
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
