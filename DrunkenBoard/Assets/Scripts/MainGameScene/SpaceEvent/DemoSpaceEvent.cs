using System.Collections;
using UnityEngine;

public class DemoSpaceEvent : ASpaceEvent
{
    protected override void PlayEvent()
    {
        StartCoroutine(DemoWait());
    }

    private IEnumerator DemoWait()
    {
        yield return new WaitForSeconds(4f);
        
        MainGameSceneManager.GameStateManager.ChangeState_RPC(EMainGameState.Board);
    }
}
