using UnityEngine;

public class CrocodielGameEvent : ASpaceEvent
{
    public override void PlayEvent()
    {
        
    }

    public void EndEvent()
    {
        MainGameSceneManager.GameStateManager.ChangeState_RPC(EMainGameState.Board);
    }
}