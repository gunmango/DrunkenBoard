using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using System.Linq;

// 즉시 반응 스페이스바 입력 처리 - 정리됨
public class ApartInputManager : NetworkBehaviour
{
    public static ApartInputManager Instance;

    [SerializeField] private GameObject ApartPlayerManagerObj;
    public GameObject InputPanel;
    public InputField NumberInputField;
    public Button CheckeButton;
    
    private ApartPlayerManager apartPlayerManager;
    
    [Networked] public int CurrentInputPlayerIndex { get; set; } = -1;
    
    // 로컬 즉시 반응용 변수들
    private Dictionary<int, int> localSpaceCounts = new Dictionary<int, int>();
    private bool isLocalTurnActive = false;

    public override void Spawned()
    {
        Instance = this;
        InputPanel.SetActive(false);
        CheckeButton.onClick.AddListener(OnNumberInputClicked);
        apartPlayerManager = ApartPlayerManagerObj.GetComponent<ApartPlayerManager>();
    }

    // Update에서 즉시 반응 (딜레이 최소화)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleSpacebarInputInstant();
        }
    }

    // 🔥 중복 생성 방지: 로컬에서는 시각적 효과만, 실제 생성은 네트워크에서만
    private void HandleSpacebarInputInstant()
    {
        // 턴 활성화 체크 (로컬 + 네트워크)
        bool turnActive = isLocalTurnActive;
        if (ApartTurnManager.Instance != null)
        {
            turnActive = ApartTurnManager.Instance.IsTurnActive;
        }

        if (!turnActive)
        {
            Debug.Log("[HandleSpacebarInputInstant] 턴이 비활성화됨");
            return;
        }

        // 🔥 UUID 가져오기
        int uuid = GetCurrentPlayerUuid();
        if (uuid < 0)
        {
            Debug.LogError("[HandleSpacebarInputInstant] UUID 없음");
            return;
        }

        // 🔥 중앙화된 메소드 사용
        int playerIndex = ApartPlayerManager.Instance.GetPlayerIndexByUuid(uuid);
        if (playerIndex < 0)
        {
            Debug.LogError($"[HandleSpacebarInputInstant] 플레이어 인덱스 없음: UUID {uuid}");
            return;
        }

        // 로컬 2번 제한 체크
        if (!localSpaceCounts.ContainsKey(playerIndex))
        {
            localSpaceCounts[playerIndex] = 0;
        }

        if (localSpaceCounts[playerIndex] >= 2)
        {
            Debug.Log($"[HandleSpacebarInputInstant] 플레이어 {playerIndex} 이미 2번 완료");
            return;
        }

        // 🔥 즉시 로컬 카운트만 증가 (시각적 반응용)
        localSpaceCounts[playerIndex]++;
        Debug.Log($"[HandleSpacebarInputInstant] ⚡ 즉시 처리! 플레이어 {playerIndex}: {localSpaceCounts[playerIndex]}/2");

        // 🔥 로컬에서는 층 생성하지 않음! (중복 방지)
        // 대신 시각적 피드백만 제공 (예: 사운드, 이펙트 등)
        if (ApartPlayerManager.Instance != null)
        {
            EPlayerColor playerColor = ApartPlayerManager.Instance.players[playerIndex].PlayerColor;
            Debug.Log($"[HandleSpacebarInputInstant] ⚡ 스페이스바 입력 피드백: {playerColor} (층 생성은 네트워크에서)");
        }

        // 🔥 네트워크 동기화로만 실제 층 생성
        if (ApartTurnManager.Instance != null)
        {
            Debug.Log($"[HandleSpacebarInputInstant] 네트워크로 실제 층 생성 요청: UUID {uuid}");
            ApartTurnManager.Instance.RPC_OnSpacePressed(uuid);
        }
    }

    // 🔥 현재 플레이어 UUID 가져오기
    private int GetCurrentPlayerUuid()
    {
        Debug.Log($"[GetCurrentPlayerUuid] 시작");
        
        // 방법 1: PlayerManager에서 로컬 플레이어의 UUID 찾기
        if (PlayerManager.Instance != null)
        {
            foreach (var player in PlayerManager.Instance.Players)
            {
                if (player.Object != null && player.Object.HasInputAuthority)
                {
                    int uuid = player.Uuid;
                    Debug.Log($"[GetCurrentPlayerUuid] ✅ PlayerManager에서 로컬 플레이어 UUID: {uuid}");
                    return uuid;
                }
            }
            Debug.Log("[GetCurrentPlayerUuid] PlayerManager에서 InputAuthority 가진 플레이어 못찾음");
        }

        // 방법 2: Runner.LocalPlayer에서 UUID 가져오기
        if (Runner.LocalPlayer != null)
        {
            int playerId = Runner.LocalPlayer.PlayerId;
            Debug.Log($"[GetCurrentPlayerUuid] Runner.LocalPlayer.PlayerId: {playerId}");
            
            // PlayerManager에서 해당 PlayerId의 실제 UUID 확인
            if (PlayerManager.Instance != null)
            {
                foreach (var player in PlayerManager.Instance.Players)
                {
                    if (player.Object != null && player.Object.InputAuthority == Runner.LocalPlayer)
                    {
                        int uuid = player.Uuid;
                        Debug.Log($"[GetCurrentPlayerUuid] ✅ PlayerId {playerId}의 실제 UUID: {uuid}");
                        return uuid;
                    }
                }
            }
            
            // 못찾으면 PlayerId를 UUID로 사용
            Debug.LogWarning($"[GetCurrentPlayerUuid] ⚠️ PlayerId를 UUID로 사용: {playerId}");
            return playerId;
        }

        // 방법 3: InputAuthority에서 직접 가져오기
        if (Object.HasInputAuthority && Object.InputAuthority != null)
        {
            int playerId = Object.InputAuthority.PlayerId;
            Debug.Log($"[GetCurrentPlayerUuid] Object.InputAuthority.PlayerId: {playerId}");
            
            // 이것도 UUID일 가능성 체크
            if (PlayerManager.Instance != null)
            {
                foreach (var player in PlayerManager.Instance.Players)
                {
                    if (player.Uuid == playerId)
                    {
                        Debug.Log($"[GetCurrentPlayerUuid] ✅ InputAuthority PlayerId가 UUID와 일치: {playerId}");
                        return playerId;
                    }
                }
            }
            
            return playerId;
        }

        // 방법 4: 마스터 클라이언트면 첫 번째 플레이어 UUID
        if (Runner.IsSharedModeMasterClient && PlayerManager.Instance != null)
        {
            var firstPlayer = PlayerManager.Instance.Players.FirstOrDefault();
            if (firstPlayer != null)
            {
                int uuid = firstPlayer.Uuid;
                Debug.LogWarning($"[GetCurrentPlayerUuid] ⚠️ 마스터 클라이언트 첫 번째 플레이어 UUID 사용: {uuid}");
                return uuid;
            }
        }

        Debug.LogError("[GetCurrentPlayerUuid] ❌ UUID를 찾을 수 없음!");
        return -1;
    }

    // 턴 시작 알림 (TurnManager에서 호출)
    public void OnTurnStarted()
    {
        Debug.Log("[OnTurnStarted] 로컬 턴 시작!");
        isLocalTurnActive = true;
        localSpaceCounts.Clear();
    }

    // 턴 종료 알림 (TurnManager에서 호출)
    public void OnTurnEnded()
    {
        Debug.Log("[OnTurnEnded] 로컬 턴 종료!");
        isLocalTurnActive = false;
        localSpaceCounts.Clear();
    }

    // FixedUpdateNetwork는 엔터키만 처리
    public override void FixedUpdateNetwork()
    {
        // 엔터키로 숫자 입력 확인
        if (Input.GetKeyDown(KeyCode.Return) && InputPanel.activeInHierarchy)
        {
            OnNumberInputClicked();
        }
    }

    // 게임 시작 RPC
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_OnGameStarted(int starterPlayerIndex)
    {
        if (!Runner.IsSharedModeMasterClient) return;
        
        CurrentInputPlayerIndex = starterPlayerIndex;
        
        if (ApartTurnManager.Instance != null)
        {
            Debug.Log("[RPC_OnGameStarted] TurnManager로 게임 시작");
            ApartTurnManager.Instance.StartTurn();
        }
    }
    

    // 숫자 입력 버튼 클릭 처리
    private void OnNumberInputClicked()
    {
        //if (!Runner.IsSharedModeMasterClient) return;

        if (int.TryParse(NumberInputField.text, out int targetFloor))
        {
            RPC_OnNumberInput(targetFloor);
        }
    }
    
    // 🔧 수정: 현재 턴 플레이어에게만 숫자 입력 UI 표시
    public void ShowNumberInputUIForCurrentPlayer()
    {
        Debug.Log("[ShowNumberInputUIForCurrentPlayer] 🎯 현재 턴 플레이어 UI 표시 시작");
    
        if (ApartPlayerManager.Instance == null)
        {
            Debug.LogError("[ShowNumberInputUIForCurrentPlayer] ❌ ApartPlayerManager가 없음!");
            return;
        }
        
        int currentPlayerUuid = MainGameSceneManager.SpaceEventManager.CurrentSpaceEvent.EnteredPlayerUuid;
        
        if (currentPlayerUuid < 0)
        {
            Debug.LogError("[ShowNumberInputUIForCurrentPlayer] ❌ 현재 플레이어 UUID를 찾을 수 없음!");
            return;
        }
    
        // 🔥 RPC로 해당 UUID 플레이어에게만 UI 표시
        RPC_ShowNumberInputToSpecificPlayer(currentPlayerUuid);
    }
    // 🔧 추가: 특정 플레이어에게만 숫자 입력 UI 표시
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ShowNumberInputToSpecificPlayer(int targetPlayerUuid)
    {
        Debug.Log($"[RPC_ShowNumberInputToSpecificPlayer] 대상 UUID: {targetPlayerUuid}");
    
        // 🔥 현재 클라이언트의 UUID 확인
        int myUuid = GetCurrentPlayerUuid();
    
        Debug.Log($"[RPC_ShowNumberInputToSpecificPlayer] 내 UUID: {myUuid}, 대상 UUID: {targetPlayerUuid}");
    
        // 🔥 대상 플레이어인지 확인
        bool shouldShowUI = (myUuid == targetPlayerUuid);
    
        if (shouldShowUI)
        {
            Debug.Log($"[RPC_ShowNumberInputToSpecificPlayer] ✅ UUID {myUuid} 플레이어에게 UI 표시");
            NumberInputField.text = "";
            InputPanel.SetActive(true);
        }
        else
        {
            Debug.Log($"[RPC_ShowNumberInputToSpecificPlayer] ❌ UUID {myUuid} 플레이어 UI 숨김");
            InputPanel.SetActive(false);
        }
    }
    // 🔧 추가: ApartPlayer로부터 UUID 추출
    private int GetPlayerUuidFromApartPlayer(ApartPlayer apartPlayer)
    {
        if (apartPlayer == null) return -1;
    
        // 방법 1: playerName에서 UUID 추출 ("Player_123" → 123)
        string playerName = apartPlayer.playerName;
        if (!string.IsNullOrEmpty(playerName) && playerName.StartsWith("Player_"))
        {
            string uuidStr = playerName.Substring(7); // "Player_" 제거
            if (int.TryParse(uuidStr, out int uuid))
            {
                Debug.Log($"[GetPlayerUuidFromApartPlayer] ✅ playerName에서 UUID 추출: {playerName} → {uuid}");
                return uuid;
            }
        }
    
        // 방법 2: PlayerManager에서 색상으로 UUID 찾기
        if (PlayerManager.Instance != null)
        {
            foreach (var fusionPlayer in PlayerManager.Instance.Players)
            {
                if (fusionPlayer.PlayerColor == apartPlayer.PlayerColor)
                {
                    Debug.Log($"[GetPlayerUuidFromApartPlayer] ✅ 색상 매칭으로 UUID 발견: {apartPlayer.PlayerColor} → {fusionPlayer.Uuid}");
                    return fusionPlayer.Uuid;
                }
            }
        }
    
        Debug.LogError($"[GetPlayerUuidFromApartPlayer] ❌ UUID를 찾을 수 없음: {playerName}");
        return -1;
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_OnNumberInput(int targetFloor)
    {
        if (!Runner.IsSharedModeMasterClient) return;
        
        RPC_HideInputPanel();
        
        int currentFloorCount = ApartGameManager.Instance.GetCurrentFloorCount();
        
        if (targetFloor > currentFloorCount)
        {
            ApartGameManager.Instance.RaiseFloorsToTarget(targetFloor, () =>
            {
                ApartGameManager.Instance.HighlightFloor(targetFloor);
            });
        }
        else
        {
            ApartGameManager.Instance.HighlightFloor(targetFloor);
        }
        
        if (apartPlayerManager != null)
        {
            apartPlayerManager.NextPlayer();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_HideInputPanel()
    {
        InputPanel.SetActive(false);
    }

    // 🔧 수정: 기존 메서드명 변경
    public void ShowNumberInputUIForMasterClient()
    {
        // 🔥 이제 현재 턴 플레이어에게 표시
        ShowNumberInputUIForCurrentPlayer();
    }
    // 🔧 추가: InputManager 초기화
    public void ResetManager()
    {
        Debug.Log("[ApartInputManager.ResetManager] 🔄 InputManager 초기화");
    
        // 네트워크 변수 초기화 (StateAuthority에서만)
        if (Object.HasStateAuthority)
        {
            CurrentInputPlayerIndex = -1;
        }
    
        // 로컬 변수 초기화
        localSpaceCounts.Clear();
        isLocalTurnActive = false;
    
        // UI 정리
        if (InputPanel != null)
        {
            InputPanel.SetActive(false);
        }
    
        if (NumberInputField != null)
        {
            NumberInputField.text = "";
        }
    
        Debug.Log("[ApartInputManager.ResetManager] ✅ InputManager 초기화 완료");
    }

}