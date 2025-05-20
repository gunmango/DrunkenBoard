using System;
using UnityEngine;
using UnityEngine.UI;

public class TitleSceneManager : ASceneManager<TitleSceneManager>
{
    public void CreateLobby(string lobbyName)
    {
        GameManager.SignalingClient.SetRoomId(lobbyName);
        GameManager.SignalingClient.SetHostSignalingUrl();

        Debug.Log("Create lobby");
        GameManager.SignalingClient.OnConnected += LoadLobby;
        GameManager.SignalingClient.JoinRoom();
    }

    public void JoinLobby(string lobbyName, string hostIp)
    {
        GameManager.SignalingClient.SetRoomId(lobbyName);
        GameManager.SignalingClient.SetSignalingUrl(hostIp);
        
        GameManager.SignalingClient.OnConnected += LoadLobby;
        GameManager.SignalingClient.JoinRoom();

    }

    private void LoadLobby()
    {
        GameManager.SceneController.LoadScene(ESceneType.Lobby);
    }

    private void OnDestroy()
    {
        GameManager.SignalingClient.OnConnected -= LoadLobby;
    }
}
