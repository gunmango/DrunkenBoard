using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Fusion;
using UnityEngine;

public class MainGameStateManager : NetworkBehaviour
{
    [Networked ,OnChangedRender(nameof(OnChangedState))] public EMainGameState CurrentState { get; private set; }

    public Action ActOnStartGame { get; set; } //준비에서 첫시작
    public Action ActOnBoard { get; set; } //보드판
    public Action ActOnSpaceEvent { get; set; } //칸이벤트
    public Action ActOnDrinkTime { get; set; }
    public bool IsSpawned { get; private set; } = false;

    public override void Spawned()
    {
        // 마스터(또는 State Authority)만 초기 상태 설정
        if (Object.HasStateAuthority)
            CurrentState = EMainGameState.Ready;

        IsSpawned = true;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void ChangeState_RPC(EMainGameState newState)
    {
        if (newState == EMainGameState.Ready)
            return;
        CurrentState = newState;
    }

    public void OnChangedState(NetworkBehaviourBuffer previous)
    {
        var oldState = GetPropertyReader<EMainGameState>(nameof(CurrentState)).Read(previous);
        
        if (oldState == EMainGameState.Ready)
        {
            ActOnStartGame?.Invoke();
        }

        if (CurrentState == EMainGameState.Board)
        {
            ActOnBoard?.Invoke();
            return;
        }

        if (CurrentState == EMainGameState.DrinkTime)
        {
            ActOnDrinkTime?.Invoke();
            return;
        }
        
        ActOnSpaceEvent?.Invoke();
    }
    
}
