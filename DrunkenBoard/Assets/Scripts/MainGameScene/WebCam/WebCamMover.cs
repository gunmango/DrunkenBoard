using System;
using DG.Tweening;
using UnityEngine;

public class WebCamMover : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float moveDuration = 2f;

    public void SetTo(Vector2 anchoredPosition)
    {
        rectTransform.anchoredPosition = anchoredPosition;
    }

    public void MoveTween(Vector2 anchoredPosition, Action onComplete = null)
    {
        rectTransform.DOAnchorPos(anchoredPosition,moveDuration).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }
}
