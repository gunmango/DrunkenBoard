using System;
using UnityEngine;
using UnityEngine.UI;

public class TitleSceneManager : ASceneManager<TitleSceneManager>
{
    public void CreateLobby(string lobbyName)
    {
        GameManager.SignalingClient.SetHostSignalingUrl();

        ToMainGame(lobbyName);
    }

    public void JoinLobby(string lobbyName, string hostIp)
    {
        GameManager.SignalingClient.SetSignalingUrl(hostIp);
        
        ToMainGame(lobbyName);
    }

    private void ToMainGame(string lobbyName)
    {
        GameManager.SignalingClient.SetRoomId(lobbyName);
        //GameManager.SignalingClient.JoinRoom();

        GameManager.FusionSession.TryConnect(lobbyName);
    }
    
}
