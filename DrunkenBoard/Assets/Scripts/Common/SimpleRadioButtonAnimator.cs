using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SimpleRadioButtonAnimator : MonoBehaviour
{
    private Animator _animator = null;

    private readonly int _selectTrigger = Animator.StringToHash("Selected");
    private readonly int _normalTrigger = Animator.StringToHash("Normal");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void PlaySelectAnimation()
    {
        _animator.SetTrigger(_selectTrigger);
    }

    public void PlayNormalAnimation()
    {
        _animator.SetTrigger(_normalTrigger);
    }
}