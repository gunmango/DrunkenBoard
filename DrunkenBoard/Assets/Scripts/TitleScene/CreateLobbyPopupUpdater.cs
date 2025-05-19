using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobbyPopupUpdater : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private Button joinLobbyButton;
    
    public TMP_InputField LobbyNameInputField => lobbyNameInputField;
    public Button JoinLobbyButton => joinLobbyButton;
}
