using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyPopupUpdater : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private TMP_InputField hostIpInputField;
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private Button closeButton;
    
    public TMP_InputField LobbyNameInputField => lobbyNameInputField;
    public TMP_InputField HostIpInputField => hostIpInputField;
    public Button JoinLobbyButton => joinLobbyButton;
    public Button CloseButton => closeButton;
}
