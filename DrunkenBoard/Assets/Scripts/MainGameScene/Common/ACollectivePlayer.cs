using System.Collections;
using Fusion;
using UnityEngine;

public abstract class ACollectivePlayer : NetworkBehaviour
{
    [Networked] public int Uuid { get; set; }
    public CollectiveSystem CollectiveSystem { get; set; }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public virtual void TakeAction_RPC()
    {
        Debug.Log($"{Uuid} Taking action");
        StartCoroutine(TakeTurnCoroutine());
    }

    //자기턴에 해야할 행동, 코루틴 끝에 EndAction 호출해야함
    protected abstract IEnumerator TakeTurnCoroutine();

    protected virtual void EndAction()
    {
        CollectiveSystem.EndAction_RPC(this);
    }
}
