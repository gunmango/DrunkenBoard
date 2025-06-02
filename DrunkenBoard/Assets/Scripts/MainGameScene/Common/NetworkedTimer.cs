using System;
using System.Collections;
using Fusion;
using TMPro;
using UnityEngine;

public class NetworkedTimer : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(UpdateText))] public float LeftTime {get; private set;}

    [SerializeField] private TextMeshProUGUI timerText;
    [Networked] private bool IsPlaying { get; set; }
    public Action ActOnEndTimer { get; set; }

    public override void FixedUpdateNetwork()
    {
        if (!IsPlaying || !Runner.IsRunning)
            return;
            
        if (Runner.IsSharedModeMasterClient)
            LeftTime -= Runner.DeltaTime;
        
        //UpdateText();
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void StartCountDown_RPC(float time)
    {
        IsPlaying = true;
        LeftTime = time;
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void StopCountDown_RPC()
    {
        IsPlaying = false;
    }
    
    //이거 singer에선 호출됨 처리해야함
    private void UpdateText()
    {
        timerText.text = $"남은 시간 {Mathf.CeilToInt(LeftTime)}";

        if (LeftTime > 0)
        {
            return;
        }

        Debug.Log("End Time");
        IsPlaying = false;
        ActOnEndTimer?.Invoke();
    }
}
