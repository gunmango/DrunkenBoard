using DG.Tweening;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class VoteGauge : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnChangedCount))] public int RedCount {get; set;}
    [Networked, OnChangedRender(nameof(OnChangedCount))] public int BlueCount {get; set;}
    
    [SerializeField] private Image redFill;
    [SerializeField] private Image blueFill;
    
    private Tween _redFillTween;
    private Tween _blueFillTween;
    public bool IsBlueWinning => BlueCount > RedCount;
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void AddRedCount_RPC(int redCount)
    {
        RedCount += redCount;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void AddBlueCount_RPC(int blueCount)
    {
        BlueCount += blueCount;
    }
    
    public void ResetGauge()
    {
        RedCount = 0;
        BlueCount = 0;
        redFill.fillAmount = 0;
        blueFill.fillAmount = 0;
    }
    
    private void OnChangedCount()
    {
        UpdateGauge(RedCount, BlueCount);
    }
    
    public void UpdateGauge(int redCount, int blueCount)
    {
        int total = redCount + blueCount;

        if (total == 0)
        {
            _redFillTween?.Kill();
            _blueFillTween?.Kill();
            
            redFill.fillAmount = 0;
            blueFill.fillAmount = 0;
            return;
        }
        
        float redRatio = (float)redCount / total;
        float blueRatio = (float)blueCount / total;
        
        _redFillTween?.Kill();
        _blueFillTween?.Kill();
        
        _redFillTween = redFill.DOFillAmount(redRatio, 0.5f).SetEase(Ease.OutCubic);
        _blueFillTween = blueFill.DOFillAmount(blueRatio, 0.5f).SetEase(Ease.OutCubic);
    }
}
