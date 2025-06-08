using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fusion;

// 포톤 퓨전2 쉐어드 모드용 ApartPlayerManager - 중앙화된 플레이어 관리
public class ApartPlayerManager : NetworkBehaviour
{
    public static ApartPlayerManager Instance;
    public List<ApartPlayer> players = new List<ApartPlayer>();
    
    [Networked] public int CurrentPlayerIndex { get; set; } = 0;
    
    [SerializeField] private PlayerPrefabTable playerPrefabTable;

    public override void Spawned()
    {
        Instance = this;
        InitializePlayersFromFusion();
    }

    // 플레이어 초기화 (강화된 로직)
    private void InitializePlayersFromFusion()
    {
        if (PlayerManager.Instance == null)
        {
            Debug.LogWarning("[ApartPlayerManager] PlayerManager.Instance가 null, 1초 후 재시도");
            Invoke(nameof(InitializePlayersFromFusion), 1f);
            return;
        }

        Debug.Log($"[ApartPlayerManager] 초기화 시작 - PlayerManager 플레이어 수: {PlayerManager.Instance.Players.Count()}");

        players.Clear();

        foreach (var fusionPlayer in PlayerManager.Instance.Players)
        {
            Debug.Log($"[ApartPlayerManager] 처리중인 Fusion Player: UUID={fusionPlayer?.Uuid}, Color={fusionPlayer?.PlayerColor}, IsValid={fusionPlayer?.Object?.IsValid}");

            if (fusionPlayer?.Object != null && fusionPlayer.Object.IsValid)
            {
                AddPlayerLocal(fusionPlayer);
            }
            else
            {
                Debug.LogWarning($"[ApartPlayerManager] 유효하지 않은 Fusion Player: UUID={fusionPlayer?.Uuid}");
                
                // 🔥 유효하지 않아도 UUID와 색상이 있으면 생성 시도
                if (fusionPlayer != null && fusionPlayer.Uuid >= 0)
                {
                    Debug.Log($"[ApartPlayerManager] 강제로 ApartPlayer 생성 시도: UUID={fusionPlayer.Uuid}");
                    AddPlayerLocal(fusionPlayer);
                }
            }
        }
        
        Debug.Log($"[ApartPlayerManager] 초기화 완료 - 총 {players.Count}명의 플레이어 생성됨");
        
        // 🔥 디버그: 생성된 플레이어들 출력
        for (int i = 0; i < players.Count; i++)
        {
            Debug.Log($"[ApartPlayerManager] 생성된 플레이어[{i}]: Name={players[i].playerName}, Color={players[i].PlayerColor}");
        }
        
        // 플레이어가 없으면 재시도
        if (players.Count == 0)
        {
            Debug.LogWarning("[ApartPlayerManager] 플레이어가 없습니다. 5초 후 재시도...");
            Invoke(nameof(InitializePlayersFromFusion), 5f);
        }
    }

    // 로컬 플레이어 추가 (최적화)
    private void AddPlayerLocal(Player fusionPlayer)
    {
        string playerName = $"Player_{fusionPlayer.Uuid}";
        
        if (players.Any(p => p.playerName == playerName)) return;

        try
        {
            GameObject apartPlayerObj = new GameObject($"ApartPlayer_{fusionPlayer.Uuid}");
            apartPlayerObj.transform.SetParent(transform);

            ApartPlayer apartPlayer = apartPlayerObj.AddComponent<ApartPlayer>();
            apartPlayer.playerName = playerName;
            apartPlayer.pressCount = 0;
            apartPlayer.PlayerColor = fusionPlayer.PlayerColor;
            apartPlayer.currentPlayerIndex = players.Count;

            players.Add(apartPlayer);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"AddPlayerLocal 에러: {e.Message}");
        }
    }

    // 외부에서 플레이어 추가
    public void AddPlayerDirect(Player fusionPlayer)
    {
        if (fusionPlayer == null) return;
        AddPlayerLocal(fusionPlayer);
    }

    public void OnFusionPlayerAdded(Player fusionPlayer)
    {
        if (fusionPlayer == null) return;
        AddPlayerDirect(fusionPlayer);
    }

    // 플레이어 제거
    public void OnFusionPlayerLeft(int uuid)
    {
        ApartPlayer playerToRemove = players.FirstOrDefault(p => p.playerName == $"Player_{uuid}");
        if (playerToRemove != null)
        {
            players.Remove(playerToRemove);
            Destroy(playerToRemove.gameObject);

            if (Runner.IsSharedModeMasterClient)
            {
                if (CurrentPlayerIndex >= players.Count && players.Count > 0)
                {
                    CurrentPlayerIndex = 0;
                }
            }
        }
    }

    // 🔥 중앙화된 UUID -> 플레이어 인덱스 찾기 (통합된 버전)
    public int GetPlayerIndexByUuid(int uuid)
    {
        Debug.Log($"[GetPlayerIndexByUuid] 🔍 찾는 UUID: {uuid}");
        
        if (players.Count == 0)
        {
            Debug.LogError("[GetPlayerIndexByUuid] 플레이어 목록이 비어있음!");
            return -1;
        }
        
        Debug.Log($"[GetPlayerIndexByUuid] 등록된 플레이어 수: {players.Count}");
        
        // 🔥 방법 1: PlayerManager를 통한 색상 매칭
        if (PlayerManager.Instance != null)
        {
            EPlayerColor playerColor = PlayerManager.Instance.GetPlayerColor(uuid);
            Debug.Log($"[GetPlayerIndexByUuid] PlayerManager.GetPlayerColor({uuid}) 결과: {playerColor}");
            
            if (playerColor != EPlayerColor.None)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].PlayerColor == playerColor)
                    {
                        Debug.Log($"[GetPlayerIndexByUuid] ✅ 색상 매칭 성공: UUID {uuid} -> Color {playerColor} -> Index {i}");
                        return i;
                    }
                }
                
                // 🔥 색상 매칭 실패시 플레이어 생성
                Debug.LogWarning($"[GetPlayerIndexByUuid] ⚠️ 색상 {playerColor} 플레이어 없음! 즉시 생성 시도");
                var fusionPlayer = PlayerManager.Instance.Players.FirstOrDefault(p => p.Uuid == uuid);
                if (fusionPlayer != null)
                {
                    AddPlayerLocal(fusionPlayer);
                    
                    // 생성 직후 다시 찾기
                    for (int i = 0; i < players.Count; i++)
                    {
                        if (players[i].PlayerColor == playerColor)
                        {
                            Debug.Log($"[GetPlayerIndexByUuid] ✅ 강제 생성 후 매칭 성공: UUID {uuid} -> Index {i}");
                            return i;
                        }
                    }
                }
            }
        }

        // 🔥 방법 2: 이름 매칭
        string targetPlayerName = $"Player_{uuid}";
        Debug.Log($"[GetPlayerIndexByUuid] 이름 '{targetPlayerName}'로 찾는 중...");
        
        for (int i = 0; i < players.Count; i++)
        {
            string playerName = players[i].playerName;
            if (playerName == targetPlayerName)
            {
                Debug.Log($"[GetPlayerIndexByUuid] ✅ 이름 매칭 성공: UUID {uuid} -> Index {i}");
                return i;
            }
        }

        // 🔥 방법 3: 강제 플레이어 생성 시도
        Debug.LogWarning($"[GetPlayerIndexByUuid] ⚠️ UUID {uuid} 플레이어가 없음, 강제 생성 시도");
        if (PlayerManager.Instance != null)
        {
            var fusionPlayer = PlayerManager.Instance.Players.FirstOrDefault(p => p.Uuid == uuid);
            if (fusionPlayer != null)
            {
                Debug.Log($"[GetPlayerIndexByUuid] 🔧 PlayerManager에서 UUID {uuid} 발견, ApartPlayer 강제 생성");
                AddPlayerLocal(fusionPlayer);
                
                // 생성 후 이름으로 재시도
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].playerName == targetPlayerName)
                    {
                        Debug.Log($"[GetPlayerIndexByUuid] ✅ 강제 생성 후 이름 매칭 성공: UUID {uuid} -> Index {i}");
                        return i;
                    }
                }
            }
        }

        Debug.LogError($"[GetPlayerIndexByUuid] ❌ 모든 방법 실패! UUID {uuid}를 찾을 수 없음!");
        return -1;
    }

    // 현재 플레이어 가져오기
    public ApartPlayer GetCurrentPlayer() 
    {
        if (players.Count == 0) return null;
        
        if (CurrentPlayerIndex < 0 || CurrentPlayerIndex >= players.Count)
        {
            if (Runner.IsSharedModeMasterClient)
            {
                CurrentPlayerIndex = 0;
            }
        }
        
        return players[CurrentPlayerIndex];
    }

    // 게임 시작
    public void StartGame()
    {
        if (Runner.IsSharedModeMasterClient)
        {
            RPC_StartGame();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_StartGame()
    {
        if (ApartTurnManager.Instance != null)
        {
            ApartTurnManager.Instance.StartTurn();
        }
    }

    // 다음 플레이어로 턴 변경
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_NextPlayer()
    {
        if (!Runner.IsSharedModeMasterClient) return;
    
        if (players.Count == 0) return;

        CurrentPlayerIndex = (CurrentPlayerIndex + 1) % players.Count;
    
        if (ApartTurnManager.Instance != null)
        {
            ApartTurnManager.Instance.ResetTurnSystem();
        }
    }

    public void NextPlayer()
    {
        if (Runner.IsSharedModeMasterClient)
        {
            RPC_NextPlayer();
        }
    }

    public void RefreshPlayersFromFusion()
    {
        InitializePlayersFromFusion();
    }

    public PlayerColorSet GetPlayerColorSet(EPlayerColor playerColor)
    {
        if (playerPrefabTable == null) return null;
        return playerPrefabTable.GetColorSet(playerColor);
    }
    
// 🔧 추가: PlayerManager 초기화
    public void ResetManager()
    {
        Debug.Log("[ApartPlayerManager.ResetManager] 🔄 PlayerManager 초기화");
    
        // 네트워크 변수 초기화 (StateAuthority에서만)
        if (Object.HasStateAuthority)
        {
            CurrentPlayerIndex = 0;
        }
    
        // 플레이어 상태 초기화 (players 리스트는 유지)
        foreach (var player in players)
        {
            if (player != null)
            {
                player.pressCount = 0;
                // 다른 플레이어별 상태 초기화가 필요하면 여기 추가
            }
        }
    
        Debug.Log("[ApartPlayerManager.ResetManager] ✅ PlayerManager 초기화 완료");
    }
}