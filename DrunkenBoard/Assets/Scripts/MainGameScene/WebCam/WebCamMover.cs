using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using static DG.Tweening.DOTween;

public class WebCamMover : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float moveDuration = 2f;

    public void SetTo(Vector2 anchoredPosition)
    {
        rectTransform.anchoredPosition = anchoredPosition;
    }

    public void MoveTween(Vector2 anchoredPosition)
    {
        rectTransform.DOAnchorPos(anchoredPosition,moveDuration).SetEase(Ease.InOutQuad);
    }
}
