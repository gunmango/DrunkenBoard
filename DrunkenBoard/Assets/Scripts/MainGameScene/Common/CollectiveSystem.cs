using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CollectiveSystem : NetworkBehaviour
{
    [Networked, Capacity(8)] public NetworkLinkedList<ACollectivePlayer> CollectivePlayers => default;
    [Networked, Capacity(8)] public NetworkLinkedList<ACollectivePlayer> FinishedPlayers => default;

    public Action<List<ACollectivePlayer>> ActOnEndSystem { get; set; }
    
    public void StartSystem(int totalPlayerCount)
    {
        FinishedPlayers.Clear();
        gameObject.SetActive(true);
        
        StartCoroutine(PlayersTakeAction(totalPlayerCount));
    }

    private IEnumerator PlayersTakeAction(int totalPlayerCount)
    {
        yield return new WaitUntil(() => CollectivePlayers.Count == totalPlayerCount);
        
        foreach (var collectivePlayer in CollectivePlayers)
        {
            collectivePlayer.TakeAction_RPC();
        }  
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void AddCollectivePlayer_RPC(ACollectivePlayer player)
    {
        CollectivePlayers.Add(player);
    }
    
    // 플레이어에서 턴이 행동이 끝났다고 알릴 때 호출
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void EndAction_RPC(ACollectivePlayer finishedPlayer)
    {
        if (FinishedPlayers.Contains(finishedPlayer))
        {
            Debug.LogWarning($"Player {finishedPlayer.name} has already finished action");
            return;
        }
        
        FinishedPlayers.Add(finishedPlayer);

        if (FinishedPlayers.Count == CollectivePlayers.Count)
        {
            EndSystem();
        }
    }

    public void EndSystem()
    {
        List<ACollectivePlayer> result = new List<ACollectivePlayer>();

        foreach (var finishedPlayer in FinishedPlayers)
        {
            result.Add(finishedPlayer);
        }
        
        ActOnEndSystem?.Invoke(result);
        FinishedPlayers.Clear();
        CollectivePlayers.Clear();
    }
}
