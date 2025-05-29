using UnityEngine;
using UnityEngine.Serialization;

public abstract class ASpaceEvent : MonoBehaviour
{
    public ESpaceEventType SpaceEventType = ESpaceEventType.None;
    public string EventName = string.Empty;

    public abstract void PlayEvent();
}
