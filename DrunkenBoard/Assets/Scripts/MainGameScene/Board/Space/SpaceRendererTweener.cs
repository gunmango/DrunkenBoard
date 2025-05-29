using UnityEngine;
using TMPro;
using DG.Tweening;

public class SpaceRendererTweener : SpriteRendererTweener
{
    [SerializeField] private TextMeshPro eventNameText;

    public override void FadeIn()
    {
        base.FadeIn();
        eventNameText.DOFade(1f, fadeDuration);

    }

    public override void FadeOut()
    {
        base.FadeOut();
        eventNameText.DOFade(0f, fadeDuration);
    }
}
