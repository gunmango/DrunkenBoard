using UnityEngine;
using Fusion;
using System;

public class TurnTimer : NetworkBehaviour
{
    [Networked]
    private float remainingTime { get; set; }

    public float turnDuration = 10f; // 턴 시간 (초)
    private bool isRunning = false;

    [Header("씬에 있는 TimerUI를 인스펙터에서 연결")]
    [SerializeField] private TimerUI timerUI;

    public event Action OnTurnTimeout; // 시간 끝났을 때 알림
    public event Action OnTurnEnded;   // 턴이 끝났을 때 알림

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
        if (!Object.HasStateAuthority) return;

        remainingTime = turnDuration;
        isRunning = true;

        if (timerUI != null)
        {
            timerUI.Show();
        }
    }

    public void EndTurn()
    {
        if (!Object.HasStateAuthority || !isRunning) return;

        isRunning = false;

        if (timerUI != null)
        {
            timerUI.Hide();
        }

        OnTurnEnded?.Invoke();
    }

    public void ForceEndTurnByInput()
    {
        EndTurn();
    }

    private void Update()
    {
        if (!Object.HasStateAuthority || !isRunning) return;

        remainingTime -= Time.deltaTime;

        if (timerUI != null)
        {
            timerUI.UpdateTimer(remainingTime);
        }

        if (remainingTime <= 0f)
        {
            isRunning = false;

            if (timerUI != null)
                timerUI.Hide();

            OnTurnTimeout?.Invoke();
            OnTurnEnded?.Invoke();
        }
    }

    public float GetRemainingTime()
    {
        return remainingTime;
    }
}