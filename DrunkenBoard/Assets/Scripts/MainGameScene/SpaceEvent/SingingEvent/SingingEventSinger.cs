using Fusion;
using UnityEngine;
using UnityEngine.UIElements;

public class SingingEventSinger : NetworkBehaviour
{
    [Networked] public int Uuid { get; set; }
    
    private AudienceVoteUpdater _updater = null;
    private SingingEventAudienceController _audienceController = null;
    private SingingEventProcessor _processor = null;

    public void Initialize(AudienceVoteUpdater updater, SingingEventAudienceController audienceController, SingingEventProcessor processor)
    {
        _updater = updater;
        _audienceController = audienceController;
        _processor = processor;
        _updater.CountDownTimer.ActOnEndTimer += EndEvent;
    }

    public void StartSinging()
    {
        _updater.gameObject.SetActive(true);
        _updater.StartCountDownButton.gameObject.SetActive(true);
        _updater.CountDownTimer.gameObject.SetActive(false);
        _updater.RadioButtonGroup.gameObject.SetActive(false);
        _updater.VoteGauge.gameObject.SetActive(false);

        _updater.StartCountDownButton.onClick.AddListener(EndSinging);
    }

    private void EndSinging()
    {
        _updater.StartCountDownButton.gameObject.SetActive(false);
        _audienceController.StartCountDown();
        _updater.CountDownTimer.gameObject.SetActive(true);
        _updater.CountDownTimer.StartCountDown_RPC(SpaceEventConstants.SingingEventVoteCountDown);
    }
    
    private void EndEvent()
    {
        _updater.CountDownTimer.ActOnEndTimer -= EndEvent;
        _processor.OnEndEvent();
        
        GameManager.FusionSession.Runner.Despawn(Object);
    }
    
    
}
