using System.Collections;
using UnityEngine;
using Fusion;
public abstract class ATurnPlayer : NetworkBehaviour
{
    [Networked] public int Uuid { get; set; }
    public TurnSystem TurnSystem { get; set; }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public virtual void TakeTurn_RPC()
    {
        StartCoroutine(TakeTurnCoroutine());
    }
    
    //자기턴에 해야할 행동, 코루틴 끝에 EndTurn 호출해야함
    protected abstract IEnumerator TakeTurnCoroutine();

    protected virtual void EndTurn()
    {
        TurnSystem.EndTurn_RPC();
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void WebCamStartBlinking_RPC()
    {
        MainGameSceneManager.WebCamManager.StartBlinkingBoundary(Uuid);
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void WebCamStopBlinking_RPC()
    {
        MainGameSceneManager.WebCamManager.StopBlinkingBoundary(Uuid);
    }
}
