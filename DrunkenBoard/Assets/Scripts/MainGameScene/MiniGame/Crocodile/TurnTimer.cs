using UnityEngine;
using Fusion;
using System;

public class TurnTimer : NetworkBehaviour
{
    [Networked]
    private float remainingTime { get; set; }
    
    [Networked]
    private bool isTimerRunning { get; set; }
    
    [Networked]
    private TickTimer networkTimer { get; set; } // 🔥 네트워크 타이머 사용

    public float turnDuration = 5f;

    [Header("씬에 있는 TimerUI를 인스펙터에서 연결")]
    [SerializeField] private TimerUI timerUI;

    public event Action OnTurnTimeout;
    public event Action OnTurnEnded;

    private bool previousTimerRunning = false;

    public override void Spawned()
    {
        base.Spawned();

        if (timerUI != null)
        {
            timerUI.Hide();
        }
        else
        {
            Debug.LogWarning("❗ TimerUI가 인스펙터에 연결되지 않았어!");
        }
    }

    public void StartTurn()
    {
        Debug.Log($"⏰ StartTurn 호출됨 - HasStateAuthority: {Object.HasStateAuthority}");
        
        if (Object.HasStateAuthority)
        {
            // 네트워크 타이머 시작
            networkTimer = TickTimer.CreateFromSeconds(Runner, turnDuration);
            remainingTime = turnDuration;
            isTimerRunning = true;
            
            Debug.Log($"🚀 타이머 시작됨: {turnDuration}초");
        }
        
        // 🔥 모든 클라이언트에서 UI 표시
        if (timerUI != null)
        {
            timerUI.Show();
            timerUI.UpdateTimer(turnDuration);
        }
    }

    public void EndTurn()
    {
        Debug.Log($"⏹️ EndTurn 호출됨 - HasStateAuthority: {Object.HasStateAuthority}");
        
        if (Object.HasStateAuthority && isTimerRunning)
        {
            isTimerRunning = false;
            networkTimer = TickTimer.None;
            
            OnTurnEnded?.Invoke();
        }
        
        // 🔥 모든 클라이언트에서 UI 숨김
        if (timerUI != null)
        {
            timerUI.Hide();
        }
    }

    public void ForceEndTurnByInput()
    {
        Debug.Log("🚀 ForceEndTurnByInput 호출됨");
        EndTurn();
    }

    public override void FixedUpdateNetwork()
    {
        // 🔥 StateAuthority에서만 타이머 로직 처리
        if (Object.HasStateAuthority && isTimerRunning)
        {
            if (networkTimer.Expired(Runner))
            {
                Debug.Log("⏰ 네트워크 타이머 만료!");
                
                isTimerRunning = false;
                networkTimer = TickTimer.None;
                
                // 타임아웃 이벤트 발생
                RPC_OnTimeout();
                OnTurnEnded?.Invoke();
            }
            else if (networkTimer.IsRunning)
            {
                // 남은 시간 계산 및 업데이트
                float timeLeft = networkTimer.RemainingTime(Runner) ?? 0f;
                remainingTime = timeLeft;
            }
        }
    }

    // 🔥 모든 클라이언트에서 실시간 UI 업데이트
    public override void Render()
    {
        // 타이머 상태 변경 감지
        if (previousTimerRunning != isTimerRunning)
        {
            previousTimerRunning = isTimerRunning;
            
            if (isTimerRunning)
            {
                Debug.Log("🔄 타이머 시작 상태 감지");
                if (timerUI != null)
                {
                    timerUI.Show();
                }
            }
            else
            {
                Debug.Log("🔄 타이머 종료 상태 감지");
                if (timerUI != null)
                {
                    timerUI.Hide();
                }
            }
        }

        // 🔥 모든 클라이언트에서 UI 업데이트 (매 프레임)
        if (isTimerRunning && timerUI != null)
        {
            timerUI.UpdateTimer(remainingTime);
            
            // 디버그 로그 (너무 많이 출력되지 않도록 가끔만)
            if (Time.frameCount % 60 == 0) // 1초마다
            {
                Debug.Log($"⏱️ 타이머 업데이트: {remainingTime:F1}초 남음");
            }
        }
    }

    // 모든 클라이언트에서 타임아웃 처리
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_OnTimeout()
    {
        Debug.Log("⏰ RPC_OnTimeout 받음 - 자동 클릭 처리");
        
        // UI 숨기기
        if (timerUI != null)
        {
            timerUI.Hide();
        }
        
        // 타임아웃 이벤트 발생
        OnTurnTimeout?.Invoke();
    }

    public float GetRemainingTime()
    {
        return remainingTime;
    }

    public bool IsRunning()
    {
        return isTimerRunning;
    }

    // 디버그용 - 강제 타임아웃
    [ContextMenu("Force Timeout")]
    private void DebugForceTimeout()
    {
        if (Object.HasStateAuthority && isTimerRunning)
        {
            networkTimer = TickTimer.CreateFromSeconds(Runner, 0.1f); // 0.1초 후 만료
        }
    }
    public void HideTimerUI()
    {
        if (timerUI != null)
        {
            timerUI.Hide();
            Debug.Log("🛑 게임 종료 - 타이머 UI 숨김");
        }
    }

}