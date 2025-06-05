using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class SpaceEventManager : NetworkBehaviour
{
    public List<ASpaceEvent> SpaceEvents;
    
    public void PlayEvent(ESpaceEventType eventType, int enteredPlayerUuid)
    {
        BroadCastEvent_RPC(eventType, enteredPlayerUuid);
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    private void BroadCastEvent_RPC(ESpaceEventType eventType, int enteredPlayerUuid)
    {
        if (GameManager.FusionSession.Runner.IsResimulation)
            return;

        ASpaceEvent spaceEvent = SpaceEvents.Find(x => x.EventType == eventType);
        if (spaceEvent == null)
        {
            Debug.LogError("event not found");
            return;
        }
        
        spaceEvent.ReadyEvent(enteredPlayerUuid);
    }
}
