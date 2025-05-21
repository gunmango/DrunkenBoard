using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ApartGameManager : MonoBehaviour
{
    public static ApartGameManager Instance;
    public EGameState CurrentState = EGameState.Waiting;
    public ApartPlayerManager HostPlayer;
    public ApartUIManager UIManager;

    public List<ApartPlayerManager> testPlayers;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (testPlayers.Count > 0)
        {
            SetHostPlayer(testPlayers[0]);
        }
    }

    public void SetHostPlayer(ApartPlayerManager player)
    {
        if(HostPlayer != null)
            HostPlayer.isHost = false;
        
        HostPlayer = player;
        HostPlayer.isHost = true;
        
        UIManager.SetHost(true);
    }

    public void StartGame()
    {
        CurrentState = EGameState.Countdown;
        ApartCountdowTimer.Instance.StartCountdown();

    }

    public void OnCountdownFinished()
    {
        CurrentState = EGameState.Input;
        ApartBuilder.Instance.BuildFromInput();
    }

    public void OnInputFinished()
    {
        CurrentState = EGameState.Building;
        ApartBuilder.Instance.BuildFromInput();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && testPlayers.Count > 0)
            testPlayers[0].ArrivedatPlayer();

        if (Input.GetKeyDown(KeyCode.Alpha2) && testPlayers.Count > 1)
            testPlayers[1].ArrivedatPlayer();
    }
}
