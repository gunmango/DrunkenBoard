using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class TurnSystem : NetworkBehaviour
{
    // 턴이 끝났음을 표시하는 네트워크 플래그
    [Networked] public bool TurnEnded { get; set; }
    
    // 네트워크로 동기화할 현재 턴 플레이어 인덱스
    [Networked] public int CurrentPlayerIndex { get; set; }

    [Networked, Capacity(8)] public NetworkLinkedList<ATurnPlayer> TurnPlayers => default;
    
    public void StartSystem()
    {
        gameObject.SetActive(true);

        TurnEnded = true;
        CurrentPlayerIndex = -1;
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void AddTurnPlayer_RPC(ATurnPlayer turnPlayer)
    {        
        TurnPlayers.Add(turnPlayer);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RemoveTurnPlayer_RPC(int uuid)
    {
        int idx = 0;
        ATurnPlayer turnPlayer = null;
        for (int i = 0; i < TurnPlayers.Count; i++)
        {
            if (TurnPlayers[i].Uuid == uuid)
            {
                idx = i;
                turnPlayer = TurnPlayers[i];
                break;
            }
        }

        if (turnPlayer == null)
            return;
        
        TurnPlayers.Remove(turnPlayer);

        // CurrentPlayerIndex 보정
        if (TurnPlayers.Count == 0)
        {
            CurrentPlayerIndex = 0;
        }
        else if (idx <= CurrentPlayerIndex)
        {
            CurrentPlayerIndex = (CurrentPlayerIndex - 1 + TurnPlayers.Count) % TurnPlayers.Count;
        }
        
        // 만약 “지금 턴이었던 사람이 삭제”라면 즉시 턴 넘기기
        if (idx == CurrentPlayerIndex)
            TurnEnded = true;
    }
    
    public override void FixedUpdateNetwork()
    {
        // StateAuthority(서버 혹은 호스트) 만이 턴을 넘깁니다.
        if (!Object.HasStateAuthority) return;

        if (TurnEnded)
        {
            TurnEnded = false;
            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % TurnPlayers.Count;
            TurnPlayers[CurrentPlayerIndex].TakeTurn_RPC();
        }
    }
    
    // 외부에서 턴이 종료됐다고 알릴 때 호출
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void EndTurn_RPC()
    {
        TurnEnded = true;
    }


}
