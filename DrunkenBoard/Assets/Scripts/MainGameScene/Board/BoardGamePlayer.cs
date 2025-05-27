using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BoardGamePlayer : ATurnPlayer
{
    [SerializeField] private Button rollDiceButton;
    private DiceSetter _diceSetter;

    private bool _clicked = false;

    public void Initialize(DiceSetter diceSetter, TurnSystem turnSystem, int uuid)
    {
        TurnSystem = turnSystem;
        _diceSetter = diceSetter;
        Uuid = uuid;
    }
    
    public override void Spawned()
    {
        base.Spawned();
        rollDiceButton.onClick.AddListener(()=>
        {
            rollDiceButton.gameObject.SetActive(false);
            _clicked = true;
            _diceSetter.SetRandomResult_RPC();
        });
        rollDiceButton.gameObject.SetActive(false);
    }
    
    protected override IEnumerator TakeTurnCoroutine()
    {
        yield return new WaitWhile(() => _diceSetter.IsRolling);
        rollDiceButton.gameObject.SetActive(true);
        
        yield return new WaitUntil(() => _clicked);
        yield return new WaitWhile(() => _diceSetter.IsRolling);
        
        _clicked = false;
        EndTurn();
    }
}
