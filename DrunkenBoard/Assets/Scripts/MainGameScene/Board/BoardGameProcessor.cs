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
        
        GameManager.FusionSession.ActOnPlayerJoined += OnPlayerJoined;
    }

    private void OnPlayerJoined(NetworkRunner arg1, PlayerRef arg2)
    {
        if (arg1.LocalPlayer != arg2)
            return;
        var newPlayer = arg1.Spawn(originalPlayer);
        newPlayer.Uuid = arg1.LocalPlayer.RawEncoded;
        newPlayer.TurnSystem = turnSystem;
        turnSystem.AddTurnPlayer_RPC(newPlayer);    
    }

    private void StartBoardGame()
    {
        StartCoroutine(StartBoardGameCo());
    }

    private IEnumerator StartBoardGameCo()
    {
        NetworkRunner runner = GameManager.FusionSession.Runner;
        
        yield return new WaitUntil(()=>turnSystem.Object.IsValid);
        
        if(runner.IsSharedModeMasterClient)
            turnSystem.StartSystem();
    }
}
