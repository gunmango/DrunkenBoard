using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;
// using TMPro;

public class CrocodileGameManager : NetworkBehaviour
{
    [Networked] private int currentTurnIndex{get;set;}
    [Networked] private bool gameStarted {get;set;}

    private List<Player> _fusionplayers;
    public Crocodile Croc;
    public List<Tooth> allTeeth = new List<Tooth>();
   
    [SerializeField] private Button startButton;
    
    [Header("Ui")]
    [SerializeField] public Text currentTurnText;
    [SerializeField] public Text timeText;

    [SerializeField] private float turnDuration = 3f;
    private float _turnTimer = 0f;
    private bool _isTurnActive = false;
    private bool _isSpawned = false;
    
    
    private Dictionary<int, int> playerOrders = new Dictionary<int, int>();

    public override void Spawned()
    {
        base.Spawned();
        Debug.Log($"PlayerManager.Instance is null? {PlayerManager.Instance == null}");
        Debug.Log($"Players count: {(PlayerManager.Instance != null ? PlayerManager.Instance.Players.Count : -1)}");
        StartCoroutine(WaitForPlayerManagerReady());
    }
    
    private IEnumerator WaitForPlayerManagerReady()
    {
        yield return new WaitUntil(() => PlayerManager.Instance != null);

        Debug.Log("✅ PlayerManager 존재함. 플레이어 등록 대기 중...");

        // Player 수가 ActivePlayers 수와 같아질 때까지 대기
        yield return new WaitUntil(() => 
            PlayerManager.Instance.Players.Count >= GameManager.FusionSession.Runner.ActivePlayers.Count());

        Debug.Log($"✅ 모든 플레이어 등록 완료: {PlayerManager.Instance.Players.Count}명");

        // Tooth들 Spawn 완료 대기
        yield return new WaitUntil(() => allTeeth.All(t => t.Object != null && t.Object.IsSpawnable));

        Debug.Log("🦷 모든 이빨 준비 완료 ✅");

        _isSpawned = true;
        RefreshPlayerList();
        gameStarted = false;
        currentTurnIndex = 0;

        InitGame();
    }

    private void InitGame()
    {
      
        if (Object.HasStateAuthority)
        {
            RPC_SetTrap(Random.Range(0, allTeeth.Count));
            RPC_ResetTeeth();
            if (startButton != null)
            {
                startButton.gameObject.SetActive(true);
                startButton.onClick.RemoveAllListeners();
                startButton.onClick.AddListener(StartGame);
            }
            else
            {
                Debug.LogError("startButton is NULL in InitGame()");
            }
        }
        else
        {
            if (startButton != null)
                startButton.gameObject.SetActive(false);
        }
    }
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_SetTrap(int trapIndex)
        {
            if (trapIndex >= 0 && trapIndex < allTeeth.Count)
                allTeeth[trapIndex].Istrap = true;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_ResetTeeth()
        {
            foreach (var tooth in allTeeth)
            {
                tooth.IsPressed = false;
                tooth.ShowTooth();
            }
        }

        private void Awake()
    {
        if (allTeeth == null) allTeeth = new List<Tooth>();
        allTeeth.AddRange(GetComponentsInChildren<Tooth>());
    }
    // private void Start()
    // {
    //     Debug.Log($"PlayerManager.Instance: {PlayerManager.Instance}");
    //     Debug.Log($"startButton: {startButton}");
    //     Debug.Log($"allTeeth Count: {allTeeth.Count}");
    //     Debug.Log($"Croc: {Croc}");
    // }

    private void Update()
    {
        if (!_isSpawned) return; 
        
        if(!gameStarted || !_isTurnActive) return;
        if (!Object.HasStateAuthority) return;
        
        _turnTimer += Time.deltaTime;
        float remainingTime = Mathf.Max(0, turnDuration - _turnTimer);
        if (timeText != null)
            timeText.text = $"{remainingTime:F1}";

        if (_turnTimer >= turnDuration)
        {
            AutoPressTooth();
            _turnTimer = 0f;
        }
    }
    private void AssignPlayerOrder()
    {
        playerOrders.Clear();

        Debug.Log("AssignPlayerOrder 호출, 플레이어 리스트:");
        foreach (var player in _fusionplayers)
        {
            Debug.Log($"플레이어 UUID: {player.Uuid}");
        }

        if (_fusionplayers.Count == 0)
        {
            Debug.LogWarning("AssignPlayerOrder: 플레이어가 없습니다.");
            return;
        }
    
        var firstPlayer = _fusionplayers[0];
        var otherPlayers = _fusionplayers.Skip(1).OrderBy(p => Random.value).ToList();

        _fusionplayers = new List<Player> { firstPlayer };
        _fusionplayers.AddRange(otherPlayers);

        playerOrders.Clear();
        for (int i = 0; i < _fusionplayers.Count; i++)
        {
            Debug.Log($"순서 부여: 플레이어 { _fusionplayers[i].Uuid} => 순서 {i}");
            playerOrders[_fusionplayers[i].Uuid] = i;
        }
    }
    
    private void StartGame()
    {
        if (playerOrders.Count != _fusionplayers.Count)
        {
            AssignPlayerOrder();
        }
        RefreshPlayerList();
        if (!Object.HasStateAuthority) return;
     
        gameStarted = true;
        startButton.gameObject.SetActive(false);
        
        // int trapIndex = Random.Range(0, allTeeth.Count);
        // allTeeth[trapIndex].Istrap = true;
        
        if (_fusionplayers.Count > 0)
        {
            int myUuid = GameManager.FusionSession.Runner.LocalPlayer.RawEncoded;
            int myOrder = GetPlayerOrder(myUuid);
            Debug.Log($"[StartGame] 내 UUID: {myUuid}, 내 순서: {myOrder}");

            StartTurn();
        }
        else
        {
            Debug.LogWarning("플레이어가 아직 없습니다. 플레이어가 들어올 때까지 기다립니다.");
        }
    }

    
    // 플레이어 순서 가져오는 함수
    private int GetPlayerOrder(int playerUuid)
    {
        if (playerOrders.TryGetValue(playerUuid, out int order))
            return order;
        return -1; // 순서 없을 때
    }


    public bool IsCurrentPlayerTurn
    {
        get
        {
            if (!gameStarted) return false;
            if (!_isTurnActive) return false;
            if (IsGameOver) return false;
            if (_fusionplayers.Count == 0) return false;

            int myUuid = GameManager.FusionSession.Runner.LocalPlayer.RawEncoded;

            Debug.Log($"내 UUID: {myUuid}, 현재 턴 순서: {(currentTurnIndex % _fusionplayers.Count)}, 내 순서: {GetPlayerOrder(myUuid)}");

            int currentTurnOrder = (currentTurnIndex % _fusionplayers.Count);
            int myOrder = GetPlayerOrder(myUuid);

            return myOrder == currentTurnOrder;
        }
    }

    public void PressTooth(Tooth tooth, bool force = false)
    {

        int myUuid = GameManager.FusionSession.Runner.LocalPlayer.RawEncoded;
        Debug.Log($"내 UUID: {myUuid}");
        Debug.Log("playerOrders 내용:");
        foreach (var kvp in playerOrders)
        {
            Debug.Log($"UUID: {kvp.Key}, Order: {kvp.Value}");
        }
        
        Debug.Log($"PressTooth 호출: IsCurrentPlayerTurn={IsCurrentPlayerTurn}, tooth.IsPressed={tooth.IsPressed}");
        if (!force && !IsCurrentPlayerTurn) return;
        if (tooth.IsPressed) return;

        _isTurnActive = false;
    
        if (tooth.HasInputAuthority)
        {
            Debug.Log("내가 권한 있음 → RPC 호출");
            tooth.RPC_RequestPressTooth();
        }
        else
        {
            Debug.LogWarning("권한 없음 → RPC 안됨");
        }
        
        if (tooth.Istrap)
        {
            Debug.Log("함정 눌러버렸찌 머야~ ");
            OnTrapTriggered();
            int currentPlayerUuid = _fusionplayers[currentTurnIndex % _fusionplayers.Count].Uuid;
            currentTurnText.text = $"Game Over! {currentPlayerUuid}";
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
            PressTooth(randomTooth, true); // ✅ 강제로 누르기
            Debug.Log($"{randomTooth.name} pressed (Auto)");
        }
    }
    private void NextCycle()
    {
        currentTurnIndex++;

        if (currentTurnIndex < allTeeth.Count)
        {
            StartTurn();
        }
        else
        {
            Debug.Log("게임종료");
            currentTurnText.text = "GameOver";
            OnTrapTriggered();
        }
    }

    private void StartTurn()
    {
        Debug.Log("StartCycle 호출됨");
        
        var playerList = GameManager.FusionSession.Runner.ActivePlayers.ToList();
        
        _isTurnActive = true;
        _turnTimer = 0f;

        int currentPlayerIndex = currentTurnIndex % _fusionplayers.Count;
        UpdateTurnText(currentPlayerIndex);
        Debug.Log($"현재 턴: {_fusionplayers[currentPlayerIndex].Uuid}");
        
        StartTurnForPlayer(_fusionplayers[currentPlayerIndex]);

        // AssignToothInputAuthority();  // ← 여기서 권한 재할당!
        
        if (_fusionplayers.Count == 0)
        {
            Debug.LogWarning("플레이어 리스트 비어있음.");
            return;
        }
    }

    private void UpdateTurnText(int currentTurnIndex)
    {
        if (_fusionplayers.Count == 0)
        {
            currentTurnText.text = "플레이어 없음 or 게임 종료";
            return;
        }

        var player = _fusionplayers[currentTurnIndex];
        currentTurnText.text = $"턴:플레이어{player.Uuid} (번호){GetPlayerOrder(player.Uuid)}";
    }
    
    public Player GetCurrentTurnPlayer()
    {
        if (_fusionplayers == null || _fusionplayers.Count == 0)
            return null;

        return _fusionplayers[currentTurnIndex % _fusionplayers.Count];
    }
    
    public bool IsGameOver => currentTurnIndex >= allTeeth.Count;
    public void OnTrapTriggered()
    {
        Croc.CloseMouth();
        foreach (var tooth in allTeeth)
        {
            tooth.HideTooth();
        }
        _isTurnActive = false;
    }

    private void RefreshPlayerList()
    {
        _fusionplayers = PlayerManager.Instance.Players.ToList();
        Debug.Log($"RefreshPlayerList 호출, 플레이어 수: {_fusionplayers.Count}");
        
        foreach (var tooth in allTeeth)
        {
            tooth.gameManager = this; // 이 부분 꼭 할당해야 함!
        }
    }

    private void AssignToothInputAuthority()
    {
        Player currentPlayer = GetCurrentTurnPlayer();
        if (currentPlayer == null)
        {
            Debug.LogWarning("현재 턴 플레이어가 없습니다.");
            return;
        }

        foreach (var tooth in allTeeth)
        {
            if (tooth.Object.HasStateAuthority)
            {
                tooth.AssignInputAuthorityTo(currentPlayer, GameManager.FusionSession.Runner);
            }
        }
    }
    private void StartTurnForPlayer(Player player)
    {

        foreach (var tooth in allTeeth)
        {
            tooth.AssignInputAuthorityTo(player, GameManager.FusionSession.Runner);
        }

        _turnTimer = 0;
        _isTurnActive = true;
    }
}