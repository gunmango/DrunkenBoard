using UnityEngine;
using Fusion;

public class DiceSetter : NetworkBehaviour
{
    [SerializeField] private DiceDisplayer diceDisplayer;
    
    [Networked] public int DiceResult { get; set; }
    public int DiceNumber { get; set; }
    public bool IsRolling => diceDisplayer.IsRolling;
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void SetRandomResult_RPC()
    {
        int rand = Random.Range(1, 7);
        DiceResult = rand;
        BroadcastDice_RPC(rand);
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void BroadcastDice_RPC(int result)
    {
        ShowDiceRoll(result);
    }
    
    private void ShowDiceRoll(int result)
    {
        diceDisplayer.ShowRoll(result);
        DiceNumber = result;
    }


}
