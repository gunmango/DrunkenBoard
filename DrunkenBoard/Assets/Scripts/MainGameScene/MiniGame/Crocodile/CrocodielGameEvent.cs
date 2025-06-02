using UnityEngine;

public class CrocodielGameEvent : ASpaceEvent
{
    public void EndEvent()
    {
        MainGameSceneManager.GameStateManager.ChangeState_RPC(EMainGameState.Board);
    }

    public override void PlayEvent(int enteredPlayerUuid)
    {
        throw new System.NotImplementedException();
    }
}