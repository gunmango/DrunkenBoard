using UnityEngine;
using DG.Tweening;

public class SpriteRendererTweener : MonoBehaviour
{
    [SerializeField] private SpriteRenderer image;
    [SerializeField] protected float fadeDuration = 2f;

    public virtual void FadeIn()
    {
        image.DOFade(1f, fadeDuration);
    }
    
    public virtual void FadeOut()
    {
        image.DOFade(0f, fadeDuration);
    }
}
