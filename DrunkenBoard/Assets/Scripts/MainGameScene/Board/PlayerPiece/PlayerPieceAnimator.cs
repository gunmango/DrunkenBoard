using UnityEngine;

public class PlayerPieceAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private readonly int HashX = Animator.StringToHash("X");
    private readonly int HashY = Animator.StringToHash("Y");

    public void SetMovement(Vector2 movement)
    {
        animator.SetFloat(HashX, movement.x);
        animator.SetFloat(HashY, movement.y);
    }
}
