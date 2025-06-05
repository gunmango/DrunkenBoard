using UnityEngine;

public class SingingEvent : ASpaceEvent
{
    [SerializeField] private SingingEventProcessor processor;
    
    protected override void PlayEvent()
    {
        processor.StartEvent(_enteredPlayerUuid);
    }
}
