using System.Collections;
using UnityEngine;
using TMPro;
using Fusion;

// 포톤 퓨전2 쉐어드 모드용 ApartTurnManager - 정리됨
public class ApartTurnManager : NetworkBehaviour
{
    public static ApartTurnManager Instance;

    [SerializeField] private float turnTimeLimit = 5f;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private NetworkTimer networkTimer;

    // 필요한 네트워크 변수들만
    [Networked] public bool IsTurnActive { get; set; } = false;
    [Networked, Capacity(8)] public NetworkArray<int> PlayerSpaceCounts => default;

    public override void Spawned()
    {
        Instance = this;
        
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
    }

    // 1️⃣ 턴 시작 (StateAuthority에서만)
    public void StartTurn()
    {
        if (!Object.HasStateAuthority) return;
        
        RPC_StartCountdown();
    }

    // 2️⃣ 카운트다운 기능 (모든 클라이언트에서 동시 실행)
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_StartCountdown()
    {
        StartCoroutine(CountdownSequence());
    }

    private IEnumerator CountdownSequence()
    {
        // 3-2-1 카운트다운
        for (int i = 3; i > 0; i--)
        {
            if (countdownText != null)
            {
                countdownText.text = i.ToString();
                countdownText.gameObject.SetActive(true);
                StartCoroutine(PlayPunchAnimation());
            }
            yield return new WaitForSeconds(0.8f);
        }

        // START 표시
        if (countdownText != null)
        {
            countdownText.text = "START";
            StartCoroutine(PlayPunchAnimation());
        }
        yield return new WaitForSeconds(0.5f);

        // 카운트다운 종료
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        // StateAuthority에서만 스페이스바 단계 시작
        if (Object.HasStateAuthority)
        {
            StartSpacebarPhase();
        }
    }

    private IEnumerator PlayPunchAnimation()
    {
        if (countdownText == null) yield break;

        float duration = 0.3f;
        float maxScale = 1.8f;
        float minScale = 1f;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            float scale = Mathf.Lerp(maxScale, minScale, t * t);
            countdownText.transform.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }

        countdownText.transform.localScale = Vector3.one;
    }

    // 3️⃣ 스페이스바 단계 관리
    private void StartSpacebarPhase()
    {
        if (!Object.HasStateAuthority) return;

        IsTurnActive = true;
        InitializePlayerSpaceCounts();
        
        // InputManager에게 턴 시작 알림
        RPC_NotifyTurnStarted();

        // NetworkTimer 시작 (모든 클라이언트 동기화)
        if (networkTimer != null)
        {
            networkTimer.ActOnEndTimer = OnTimerEnd;
            networkTimer.StartCountDown_RPC(turnTimeLimit);
        }
    }

    private void InitializePlayerSpaceCounts()
    {
        if (!Object.HasStateAuthority) return;

        for (int i = 0; i < ApartPlayerManager.Instance.players.Count; i++)
        {
            PlayerSpaceCounts.Set(i, 0);
        }
    }

    // 4️⃣ 스페이스바 입력 처리
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_OnSpacePressed(int uuid)
    {
        if (!IsTurnActive) return;

        // 🔥 중앙화된 메소드 사용
        int playerIndex = ApartPlayerManager.Instance.GetPlayerIndexByUuid(uuid);
        if (playerIndex < 0) return;

        // 2번 제한 체크
        if (PlayerSpaceCounts[playerIndex] >= 2) return;

        // 카운트 증가 및 층 생성
        PlayerSpaceCounts.Set(playerIndex, PlayerSpaceCounts[playerIndex] + 1);

        EPlayerColor playerColor = ApartPlayerManager.Instance.players[playerIndex].PlayerColor;
        if (ApartGameManager.Instance != null)
        {
            ApartGameManager.Instance.AddFloor(playerColor);
        }

        // 모든 플레이어 완료 체크
        if (CheckAllPlayersFinished())
        {
            EndSpacebarPhase();
        }
    }

    private bool CheckAllPlayersFinished()
    {
        if (ApartPlayerManager.Instance == null) return false;

        for (int i = 0; i < ApartPlayerManager.Instance.players.Count; i++)
        {
            if (PlayerSpaceCounts[i] < 2)
            {
                return false;
            }
        }
        return true;
    }

    // 5️⃣ 타이머 종료 처리
    private void OnTimerEnd()
    {
        if (!Object.HasStateAuthority) return;
        
        // 부족한 스페이스바 자동 추가
        for (int i = 0; i < ApartPlayerManager.Instance.players.Count; i++)
        {
            int missingCount = 2 - PlayerSpaceCounts[i];
            if (missingCount > 0)
            {
                EPlayerColor playerColor = ApartPlayerManager.Instance.players[i].PlayerColor;
                for (int j = 0; j < missingCount; j++)
                {
                    if (ApartGameManager.Instance != null)
                    {
                        ApartGameManager.Instance.AddFloor(playerColor);
                    }
                    PlayerSpaceCounts.Set(i, PlayerSpaceCounts[i] + 1);
                }
            }
        }

        EndSpacebarPhase();
    }

    private void EndSpacebarPhase()
    {
        if (!Object.HasStateAuthority) return;

        IsTurnActive = false;

        // NetworkTimer 정지
        if (networkTimer != null)
        {
            networkTimer.StopCountDown_RPC();
        }

        // InputManager에게 턴 종료 알림
        RPC_NotifyTurnEnded();

        // END 텍스트 표시
        RPC_ShowEndText();
        
        // 순서 저장 완료
        if (ApartGameManager.Instance != null)
        {
            ApartGameManager.Instance.CompleteSequence();
        }
        
        // 숫자 입력 단계로 이동
        StartCoroutine(ShowNumberInputAfterDelay(2f));
    }

    // 6️⃣ RPC 통신 메서드들
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_NotifyTurnStarted()
    {
        if (ApartInputManager.Instance != null)
        {
            ApartInputManager.Instance.OnTurnStarted();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_NotifyTurnEnded()
    {
        if (ApartInputManager.Instance != null)
        {
            ApartInputManager.Instance.OnTurnEnded();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ShowEndText()
    {
        if (countdownText != null)
        {
            countdownText.text = "END";
            countdownText.gameObject.SetActive(true);
            StartCoroutine(HideEndTextAfterDelay(2f));
        }
    }

    private IEnumerator HideEndTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
    }

    // 🔧 수정: 숫자 입력 단계에서 현재 플레이어에게 UI 표시
    private IEnumerator ShowNumberInputAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
    
        // 🔥 마스터가 아닌 현재 턴 플레이어에게 UI 표시
        if (Object.HasStateAuthority && ApartInputManager.Instance != null)
        {
            ApartInputManager.Instance.ShowNumberInputUIForCurrentPlayer();
        }
    }

    // 7️⃣ 유틸리티 메서드들
    public void ResetTurnSystem()
    {
        if (!Object.HasStateAuthority) return;

        IsTurnActive = false;

        if (networkTimer != null)
        {
            networkTimer.StopCountDown_RPC();
        }

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
            countdownText.text = "";
        }
    }
    // 🔧 수정: TurnManager 완전 초기화 (NetworkTimer 포함)
    public void ResetManager()
    {
        Debug.Log("[ApartTurnManager.ResetManager] 🔄 TurnManager 초기화");
    
        // 네트워크 변수 초기화 (StateAuthority에서만)
        if (Object.HasStateAuthority)
        {
            IsTurnActive = false;
        
            // PlayerSpaceCounts 초기화
            for (int i = 0; i < PlayerSpaceCounts.Length; i++)
            {
                PlayerSpaceCounts.Set(i, 0);
            }
        }
    
        // 🔥 NetworkTimer 완전 리셋
        if (networkTimer != null)
        {
            // 콜백 해제
            networkTimer.ActOnEndTimer = null;
        
            // 타이머 정지
            networkTimer.StopCountDown_RPC();
        
            // 강제 리셋 (혹시 내부 상태가 남아있을 경우)
            StartCoroutine(ForceResetTimerAfterDelay());
        }
    
        // UI 정리
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
            countdownText.text = "";
            countdownText.transform.localScale = Vector3.one;
        }
    
        Debug.Log("[ApartTurnManager.ResetManager] ✅ TurnManager 초기화 완료");
    }
    // 🔧 추가: NetworkTimer 강제 리셋 (딜레이 후)
    private IEnumerator ForceResetTimerAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
    
        if (networkTimer != null)
        {
            // 한번 더 정지 시도
            networkTimer.StopCountDown_RPC();
            Debug.Log("[ForceResetTimerAfterDelay] ✅ NetworkTimer 강제 리셋 완료");
        }
    }
}