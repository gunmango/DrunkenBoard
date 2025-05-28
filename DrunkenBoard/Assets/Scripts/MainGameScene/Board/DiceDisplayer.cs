using UnityEngine;

public class DiceDisplayer : MonoBehaviour
{
    [SerializeField] private Animator diceAnimator;
    [SerializeField] private float delayDuration = 1f;
    public bool IsRolling { get; set; }
    
    private readonly string[] _animNames = { "Dice1", "Dice2", "Dice3", "Dice4", "Dice5", "Dice6" };
    
    public void Awake()
    {
        IsRolling = false;
    }

    public void ShowRoll(int result)
    {
        diceAnimator.SetTrigger("Dice" + result); 
        string selectedAnim = _animNames[result - 1];
        diceAnimator.Play(selectedAnim);

        Invoke(nameof(FinishRoll), delayDuration);
    }

    private void FinishRoll()
    {
        IsRolling = false;
    }
}