using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WebCamTweener : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    [SerializeField] private RectTransform rectTransform;
    private float _moveDuration = 2f;

    [SerializeField] private GameObject boundary;
    private readonly float _onDuration = 0.3f;
    private readonly float _offDuration = 0.3f;
    private Sequence _blinkSequence;
    
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

    //태두리 깜빡이기
    public void ToggleBlink()
    {
        if (_blinkSequence != null && _blinkSequence.IsActive())
        {
            _blinkSequence.Kill();       // 시퀀스 중단
            boundary.SetActive(true);    // 항상 켜진 상태로 복원
            return;
        }
        
        _blinkSequence = DOTween.Sequence()
            .AppendCallback(() => boundary.SetActive(false))
            .AppendInterval(_offDuration)  // 꺼진 상태 유지
            .AppendCallback(() => boundary.SetActive(true))
            .AppendInterval(_onDuration)   // 켜진 상태 유지
            .SetLoops(-1);         // 반복 횟수 설정

        _blinkSequence.Play();
    }
    
    private void Start()
    {
        _moveDuration = WebCamConstants.MoveDuration;
    }
}
