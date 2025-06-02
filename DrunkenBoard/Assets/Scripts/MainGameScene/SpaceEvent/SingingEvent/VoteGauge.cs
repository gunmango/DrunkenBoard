using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class VoteGauge : MonoBehaviour
{
    [SerializeField] private Image redFill;
    [SerializeField] private Image blueFill;
    
    private Tween _redFillTween;
    private Tween _blueFillTween;

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
        float blueRatio = (float)redCount / total;

        _redFillTween?.Kill();
        _blueFillTween?.Kill();
        
        _redFillTween = redFill.DOFillAmount(redRatio, 0.5f).SetEase(Ease.OutCubic);
        _blueFillTween = blueFill.DOFillAmount(blueRatio, 0.5f).SetEase(Ease.OutCubic);
    }
}
