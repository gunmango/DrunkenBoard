using System.Collections;
using Fusion;
using UnityEngine;

public class BoardGameProcessor : SimulationBehaviour
{
    [SerializeField] private TurnSystem turnSystem;
    [SerializeField] private BoardGamePlayer originalPlayer;
    [SerializeField] private DiceSetter diceSetter;
    
    private void Start()
    {
        MainGameSceneManager.GameStateManager.ActOnBoard += StartBoardGame;
        
        GameManager.FusionSession.ActOnPlayerJoined += OnPlayerJoined;
        GameManager.FusionSession.ActOnPlayerLeft += OnPlayerLeft;
    }

    private void OnPlayerLeft(NetworkRunner arg1, PlayerRef arg2)
    {
        if (GameManager.FusionSession.Runner.IsSharedModeMasterClient == false)
            return;
        
        turnSystem.RemoveTurnPlayer_RPC(arg2.RawEncoded);
    }

    private void OnPlayerJoined(NetworkRunner arg1, PlayerRef arg2)
    {
        if (arg1.LocalPlayer != arg2)
            return;
        var newPlayer = arg1.Spawn(originalPlayer);
        newPlayer.Initialize(diceSetter, turnSystem, arg1.LocalPlayer.RawEncoded);
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
