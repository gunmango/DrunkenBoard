using System.Collections;
using Fusion;
using UnityEngine;

public class SingingEventAudience : NetworkBehaviour
{
    [Networked] public int Uuid { get; set; }
    
    private AudienceVoteUpdater _updater = null; 
    private SingingEventProcessor _processor = null;
    
    private int _currentVote = -1;  //-1선택안함, 0빨강, 1파랑
    
    public void Initialize(AudienceVoteUpdater updater, SingingEventProcessor processor)
    {
        _updater = updater;
        _processor = processor;
        
        updater.RadioButtonGroup.OnValueChanged += ChangeGauge;
        updater.CountDownTimer.ActOnEndTimer += EndVoting;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void StartVoting_RPC()
    {
        _updater.gameObject.SetActive(true);
        _updater.VoteGauge.gameObject.SetActive(true);
        _updater.RadioButtonGroup.gameObject.SetActive(true);
        _updater.CountDownTimer.gameObject.SetActive(false);
    }

    private void ChangeGauge(int buttonIndex)
    {
        if (_currentVote == 0)
            _updater.VoteGauge.AddRedCount_RPC(-1);
        else if (_currentVote == 1)
            _updater.VoteGauge.AddBlueCount_RPC(-1);
        
        _currentVote = buttonIndex;

        if (buttonIndex == 0)
            _updater.VoteGauge.AddRedCount_RPC(1);
        else if(buttonIndex == 1)
            _updater.VoteGauge.AddBlueCount_RPC(1);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void StartCountDown_RPC()
    {
        _updater.CountDownTimer.gameObject.SetActive(true);
    }

    private void EndVoting()
    {
        _updater.RadioButtonGroup.OnValueChanged -= ChangeGauge;
        _updater.CountDownTimer.ActOnEndTimer -= EndVoting;

        _processor.OnEndEvent();
        _updater.gameObject.SetActive(false);
        
        GameManager.FusionSession.Runner.Despawn(Object);
    }
}
