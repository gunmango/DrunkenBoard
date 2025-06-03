using System;
using Fusion;
using TMPro;
using UnityEngine;

public class NetworkTimer : NetworkBehaviour
{
    [Networked] private float StartTime {get; set;}
    [Networked] private bool IsPlaying { get; set; }
    [Networked] private float Duration { get; set; }

    public Action ActOnEndTimer { get; set; }
    
    [SerializeField] private TextMeshProUGUI timerText;
    private bool _hasEnded = false;

    public override void FixedUpdateNetwork()
    {
        if (_hasEnded)
            return;
        if (!IsPlaying)
            return;
        if(!Runner.IsRunning)
            return;
        
        float elapsed = Runner.SimulationTime - StartTime;
        float leftTime = Duration - elapsed;
        
        UpdateText_RPC(leftTime);

        if (leftTime <= 0 && !_hasEnded)
        {
            _hasEnded = true;
            IsPlaying = false;
            EndTimer_RPC();
        }
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void StartCountDown_RPC(float time)
    {
        StartTime = Runner.SimulationTime;
        Duration = time;
        IsPlaying = true;
        _hasEnded = false;
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void StopCountDown_RPC()
    {
        IsPlaying = false;
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void UpdateText_RPC(float leftTime)
    {
        timerText.text = $"남은 시간 {Mathf.CeilToInt(leftTime)}";
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void EndTimer_RPC()
    {
        ActOnEndTimer?.Invoke();
    }
}
