using System.Collections;
using Fusion;
using UnityEngine;

public class BoardGameProcessor : SimulationBehaviour
{
    [SerializeField] private TurnSystem turnSystem;
    [SerializeField] private BoardGamePlayer originalPlayer;

    private void Start()
    {
        MainGameSceneManager.GameStateManager.ActOnBoard += StartBoardGame;
    }

    private void StartBoardGame()
    {
        StartCoroutine(StartBoardGameCo());
    }

    private IEnumerator StartBoardGameCo()
    {
        yield return new WaitUntil(()=>turnSystem.Object.IsValid);
        NetworkRunner runner = GameManager.FusionSession.Runner;
        var newPlayer = runner.Spawn(originalPlayer);
        newPlayer.Uuid = runner.LocalPlayer.RawEncoded;
        turnSystem.AddTurnPlayer(newPlayer);
        
        if(runner.IsSharedModeMasterClient)
            turnSystem.gameObject.SetActive(true);
    }
}
