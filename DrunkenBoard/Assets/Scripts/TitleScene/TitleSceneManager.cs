using UnityEngine;
using UnityEngine.UI;

public class TitleSceneManager : ASceneManager<TitleSceneManager>
{
    public static void CreateLobby(string lobbyName)
    {
        GameManager.SignalingClient.SetRoomId(lobbyName);
        GameManager.SceneController.LoadScene(ESceneType.Lobby);
    }
}
