using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CrocodileGameManager : NetworkBehaviour
{
    [Header("씬에 배치된 이빨들을 Inspector에서 수동 등록")]
    [SerializeField] private CrocodileTooth[] allTeeth;

    [SerializeField] private TurnSystem turnSystemInScene;
    [SerializeField] private CrocodilePlayer playerPrefab;

    // 플레이어별 TurnTimer를 씬에서 미리 할당(예: 플레이어수만큼 배열이나 리스트)
    [SerializeField] private NetworkTimer timer;

    private TurnSystem turnSystem;
    private List<CrocodilePlayer> spawnedPlayers = new List<CrocodilePlayer>();
    
    [Networked] private bool gameStarted { get; set; }
    [Networked] private int trapToothIndex { get; set; } = -1; // 🧠 트랩 이빨 인덱스를 네트워크로 공유
    
    [Networked] public bool GameEnded { get; private set; } = false;
    
    public Action OnGameEnded {get; set;}
    
    private int _playingPlayerCount = 0;
    
    private void Awake()
    {
        turnSystem = turnSystemInScene;
        
        // 이빨들 초기화
        for (int i = 0; i < allTeeth.Length; i++)
        {
            allTeeth[i].toothIndex = i;
        }
    }

    private void Start()
    {
        GameManager.FusionSession.ActOnPlayerLeft += OnPlayerLeft;
    }

    private IEnumerator DelayedInitializeGame()
    {
        // 씬 로딩이 완전히 끝날 때까지 대기
        yield return new WaitForSeconds(0.5f);
        
        // TurnSystem이 NetworkObject로 제대로 초기화될 때까지 대기
        yield return new WaitUntil(() => turnSystem != null && turnSystem.Object != null && turnSystem.Object.IsValid);
        
        InitializeGame();
    }

    private void InitializeGame()
    {
        //Debug.Log("게임 초기화 시작");

        _playingPlayerCount = PlayerManager.Instance.Players.Count;
        
        // 🧠 트랩 이빨 지정은 오직 여기서!
        trapToothIndex = UnityEngine.Random.Range(0, allTeeth.Length);
        //Debug.Log($"🎯 트랩 이빨 지정: {trapToothIndex}");

        for (int i = 0; i < allTeeth.Length; i++)
        {
            allTeeth[i].toothIndex = i;

            // 🧠 모든 클라이언트에 트랩 여부 설정 동기화
            bool isTrap = (i == trapToothIndex);
            allTeeth[i].RPC_SetTrap(isTrap);
        }
    }

    public void SetPlayerAndStart()
    {
        // StateAuthority에서만 게임 초기화
        if (Object.HasStateAuthority)
        {
            // 잠시 대기 후 초기화 (씬 로딩 완료 대기)
            InitializeGame();
        }
        StartCoroutine(InitializePlayerSafe(Runner, Runner.LocalPlayer));
    }

    private void OnPlayerLeft(NetworkRunner runner, PlayerRef playerRef)
    {
        //Debug.Log($"OnPlayerLeft 호출됨: {playerRef.RawEncoded}");
        
        int uuid = playerRef.RawEncoded;
        
        // 스폰된 플레이어 리스트에서 제거
        for (int i = spawnedPlayers.Count - 1; i >= 0; i--)
        {
            if (spawnedPlayers[i] != null && spawnedPlayers[i].Uuid == uuid)
            {
                spawnedPlayers.RemoveAt(i);
                break;
            }
        }
        
        // 턴 시스템에서 플레이어 제거
        if (turnSystem != null)
        {
            turnSystem.RemoveTurnPlayer_RPC(uuid);
        }
    }

    private IEnumerator InitializePlayerSafe(NetworkRunner runner, PlayerRef playerRef)
    {
        int uuid = playerRef.RawEncoded;
        //Debug.Log($"InitializePlayerSafe 시작 for player {uuid}");

        // PlayerManager 준비 대기
        yield return new WaitUntil(() => PlayerManager.Instance != null);
        yield return new WaitUntil(() => PlayerManager.Instance.Object.IsValid);
        yield return new WaitUntil(() => PlayerManager.Instance.IsPlayerValid(uuid));
        //Debug.Log($"PlayerManager 확인 완료 for {uuid}");

        // 플레이어 스폰
        var newPlayer = runner.Spawn(playerPrefab);
        if (newPlayer == null)
        {
             Debug.LogError("Player prefab 스폰 실패!");
            yield break;
        }
        
        //Debug.Log("Player prefab Spawn 완료");
        spawnedPlayers.Add(newPlayer);

        // 플레이어 초기화 - 네트워크 객체가 완전히 준비될 때까지 대기
        yield return new WaitUntil(() => newPlayer.Object.IsValid);
        
        newPlayer.Initialize(turnSystem, uuid, timer, allTeeth, this);
        //Debug.Log("newPlayer.Initialize 완료");

        // 턴 시스템에 플레이어 추가
        if (turnSystem != null)
        {
            turnSystem.AddTurnPlayer_RPC(newPlayer);
            //Debug.Log("turnSystem.AddTurnPlayer_RPC 호출 완료");
        }

        // 게임 시작 조건 확인
        StartCoroutine(WaitForPlayerSpawn());
    }

    private IEnumerator WaitForPlayerSpawn()
    {
        if (Object.HasStateAuthority == false)
            yield break;
        yield return new WaitUntil(()=>turnSystem.TurnPlayers.Count == _playingPlayerCount);
        StartGame();
    }
    
    private void StartGame()
    {
        if (gameStarted) return;
        
        gameStarted = true;
        //Debug.Log("게임 시작!");
        
        turnSystem.StartSystem();
        
        // 모든 클라이언트에게 게임 시작 알림
        RPC_OnGameStarted();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_OnGameStarted()
    {
        //Debug.Log("게임이 시작되었습니다!");
        
        // 여기서 게임 시작 UI 표시 등 추가 로직 수행
        // 예: 모든 이빨 활성화, 게임 시작 사운드 재생 등
        
        foreach (var tooth in allTeeth)
        {
            tooth.gameObject.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        if (GameManager.FusionSession != null)
        {
            GameManager.FusionSession.ActOnPlayerLeft -= OnPlayerLeft;
        }
    }
    
    public void EndGame(int deadPlayerId)
    {
        //Debug.Log("🔥 게임 종료됨!");
        GameEnded = true; // ✅ 게임 종료 상태 저장
        
        // 모든 이빨에게 게임 종료 알림
        foreach (var tooth in allTeeth)
        {
            if (tooth != null)
            {
                tooth.EndGame(); // 🔧 새로 추가된 RPC 호출
            }
        }

        timer.gameObject.SetActive(false);
        // 게임 상태를 종료로 변경
        gameStarted = false;
        
        //끝
        if (Object.HasStateAuthority)
        {
            GameEnded = false;
            turnSystem.EndSystem();
        }

        OnGameEnded?.Invoke();

        ToDrinkTime(deadPlayerId);
    }

    private void ToDrinkTime(int drinkerId)
    {
        List<int> drinker = new List<int>();
        drinker.Add(drinkerId);
        MainGameSceneManager.SpaceEventManager.CurrentSpaceEvent.EndEvent(drinker);
    }
}