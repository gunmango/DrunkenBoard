using UnityEngine;

public class SingingEvent : ASpaceEvent
{
    [SerializeField] private SingingEventProcessor processor;
    
    public override void PlayEvent(int enteredPlayerUuid)
    {
        processor.StartEvent(enteredPlayerUuid);
    }
}
