using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class FusionSession : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkRunner runnerPrefab;
    
    public NetworkRunner Runner { get; private set; }
    public Action<NetworkRunner> ActOnSceneLoadDone {get; set;}
    public Action<NetworkRunner,PlayerRef> ActOnPlayerJoined {get; set;}
    public Action<NetworkRunner,PlayerRef> ActOnPlayerLeft { get; set; }

    public void TryConnect(string lobbyName)
    {
        StartCoroutine(ConnectSharedSessionRoutine(lobbyName));
    }
    private IEnumerator ConnectSharedSessionRoutine(string sessionName)
    {
        GameManager.PopupManager.ToggleInteraction(false);

        if (Runner)
            Runner.Shutdown();

        Runner = Instantiate(runnerPrefab);
        Runner.AddCallbacks(this);
        
        var task = Runner.StartGame(
            new StartGameArgs
            {
                GameMode = GameMode.Shared,
                SessionName = sessionName,
                SceneManager = Runner.GetComponent<INetworkSceneManager>(),
                ObjectProvider = Runner.GetComponent<INetworkObjectProvider>(),
                Scene = SceneRef.FromIndex((int)ESceneType.MainGame)
            });
        yield return new WaitUntil(() => task.IsCompleted);
        
        GameManager.PopupManager.ToggleInteraction(true);

        var result = task.Result;
        Debug.Log($"StartGame Result: {result.ShutdownReason}");
        if (!result.Ok)
        {
            Debug.LogWarning(result.ShutdownReason);
        }
    }
    
    #region INetworkRunnerCallbacks
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        ActOnPlayerJoined?.Invoke(runner, player);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        ActOnPlayerLeft?.Invoke(runner, player);
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        ActOnSceneLoadDone?.Invoke(runner);
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }
    #endregion
}
