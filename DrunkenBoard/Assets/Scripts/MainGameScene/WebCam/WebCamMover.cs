using DG.Tweening;
using UnityEngine;
using static DG.Tweening.DOTween;

public class WebCamMover : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private float moveDuration = 2f;

    public void SetTo(Vector2 anchoredPosition)
    {
        _rectTransform.anchoredPosition = anchoredPosition;
    }

    public void MoveToween(Vector2 anchoredPosition)
    {
        _rectTransform.DOAnchorPos(anchoredPosition,moveDuration).SetEase(Ease.InOutQuad);
    }
}
