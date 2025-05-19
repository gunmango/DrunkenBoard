using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobbyPopupUpdater : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button closeButton;
    public TMP_InputField LobbyNameInputField => lobbyNameInputField;
    public Button CreateLobbyButton => createLobbyButton;
    public Button CloseButton => closeButton;
}
