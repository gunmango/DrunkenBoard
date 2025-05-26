using System.Collections;
using UnityEngine;
using Fusion;
public abstract class ATurnPlayer : NetworkBehaviour
{
    public int Uuid { get; set; }
    public TurnSystem TurnSystem { get; set; }
    protected Coroutine _takeTurnCoroutine = null;
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public virtual void TakeTurn_RPC()
    {
        StartCoroutine(TakeTurnCoroutine());
    }
    
    //자기턴에 해야할 행동, 코루틴 끝에 EndTurn 호출해야함
    protected abstract IEnumerator TakeTurnCoroutine();

    protected virtual void EndTurn()
    {
        _takeTurnCoroutine = null;
        TurnSystem.EndTurn_RPC();
    }
}
