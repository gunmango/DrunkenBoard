using System;
using System.Collections;
using Fusion;
using UnityEngine;

public class BoardGameProcessor : SimulationBehaviour
{
    [SerializeField] private TurnSystem turnSystem;
    [SerializeField] private BoardGamePlayer originalPlayer;
    [SerializeField] private DiceSetter diceSetter;
    [SerializeField] private DiceDisplayer diceDisplayer;
    
    private void Start()
    {
        MainGameSceneManager.GameStateManager.ActOnStartGame += StartBoardGame;
        
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
        
        //보드게임 플레이어 생성
        var newPlayer = arg1.Spawn(originalPlayer);
        newPlayer.Initialize(diceSetter, diceDisplayer, turnSystem, arg1.LocalPlayer.RawEncoded);

        //말 생성
        StartCoroutine(SpawnPiece(arg1, arg2, newPlayer));
        
        turnSystem.AddTurnPlayer_RPC(newPlayer);
    }

    private IEnumerator SpawnPiece(NetworkRunner runner, PlayerRef playerRef, BoardGamePlayer newPlayer)
    {
        yield return new WaitUntil(() => PlayerManager.Instance.Object.IsValid);
        yield return new WaitUntil(() => PlayerManager.Instance.IsPlayerValid(playerRef.RawEncoded));
        
        EPlayerColor playerColor = PlayerManager.Instance.GetPlayerColor(playerRef.RawEncoded);
        var newPlayerPiece = runner.Spawn(PlayerManager.Table.GetPiece(playerColor));
        
        newPlayer.SetPiece(newPlayerPiece);
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
