using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dice : MonoBehaviour
{
    public Animator Diceanimator;
    public int result { get; set; }
    public bool isRolling { get; private set; }

    private string[] animNames = { "Dice1", "Dice2", "Dice3", "Dice4", "Dice5", "Dice6" };

    public void Awake()
    {
        isRolling = false;
    }

    public void Roll()
    {
        if (isRolling) return;

        isRolling = true;
        result = Random.Range(1, 7);
        Diceanimator.SetTrigger("Dice" + result); 

        string selectedAnim = animNames[result - 1];
        Diceanimator.Play(selectedAnim);

        Invoke(nameof(FinishRoll), 1f);
    }

    private void FinishRoll()
    {
        isRolling = false;
        Debug.Log(result);
    }
}
