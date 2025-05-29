using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpriteRendererTweener : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> images;
    [SerializeField] protected float fadeDuration = 2f;

    public virtual void FadeIn()
    {
        foreach (SpriteRenderer image in images)
        {
            image.DOFade(1f, fadeDuration);
        }
    }
    
    public virtual void FadeOut()
    {
        foreach (SpriteRenderer image in images)
        {
            image.DOFade(0f, fadeDuration);
        }
    }
}
