using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WebCamTweener : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private RectTransform rectTransform;
    private float _moveDuration = 2f;
    
    public void SetTo(Vector2 anchoredPosition)
    {
        rectTransform.anchoredPosition = anchoredPosition;
    }

    public void MoveTween(Vector2 anchoredPosition, Action onComplete = null)
    {
        rectTransform.DOAnchorPos(anchoredPosition, _moveDuration).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    public void ResizeTween(float width, float height, float rawImageWidth, float rawImageHeight, Action onComplete = null)
    {
        rectTransform.DOSizeDelta(new Vector2(width, height), _moveDuration).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
        
        rawImage.rectTransform.DOSizeDelta(new Vector2(rawImageWidth, rawImageHeight), _moveDuration).SetEase(Ease.InOutQuad);
    }
    
    public Vector2 GetAnchoredPosition()
    {
        return rectTransform.anchoredPosition;
    }

    private void Start()
    {
        _moveDuration = WebCamConstants.MoveDuration;
    }
}
