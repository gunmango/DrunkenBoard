using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Dice : MonoBehaviour
{
    [SerializeField] private Animator diceAnimator;
    public int Result { get; set; }
    public bool IsRolling { get; private set; }

    private string[] _animNames = { "Dice1", "Dice2", "Dice3", "Dice4", "Dice5", "Dice6" };

    public void Awake()
    {
        IsRolling = false;
    }

    public void Roll()
    {
        if (IsRolling) return;

        IsRolling = true;
        Result = Random.Range(1, 7);
        diceAnimator.SetTrigger("Dice" + Result); 

        string selectedAnim = _animNames[Result - 1];
        diceAnimator.Play(selectedAnim);

        Invoke(nameof(FinishRoll), 1f);
    }

    private void FinishRoll()
    {
        IsRolling = false;
        Debug.Log(Result);
    }
}
