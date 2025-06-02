using Fusion;
using UnityEngine;
using System.Collections;

public class SingingEventProcessor : SimulationBehaviour
{
    [SerializeField] private CollectiveSystem collectiveSystem;
    [SerializeField] private SingingEventSinger singingEventSinger;
    [SerializeField] private SingingEventAudience singingEventAudience;
    [SerializeField] private AudienceVoteUpdater updater = null; 

    public void StartEvent(int enteredPlayerUuid)
    {
        SpawnAudiences();
        StartCoroutine(StartEventCo());
    }

    public void SpawnAudiences()
    {
        var audience = GameManager.FusionSession.Runner.Spawn(singingEventAudience);
        audience.Initialize(updater); 
        collectiveSystem.AddCollectivePlayer_RPC(audience);
    }

    private IEnumerator StartEventCo()
    {
        NetworkRunner runner = GameManager.FusionSession.Runner;
        
        yield return new WaitUntil(()=>collectiveSystem.Object.IsValid);
        
        if(runner.IsSharedModeMasterClient)
            collectiveSystem.StartSystem();
    }

}
