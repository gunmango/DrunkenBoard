using UnityEngine;

public class CreateLobbyPopupSetter : MonoBehaviour, IBasePopup
{
    [SerializeField] private CreateLobbyPopupUpdater updater;

    private void Start()
    {
        updater.CloseButton.onClick.AddListener(()=>GameManager.PopupManager.CloseTopPopup());
        updater.CreateLobbyButton.onClick.AddListener(CreateLobby);
    }

    public void Open(PopupDataBase data = null)
    {
        updater.gameObject.SetActive(true);
    }

    public void Close()
    {
        updater.gameObject.SetActive(false);
    }

    private void CreateLobby()
    {
        TitleSceneManager.Instance.CreateLobby(updater.LobbyNameInputField.text);
    }
}
