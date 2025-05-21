using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;
// using TMPro;

public class CrocodileGameManager : MonoBehaviour
{
    public Crocodile Croc;
    public List<Tooth> allTeeth;
    // public Tooth tooth;
    //
    [SerializeField] private Button startButton;
    private bool gameStarted = false;
    
    [Header("players")]
    [SerializeField] public List<CrocodilePlayer> players = new List<CrocodilePlayer>();
    
    [Header("Ui")]
    [SerializeField] public Text currentTurnText;
    [SerializeField] public Text timeText;

    // public TextMeshProUGUI currentTurnText;
    [SerializeField] private float turnDuration = 3f;
    private int _currentTurnIndex = 0;
    private float _turnTimer = 0f;
    private bool _isTurnActive = false;

    private void Awake()
    {
        allTeeth.AddRange(GetComponentsInChildren<Tooth>());
    }
    private void Start()
    {
        //테스트용 유저 배치
        players = new List<CrocodilePlayer>();
        {
        players.Add(new CrocodilePlayer("삐약이,첫도착자"));
        players.Add(new CrocodilePlayer("데굴이"));
        players.Add(new CrocodilePlayer("삐죽이"));
        }
        
        AssignPlayerOrder();
        
        //시작버튼 관련 설정 아직 네트워크 전이라 일단아래쪽에 트루로 해놈

        if (IsLocalPlayerOne())
        {
            startButton.gameObject.SetActive(true);
            startButton.onClick.AddListener(StartGame);
        }

        else
        {
            startButton.gameObject.SetActive(false);
        }

        foreach (var tooth in allTeeth)
        {
            tooth.Croc = Croc;
            tooth.gameManager = this;
        }
        int trapIndex = Random.Range(0, allTeeth.Count);
        allTeeth[trapIndex].Istrap = true;

        StartCycle();
    }

    private bool IsLocalPlayerOne()
    {
        return true;
    }

    private void StartGame()
    {
        gameStarted = true;
        startButton.gameObject.SetActive(false);
        
        int trapIndex = Random.Range(0, allTeeth.Count);
        allTeeth[trapIndex].Istrap = true;
        
        StartCycle();
    }

    private void AssignPlayerOrder()
    {
        if (players.Count == 0)
        {
            Debug.Log("No players assigned to Crocodile");
            return;
        }
        
        var fristPlayer = players[0];
        var otherPlayer = players.Skip(1).OrderBy(p=>Random.value).ToList();

        players = new List<CrocodilePlayer> { fristPlayer };
        players.AddRange(otherPlayer);

        for (int i = 0; i < players.Count; i++)
        {
            players[i].PlayerOrder = i + 1;
        }
    }

    private void Update()
    {
        if(!gameStarted||!_isTurnActive) return;
        
        _turnTimer += Time.deltaTime;
        float remainingTime = Mathf.Max(0,turnDuration-_turnTimer);
        timeText.text = $"{remainingTime:F1}";
        if (_turnTimer >= turnDuration)
        {
            AutoPressTooth();
            _turnTimer = 0f;
        }
    }

    public bool IsCurrentPlayerTurn
    {
        get
        {
            if(!gameStarted) return false;
            if(!_isTurnActive) return false;
            if (IsGameOver) return false;
            if(players.Count == 0) return false;
            
            int currentPlayerIndex = _currentTurnIndex%players.Count;
            return !players[currentPlayerIndex].TurnPlayer;
        }
    }

    public void PressTooth(Tooth tooth)
    {
        if(!IsCurrentPlayerTurn|| tooth.IsPressed) return;
        
        _isTurnActive = false;
        
        int currentPlayerIndex = _currentTurnIndex % players.Count;
        players[currentPlayerIndex].TurnPlayer = true;

        tooth.ForcePress();
        if (tooth.Istrap)
        {
            Debug.Log("함정 눌러버렸찌 머야~ ");
            OnTrapTriggered();
            currentTurnText.text = $"Game Over! {players[_currentTurnIndex % players.Count].PlayerName}";
            return;
        }
        
        NextCycle();
    }


    private void AutoPressTooth()
    {
        var available = allTeeth.Where(t => !t.IsPressed).ToList();
        if (available.Count > 0)
        {
            var randomTooth = available[Random.Range(0, available.Count)];
            PressTooth(randomTooth);
            Debug.Log($"{randomTooth.name} pressed");
        }
    }
    private void NextCycle()
    {
        _currentTurnIndex++;
        
        if (_currentTurnIndex < allTeeth.Count)
        {
            StartCycle();
        }
        else
        {
            Debug.Log("게임종료");
            currentTurnText.text = "GameOver";
            OnTrapTriggered();
        }
    }

    private void StartCycle()
    {
        if (players.Count == 0)
        {
            currentTurnText.text = "Game Over";
            _isTurnActive = false;
            return;
        }

        foreach (var player in players)
        {
            player.TurnPlayer = false;
        }
        _isTurnActive = true;
        _turnTimer = 0f;

        int currentPlayerIdex= _currentTurnIndex % players.Count;
        UpdateTurnText(currentPlayerIdex);
        Debug.Log($"현재 턴:{players[currentPlayerIdex].PlayerName},{players[currentPlayerIdex].PlayerOrder}번");
    }

    private void UpdateTurnText(int currentTurnIndex)
    {
        if (players.Count == 0)
        {
            currentTurnText.text = "플레이어 없음 or 게임 종료";
            return;
        }
        currentTurnText.text = $"턴: {players[currentTurnIndex].PlayerName} (번호 {players[currentTurnIndex].PlayerOrder})";
    }
    
    public bool IsGameOver => _currentTurnIndex >= allTeeth.Count;
    public void OnTrapTriggered()
    {
        Croc.CloseMouth();
        foreach (var tooth in allTeeth)
        {
            tooth.HideTooth();
        }
        _isTurnActive = false;
    }
    
}