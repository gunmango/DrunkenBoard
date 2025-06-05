using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SimplePopupAnimator : MonoBehaviour
{
    public Action ActOnEndOpen = null;
    public Action ActOnEndClose = null;
    
    private Animator _animator = null;
    
    private readonly int _openTrigger = Animator.StringToHash("Open");
    private readonly int _closeTrigger = Animator.StringToHash("Close");
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void PlayOpenAnimation()
    {
        _animator.SetTrigger(_openTrigger);
    }

    public void PlayCloseAnimation()
    {
        _animator.SetTrigger(_closeTrigger);
    }
    
    public void OnEndOpenAnimation()
    {
        ActOnEndOpen?.Invoke();
    }

    public void OnEndCloseAnimation()
    {
        ActOnEndClose?.Invoke();
    }
}
