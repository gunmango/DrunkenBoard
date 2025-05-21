using UnityEngine;

public class WebCamMover : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;

    public void MoveTo(Vector2 anchoredPosition)
    {
        _rectTransform.anchoredPosition = anchoredPosition;
    }
}
