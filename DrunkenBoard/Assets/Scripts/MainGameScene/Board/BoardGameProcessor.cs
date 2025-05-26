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
        NetworkRunner runner = GameManager.FusionSession.Runner;
        // if (runner.IsSharedModeMasterClient)
        //     runner.Spawn(turnSystem);
        
        yield return new WaitUntil(()=>turnSystem.Object.IsValid);
        
        var newPlayer = runner.Spawn(originalPlayer);
        newPlayer.Uuid = runner.LocalPlayer.RawEncoded;
        turnSystem.AddTurnPlayer(newPlayer);
        
        Debug.Log(runner.LocalPlayer.RawEncoded);
        
        turnSystem.gameObject.SetActive(true);
    }
}
