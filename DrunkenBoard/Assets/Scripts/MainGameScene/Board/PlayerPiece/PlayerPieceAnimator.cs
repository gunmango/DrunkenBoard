using Fusion;
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
        {
            Debug.Log("no authority");
            return;
        }
        
        CurrentMovement = new Vector3(movement.x, 0, movement.y);
        
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
