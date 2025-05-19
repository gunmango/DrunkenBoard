using UnityEngine;
using UnityEngine.UI;

public class MainMenuUpdater : MonoBehaviour
{
    [SerializeField] private Button createLobbyButton = null;
    [SerializeField] private Button joinLobbyButton = null;
    [SerializeField] private Button optionButton = null;
    
    public Button CreateLobbyButton => createLobbyButton;
    public Button JoinLobbyButton => joinLobbyButton;
    public Button OptionButton => optionButton;
}
