using System.Collections.Generic;
using UnityEngine;

public class SpaceEventManager : MonoBehaviour
{
    public List<ASpaceEvent> SpaceEvents;

    public void PlayEvent(ESpaceEventType eventType)
    {
        SpaceEvents.Find(x => x.SpaceEventType == eventType).PlayEvent();
    }
}
