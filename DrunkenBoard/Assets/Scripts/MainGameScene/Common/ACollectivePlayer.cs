using System.Collections;
using Fusion;

public abstract class ACollectivePlayer : NetworkBehaviour
{
    public int Uuid { get; set; }
    public CollectiveSystem CollectiveSystem { get; set; }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public virtual void TakeAction_RPC()
    {
        StartCoroutine(TakeTurnCoroutine());
    }

    //자기턴에 해야할 행동, 코루틴 끝에 EndAction 호출해야함
    protected abstract IEnumerator TakeTurnCoroutine();

    protected virtual void EndAction()
    {
        CollectiveSystem.EndAction_RPC(this);
    }
}
