using System;
using Fusion;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SingingEventProcessor : SimulationBehaviour
{
    [SerializeField] private SingingEventSinger singingEventSinger;
    [SerializeField] private SingingEventAudience singingEventAudience;
    [SerializeField] private AudienceVoteUpdater updater;
    [SerializeField] private SingingEventAudienceController audienceController;
    
    private int _singerUuid = 0;

    public void StartEvent(int enteredPlayerUuid)
    {
        _singerUuid = enteredPlayerUuid;
        updater.VoteGauge.ResetGauge();

        SetCamPos();

        if (GameManager.FusionSession.Runner.LocalPlayer.RawEncoded == enteredPlayerUuid)
        {
            SpawnSinger();
        }
        else
        {
            SpawnAudience();
        }
        StartCoroutine(StartEventCo());
    }

    private void SpawnAudience()
    {
        var audience = GameManager.FusionSession.Runner.Spawn(singingEventAudience);
        audience.Uuid = GameManager.FusionSession.Runner.LocalPlayer.RawEncoded;
        audience.Initialize(updater,this); 
        audienceController.AddAudience_RPC(audience);
    }

    private void SpawnSinger()
    {
        var singer = GameManager.FusionSession.Runner.Spawn(singingEventSinger);
        singer.Uuid = GameManager.FusionSession.Runner.LocalPlayer.RawEncoded;
        singer.Initialize(updater, audienceController,this);
        singer.StartSinging();
    }

    private IEnumerator StartEventCo()
    {
        yield return new WaitUntil(()=>audienceController.Object.IsValid);

        NetworkRunner runner = GameManager.FusionSession.Runner;
        int audienceCount = runner.ActivePlayers.Count() - 1;
        
        if(runner.IsSharedModeMasterClient)
            audienceController.StartVoting(audienceCount);
    }

    //나중에 팝업만들고 위치수정
    private void SetCamPos()
    {
        MainGameSceneManager.WebCamManager.SetCamToStageView(_singerUuid);
    }
    
    public void OnEndEvent()
    {
        MainGameSceneManager.WebCamManager.SetCamToNormalSize(_singerUuid);

        if (GameManager.FusionSession.Runner.IsSharedModeMasterClient)
        {
            audienceController.ClearAudiences_RPC();
        }

        
        List<int> drinker = new List<int>();
        if(updater.VoteGauge.IsBlueWinning)
            drinker.Add(_singerUuid);
        MainGameSceneManager.SpaceEventManager.CurrentSpaceEvent.EndEvent(drinker);
    }
}
