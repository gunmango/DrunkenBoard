using System.Collections.Generic;
using Fusion;

public class TurnSystem : NetworkBehaviour
{
    // 네트워크로 동기화할 현재 턴 플레이어 인덱스
    [Networked] public int CurrentPlayerIndex { get; set; }
    
    // 턴이 끝났음을 표시하는 네트워크 플래그
    [Networked] public bool TurnEnded { get; set; }
    
    [Networked, Capacity(8)] public NetworkLinkedList<ATurnPlayer> TurnPlayers => default;

    //public List<ATurnPlayer> TurnPlayers;
    
    public int GetCurrentPlayerUuid()
    {
        return TurnPlayers[CurrentPlayerIndex].Uuid;
    }
    
    public void AddTurnPlayer(ATurnPlayer turnPlayer)
    {        
        turnPlayer.TurnSystem = this;
        TurnPlayers.Add(turnPlayer);
    }   
    
    public void RemoveTurnPlayer(ATurnPlayer turnPlayer)
    {
        int idx = TurnPlayers.IndexOf(turnPlayer);
        if (idx < 0) return;
        
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
        }
    }
    
    // 외부에서 턴이 종료됐다고 알릴 때 호출x`z`
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void EndTurn_RPC()
    {
        TurnEnded = true;
    }


}
