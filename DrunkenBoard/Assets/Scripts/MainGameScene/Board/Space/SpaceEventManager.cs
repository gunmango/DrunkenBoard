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

        SpaceEvents.Find(x => x.EventType == eventType).PlayEvent(enteredPlayerUuid);
    }
}
