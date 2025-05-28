using UnityEngine;
using UnityEngine.Serialization;

public abstract class ASpaceEvent : MonoBehaviour
{
    public ESpaceEventType SpaceEventType = ESpaceEventType.None;

    public abstract void PlayEvent();
}
