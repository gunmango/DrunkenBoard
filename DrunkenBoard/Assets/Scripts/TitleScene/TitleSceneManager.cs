using System;
using UnityEngine;
using UnityEngine.UI;

public class TitleSceneManager : ASceneManager<TitleSceneManager>
{
    private string _playerName = string.Empty;
    private EPlayerColor _playerColor = EPlayerColor.White;
    
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
        GameManager.FusionSession.TryConnect(lobbyName);
    }
}
