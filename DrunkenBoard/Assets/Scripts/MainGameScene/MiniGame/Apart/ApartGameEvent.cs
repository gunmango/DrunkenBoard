using UnityEngine;
using System.Collections;
using Fusion;

// 포톤 퓨전2 쉐어드 모드용 ApartGameEvent - 네트워크 동기화
public class ApartGameEvent : ASpaceEvent
{
    [SerializeField] private ApartPlayerManager apartPlayerManager;
    
    protected override void PlayEvent()
    {
        Debug.Log("[ApartGameEvent] PlayEvent 시작");
        
        // 아파트 게임 시작 처리
        StartCoroutine(StartApartGame());
    }
    
    private IEnumerator StartApartGame()
    {
        // 🔥 ApartPlayerManager에서 NetworkRunner 가져오기 (FindObjectOfType 금지!)
        NetworkRunner runner = null;
        if (apartPlayerManager != null && apartPlayerManager.Runner != null)
        {
            runner = apartPlayerManager.Runner;
        }
        else if (ApartPlayerManager.Instance != null && ApartPlayerManager.Instance.Runner != null)
        {
            runner = ApartPlayerManager.Instance.Runner;
        }
        
        if (runner == null)
        {
            Debug.LogError("[ApartGameEvent] NetworkRunner를 ApartPlayerManager에서 가져올 수 없음!");
            yield break;
        }
        
        Debug.Log($"[ApartGameEvent] NetworkRunner 발견 - IsSharedModeMasterClient: {runner.IsSharedModeMasterClient}");
        
        // PlayerManager가 준비될 때까지 대기
        while (PlayerManager.Instance == null)
        {
            Debug.LogWarning("[ApartGameEvent] PlayerManager.Instance 초기화 대기 중...");
            yield return new WaitForSeconds(0.5f);
        }
        
        Debug.Log("[ApartGameEvent] PlayerManager 발견");
        
        // PlayerManager에 플레이어들이 있을 때까지 대기
        while (PlayerManager.Instance.Players.Count == 0)
        {
            Debug.Log("[ApartGameEvent] PlayerManager에서 플레이어 등록 대기 중...");
            yield return new WaitForSeconds(1f);
        }
        
        Debug.Log($"[ApartGameEvent] PlayerManager에 플레이어 {PlayerManager.Instance.Players.Count}명 확인됨");
        
        // 🔥 PlayerManager 플레이어 정보 출력
        foreach (var player in PlayerManager.Instance.Players)
        {
            Debug.Log($"[ApartGameEvent] PlayerManager 플레이어: UUID={player.Uuid}, Color={player.PlayerColor}, IsValid={player.Object?.IsValid}");
        }
        
        // ApartPlayerManager가 없으면 싱글톤으로 접근
        if (apartPlayerManager == null)
        {
            apartPlayerManager = ApartPlayerManager.Instance;
        }
        
        // ApartPlayerManager가 준비될 때까지 대기
        while (apartPlayerManager == null)
        {
            Debug.LogWarning("[ApartGameEvent] ApartPlayerManager.Instance 초기화 대기 중...");
            apartPlayerManager = ApartPlayerManager.Instance;
            yield return new WaitForSeconds(0.5f);
        }
        
        Debug.Log("[ApartGameEvent] ApartPlayerManager 발견");
        
        // 🔥 마스터 클라이언트에서만 플레이어 생성 강제 실행
        if (runner.IsSharedModeMasterClient)
        {
            Debug.Log("[ApartGameEvent] 🔥 마스터 클라이언트에서 플레이어 생성 강제 실행");
            
            // 모든 PlayerManager의 플레이어를 ApartPlayerManager에 추가
            foreach (var fusionPlayer in PlayerManager.Instance.Players)
            {
                Debug.Log($"[ApartGameEvent] 플레이어 추가 시도: UUID={fusionPlayer.Uuid}, Color={fusionPlayer.PlayerColor}");
                apartPlayerManager.AddPlayerDirect(fusionPlayer);
            }
        }
        
        // 🔥 모든 클라이언트에서 ApartPlayer들이 생성될 때까지 대기
        int maxWaitTime = 10; // 최대 10초 대기
        int waitTime = 0;
        
        while (apartPlayerManager.players.Count < PlayerManager.Instance.Players.Count && waitTime < maxWaitTime)
        {
            Debug.Log($"[ApartGameEvent] ApartPlayer 생성 대기 중... ({apartPlayerManager.players.Count}/{PlayerManager.Instance.Players.Count})");
            
            // 🔥 마스터 클라이언트에서 주기적으로 새로고침
            if (runner.IsSharedModeMasterClient)
            {
                Debug.Log("[ApartGameEvent] 마스터 클라이언트에서 새로고침 시도");
                apartPlayerManager.RefreshPlayersFromFusion();
                
                // 누락된 플레이어 강제 추가
                foreach (var fusionPlayer in PlayerManager.Instance.Players)
                {
                    bool exists = false;
                    string targetName = $"Player_{fusionPlayer.Uuid}";
                    
                    foreach (var apartPlayer in apartPlayerManager.players)
                    {
                        if (apartPlayer.playerName == targetName)
                        {
                            exists = true;
                            break;
                        }
                    }
                    
                    if (!exists)
                    {
                        Debug.Log($"[ApartGameEvent] 누락된 플레이어 강제 추가: {targetName}");
                        apartPlayerManager.AddPlayerDirect(fusionPlayer);
                    }
                }
            }
            
            yield return new WaitForSeconds(1f);
            waitTime++;
        }
        
        if (waitTime >= maxWaitTime)
        {
            Debug.LogWarning($"[ApartGameEvent] ⚠️ 타임아웃! ApartPlayer 생성 불완전: {apartPlayerManager.players.Count}/{PlayerManager.Instance.Players.Count}");
        }
        else
        {
            Debug.Log($"[ApartGameEvent] ✅ ApartPlayer 생성 완료: {apartPlayerManager.players.Count}명");
        }
        
        // 🔥 생성된 ApartPlayer 정보 출력
        for (int i = 0; i < apartPlayerManager.players.Count; i++)
        {
            var player = apartPlayerManager.players[i];
            Debug.Log($"[ApartGameEvent] ApartPlayer[{i}]: Name={player.playerName}, Color={player.PlayerColor}");
        }
        
        // 🔥 마스터 클라이언트에서만 게임 시작
        if (runner.IsSharedModeMasterClient)
        {
            Debug.Log("[ApartGameEvent] 🔥 마스터 클라이언트에서 게임 시작");
            StartGame();
        }
        else
        {
            Debug.Log("[ApartGameEvent] 클라이언트는 마스터의 게임 시작을 대기");
        }
    }
    
    private void StartGame()
    {
        Debug.Log("[ApartGameEvent] StartGame 호출됨");
        
        if (ApartTurnManager.Instance != null)
        {
            Debug.Log("[ApartGameEvent] ApartTurnManager.StartTurn 호출");
            ApartTurnManager.Instance.StartTurn();
        }
        else
        {
            Debug.LogError("[ApartGameEvent] ApartTurnManager.Instance가 null입니다!");
        }

        ApartPlayer currentPlayer = apartPlayerManager.GetCurrentPlayer();
        Debug.Log($"[ApartGameEvent] 현재 플레이어: {currentPlayer?.playerName} (인덱스: {apartPlayerManager.CurrentPlayerIndex})");
    }
}