using System;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class DiceGameManager : MonoBehaviour
{
    public Dice dice;
    public Button rollButton;
    public int currentplayerIndex = 0;
    public int totalplayers = 2;

    private void Start()
    {
        rollButton.onClick.AddListener(OnRollDiceButton);
    }

    public void OnRollDiceButton()
    {
        if (IsMyTurn())
        {
            dice.Roll();
        }
    }

    public bool IsMyTurn()
    {
        //네트워크시 ID 와 currentplayerIndex가 맞는지 비교
        return true;
    }

    private void Update()
    {
        if (!dice.IsRolling && dice.Result != 0)
        {
            Debug.Log($"사용자:{currentplayerIndex+1}_{dice.Result}");
            dice.Result = 0;
            currentplayerIndex = (currentplayerIndex + 1) % totalplayers;
            
            rollButton.interactable = true;
        }
    }
}
