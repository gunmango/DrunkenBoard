using Fusion;
using UnityEditor;
using UnityEngine;

public class PlayerPieceAnimator : NetworkBehaviour
{
    [SerializeField] private Animator animator;

    [Networked, OnChangedRender(nameof(OnAnimationChanged))]
    public Vector3 CurrentMovement { get; set; }
    
    private readonly int HashX = Animator.StringToHash("X");
    private readonly int HashY = Animator.StringToHash("Y");
    

    public void SetMovement(Vector2 movement)
    {
        if (HasStateAuthority == false)
            return;
        
        CurrentMovement = new Vector3(movement.x, movement.y, 0);

        animator.SetFloat(HashX, movement.x);
        animator.SetFloat(HashY, movement.y);
    }

    private void OnAnimationChanged()
    {        
        if (HasStateAuthority)
            return;
        animator.SetFloat(HashX, CurrentMovement.x);
        animator.SetFloat(HashY, CurrentMovement.y);
    }
}
