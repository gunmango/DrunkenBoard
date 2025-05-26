using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BoardGamePlayer : ATurnPlayer
{
    [SerializeField] private Button rollDiceButton;

    private bool isClicked = false;
    
    public override void Spawned()
    {
        base.Spawned();
        rollDiceButton.onClick.AddListener(()=> isClicked = true);
        rollDiceButton.gameObject.SetActive(false);
    }
    
    protected override IEnumerator TakeTurnCoroutine()
    {
        Debug.Log($"{Uuid} is taking turn");
        rollDiceButton.gameObject.SetActive(true);
        yield return new WaitUntil(() => isClicked);
        Debug.Log($"{Uuid} turn ended");
        isClicked = false;
        rollDiceButton.gameObject.SetActive(false);
        EndTurn();
    }
    
    
}
