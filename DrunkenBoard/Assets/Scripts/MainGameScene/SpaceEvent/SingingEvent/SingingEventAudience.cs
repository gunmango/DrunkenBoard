using System.Collections;
using UnityEngine;

public class SingingEventAudience : ACollectivePlayer
{
    private AudienceVoteUpdater _updater = null; 
    
    public void Initialize(AudienceVoteUpdater updater)
    {
        _updater = updater;
    }

    protected override IEnumerator TakeTurnCoroutine()
    {
        _updater.gameObject.SetActive(true);
        yield return null;
    }
}
