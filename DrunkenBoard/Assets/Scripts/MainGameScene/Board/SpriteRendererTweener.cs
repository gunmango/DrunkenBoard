using UnityEngine;
using DG.Tweening;

public class SpriteRendererTweener : MonoBehaviour
{
    [SerializeField] private SpriteRenderer image;
    [SerializeField] private float fadeDuration = 2f;

    public void FadeIn()
    {
        image.DOFade(1f, fadeDuration);
    }
    
    public void FadeOut()
    {
        image.DOFade(0f, fadeDuration);
    }
}
