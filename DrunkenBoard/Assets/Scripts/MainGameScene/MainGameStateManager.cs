using System;
using Fusion;
using UnityEngine;

public class MainGameStateManager : NetworkBehaviour
{
    [Networked] public EMainGameState CurrentState { get; private set; }
    
    public Action ActOnStartGame { get; set; }  //준비에서 첫시작
    public Action ActOnBoard { get; set; } //보드판
    public Action ActOnSpaceEvent { get; set; } //칸이벤트
    public bool IsSpawned { get; private set; } = false;

    public override void Spawned()
    {
        // 마스터(또는 State Authority)만 초기 상태 설정
        if (Object.HasStateAuthority)
            CurrentState = EMainGameState.Ready;

        IsSpawned = true;
    }

    public void ChangeState(EMainGameState newState)
    {
        if (!Object.HasStateAuthority)
            return;
        
        if (CurrentState == EMainGameState.Ready)
        {
            ActOnStartGame?.Invoke();
        }
        CurrentState = newState;

        if (CurrentState == EMainGameState.Board)
        {
            ActOnBoard?.Invoke();
            return;
        }
        
        ActOnSpaceEvent?.Invoke();
    }
    
}
