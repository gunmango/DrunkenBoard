using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CrocodilePlayer : ATurnPlayer
{
    private CrocodileTooth[] allTeeth;
    private bool clicked = false;
    private CrocodileTooth selectedTooth;

    private TurnTimer _turnTimer;

    // 턴 시스템, UUID, 타이머, 그리고 이빨 배열까지 초기화 인자로 받음
    public void Initialize(TurnSystem turnSystem, int uuid, TurnTimer turnTimer, CrocodileTooth[] teeth)
    {
        TurnSystem = turnSystem;
        Uuid = uuid;
        _turnTimer = turnTimer;
        allTeeth = teeth;
    }
    public void OnTurnStart()
    {
        // 🔄 턴 시작 시 모든 이빨의 상태를 확인해서 시각적 상태 반영
        foreach (var tooth in allTeeth)
        {
            tooth.UpdateVisuals();
        }

        StartCoroutine(TakeTurnCoroutine());
    }

    protected override IEnumerator TakeTurnCoroutine()
    {
        clicked = false;
        selectedTooth = null;

        Debug.Log($"🎯 플레이어 {Uuid} 턴 시작");

        // ✅ 이빨 이벤트 구독
        SubscribeToothEvents();

        // ✅ 타이머 시작
        _turnTimer.OnTurnTimeout += OnTurnTimeout;
        _turnTimer.StartTurn();

        Debug.Log($"⏱️ 타이머 시작, 이빨 클릭 대기 중...");

        // ✅ 클릭 완료까지 대기
        yield return new WaitUntil(() => clicked == true);

        Debug.Log($"✅ 이빨 클릭 완료: {selectedTooth?.toothIndex}");

        CleanupTurn();
        EndTurn();
    }

    // ✅ 이빨 이벤트 구독
    private void SubscribeToothEvents()
    {
        foreach (var tooth in allTeeth)
        {
            tooth.OnToothClicked += OnToothClicked;
        }
    }

    // ✅ 이빨 이벤트 구독 해제
    private void UnsubscribeToothEvents()
    {
        foreach (var tooth in allTeeth)
        {
            tooth.OnToothClicked -= OnToothClicked;
        }
    }

    // ✅ 이빨 클릭 이벤트 처리
    private void OnToothClicked(int toothIndex)
    {
        if (clicked) return; // 이미 클릭했으면 무시

        Debug.Log($"🦷 플레이어 {Uuid}가 이빨 {toothIndex} 클릭 시도");

        // 해당 이빨 찾기
        CrocodileTooth clickedTooth = null;
        foreach (var tooth in allTeeth)
        {
            if (tooth.toothIndex == toothIndex)
            {
                clickedTooth = tooth;
                break;
            }
        }

        if (clickedTooth != null && !clickedTooth.IsClicked())
        {
            // 실제 클릭 처리
            clickedTooth.ProcessClick();
            
            // 클릭 완료 처리
            NotifyToothClicked(clickedTooth);
        }
    }

    // ✅ 타임아웃 시 자동 클릭
    private void OnTurnTimeout()
    {
        if (clicked) return;

        Debug.Log($"⏰ 플레이어 {Uuid} 타임아웃 - 자동 클릭 수행");

        // 클릭되지 않은 이빨 찾기
        List<CrocodileTooth> availableTeeth = new List<CrocodileTooth>();
        foreach (var tooth in allTeeth)
        {
            if (!tooth.IsClicked())
                availableTeeth.Add(tooth);
        }

        if (availableTeeth.Count > 0)
        {
            // 랜덤 선택
            int randomIndex = Random.Range(0, availableTeeth.Count);
            CrocodileTooth randomTooth = availableTeeth[randomIndex];
            
            Debug.Log($"🎲 자동 선택된 이빨: {randomTooth.toothIndex}");
            
            // 자동 클릭 처리
            randomTooth.ProcessClick();
            NotifyToothClicked(randomTooth);
        }
    }

    // ✅ 턴 정리
    private void CleanupTurn()
    {
        Debug.Log($"🧹 플레이어 {Uuid} 턴 정리");

        // 이벤트 구독 해제
        UnsubscribeToothEvents();

        // 타이머 정리
        _turnTimer.OnTurnTimeout -= OnTurnTimeout;
        _turnTimer.EndTurn();
    }

    // ✅ 이빨 클릭 완료 알림
    public void NotifyToothClicked(CrocodileTooth tooth)
    {
        if (clicked) return;

        clicked = true;
        selectedTooth = tooth;
        
        Debug.Log($"✅ 플레이어 {Uuid} 이빨 {tooth.toothIndex} 선택 완료");
        
        // 타이머 강제 종료
        _turnTimer.ForceEndTurnByInput();
    }

    // ✅ 게임 종료 시 정리
    public void Cleanup()
    {
        UnsubscribeToothEvents();
        if (_turnTimer != null)
        {
            _turnTimer.OnTurnTimeout -= OnTurnTimeout;
        }
    }
}