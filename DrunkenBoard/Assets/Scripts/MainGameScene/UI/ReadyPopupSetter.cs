using System.Collections;
using UnityEngine;
using Fusion;

public class ReadyPopupSetter : SimulationBehaviour
{
    [SerializeField] private NetworkPrefabRef popupPrefab;

    private void Start()
    {
        StartCoroutine(OpenReadyPopupCo());
    }
    
    private IEnumerator OpenReadyPopupCo()
    {
        NetworkRunner runner = GameManager.FusionSession.Runner;
        yield return new WaitUntil(()=> runner.IsRunning);

        if(!runner.IsSharedModeMasterClient)
            yield break;
        
        runner.Spawn(popupPrefab, Vector3.zero, Quaternion.identity);
    }
}
