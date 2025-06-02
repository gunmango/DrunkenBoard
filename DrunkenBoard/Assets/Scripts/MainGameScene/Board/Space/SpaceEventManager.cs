using System.Collections.Generic;
using UnityEngine;

public class SpaceEventManager : MonoBehaviour
{
    public List<ASpaceEvent> SpaceEvents;

    public void PlayEvent(ESpaceEventType eventType, int enteredPlayerUuid)
    {
        SpaceEvents.Find(x => x.EventType == eventType).PlayEvent(enteredPlayerUuid);
    }
}
