using System.Collections;
using UnityEngine;
using Fusion;
public abstract class ATurnPlayer : NetworkBehaviour
{
    public int Uuid { get; set; }
    public TurnSystem TurnSystem { get; set; }
    protected Coroutine _takeTurnCoroutine = null;

    public override void Spawned()
    {
        base.Spawned();
    }
    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) 
            return;   // 내 로컬 클라이언트만 아래 로직 실행
        
        // 내 차례가 아니면 아무 것도 안 함
        if (TurnSystem.GetCurrentPlayerUuid() != Uuid) return;

        // 한 번만 호출하도록 
        if (_takeTurnCoroutine == null)
        {
            _takeTurnCoroutine = Runner.StartCoroutine(TakeTurn());
        }
    }
    
    //자기턴에 해야할 행동, 코루틴 끝에 EndTurn 호출해야함
    protected abstract IEnumerator TakeTurn();

    protected virtual void EndTurn()
    {
        _takeTurnCoroutine = null;
        TurnSystem.EndTurn_RPC();
    }
}
