using System.Collections;
using UnityEngine;
using Fusion;
using System.Collections.Generic;

// 포톤 퓨전2 쉐어드 모드용 ApartGameManager - 정리됨
public class ApartGameManager : NetworkBehaviour
{
    public static ApartGameManager Instance;
    
    public Transform ApartBase;
    [SerializeField] private PlayerPrefabTable PlayerPrefabTable;
    public float FloorHeight = 1.0f;
    [SerializeField] private float cameraFollowSpeed = 2f;
    [SerializeField] private bool enableRealTimeFollow = true;
    
    [Networked] public int CurrentFloorCount { get; set; }
    [Networked] public bool IsGameEnded { get; set; }
    [Networked] public bool IsRealTimeFollowing { get; set; } = false;
    [Networked] public bool IsSequenceCompleted { get; set; } = false; // 🔥 순서 저장 완료 플래그

    public System.Action<EPlayerColor> OnGameEnded;
    
    private List<NetworkObject> spawnedFloors = new List<NetworkObject>();
    private Coroutine realTimeFollowCoroutine;
    
    // 🔥 완성된 최종 순서 저장 (3번에서 저장할 순서)
    [Networked, Capacity(100)] public NetworkArray<EPlayerColor> CompletedFloorSequence => default;
    [Networked] public int CompletedSequenceLength { get; set; } = 0;
    
    public int GetCurrentFloorCount()
    {
        return CurrentFloorCount;
    }
    
    public override void Spawned()
    {
        Instance = this;
    }

    // 층 추가 (순서 기록 포함)
    public void AddFloor(EPlayerColor playerColor)
    {
        if (Runner == null || !Runner.IsSharedModeMasterClient) return;

        if (playerColor == EPlayerColor.None)
        {
            playerColor = GetNextPlayerColorFromSequence();
        }
        
        CreateFloorNetworkObject(playerColor);
        
        // 🔥 아직 순서가 완성되지 않았으면 기록
        if (!IsSequenceCompleted)
        {
            RecordFloorInSequence(playerColor);
        }
        
        // 실시간 카메라 따라가기 시작
        if (enableRealTimeFollow && !IsRealTimeFollowing)
        {
            StartRealTimeFollow();
        }
    }
    
    // 🔥 층 순서 기록 (3번에서 저장할 순서)
    private void RecordFloorInSequence(EPlayerColor playerColor)
    {
        if (!Runner.IsSharedModeMasterClient) return;
        
        if (CompletedSequenceLength < CompletedFloorSequence.Length)
        {
            CompletedFloorSequence.Set(CompletedSequenceLength, playerColor);
            CompletedSequenceLength++;
            
            Debug.Log($"[RecordFloorInSequence] 순서 기록: {CompletedSequenceLength}층 - {playerColor}");
        }
    }
    
    // 🔥 순서 저장 완료 (스페이스바 + 자동 추가 모든 층이 완성된 후 호출)
    public void CompleteSequence()
    {
        if (!Runner.IsSharedModeMasterClient) return;
        
        IsSequenceCompleted = true;
        
        Debug.Log($"[CompleteSequence] 🔥 최종 순서 저장 완료! 총 {CompletedSequenceLength}층");
        Debug.Log("=== 저장된 최종 순서 ===");
        for (int i = 0; i < CompletedSequenceLength; i++)
        {
            Debug.Log($"층 {i + 1}: {CompletedFloorSequence[i]}");
        }
        Debug.Log("========================");
    }
    
    // 실시간 카메라 따라가기 시작
    private void StartRealTimeFollow()
    {
        if (!Runner.IsSharedModeMasterClient) return;
        
        IsRealTimeFollowing = true;
        RPC_StartRealTimeFollow();
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_StartRealTimeFollow()
    {
        if (realTimeFollowCoroutine != null)
        {
            StopCoroutine(realTimeFollowCoroutine);
        }
        
        realTimeFollowCoroutine = StartCoroutine(RealTimeFollowCoroutine());
    }
    
    private IEnumerator RealTimeFollowCoroutine()
    {
        Vector3 startPosition = ApartBase.position;
        
        while (IsRealTimeFollowing && spawnedFloors.Count > 0)
        {
            float targetY = -(CurrentFloorCount - 1) * FloorHeight + FloorHeight;
            Vector3 targetPosition = new Vector3(startPosition.x, targetY, startPosition.z);
            
            ApartBase.position = Vector3.Lerp(ApartBase.position, targetPosition, 
                Time.deltaTime * cameraFollowSpeed);
                
            yield return null;
        }
    }
    
    public void StopRealTimeFollow()
    {
        if (!Runner.IsSharedModeMasterClient) return;
        
        IsRealTimeFollowing = false;
        RPC_StopRealTimeFollow();
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_StopRealTimeFollow()
    {
        if (realTimeFollowCoroutine != null)
        {
            StopCoroutine(realTimeFollowCoroutine);
            realTimeFollowCoroutine = null;
        }
    }
    
    // 🔧 수정: 네트워크 오브젝트 생성 - 정확한 위치 계산
    private void CreateFloorNetworkObject(EPlayerColor playerColor)
    {
        if (!Runner.IsSharedModeMasterClient) return;

        GameObject floorPrefab = GetFloorPrefabByPlayerColor(playerColor);
        if (floorPrefab == null) return;

        CurrentFloorCount++;

        // 🔥 정확한 위치 계산 (ApartBase 기준)
        Vector3 spawnPosition = ApartBase.position + new Vector3(0, (CurrentFloorCount - 1) * FloorHeight, 0);
    
        Debug.Log($"[CreateFloorNetworkObject] 마스터 층 생성: {CurrentFloorCount}층, 위치: {spawnPosition}, 색상: {playerColor}");
    
        GameObject floorObj = Instantiate(floorPrefab, spawnPosition, Quaternion.identity);
    
        if (floorObj != null)
        {
            floorObj.transform.SetParent(ApartBase);
        
            // 🔥 부모 설정 후 로컬 위치 재조정
            floorObj.transform.localPosition = new Vector3(0, (CurrentFloorCount - 1) * FloorHeight, 0);
        
            SetFloorNumberWithSprite(floorObj, CurrentFloorCount);
        
            NetworkObject networkObj = floorObj.GetComponent<NetworkObject>();
            if (networkObj != null)
            {
                spawnedFloors.Add(networkObj);
            }
        
            Debug.Log($"[CreateFloorNetworkObject] ✅ 마스터 층 생성 완료: 로컬위치 {floorObj.transform.localPosition}");
        }
    
        // 🔥 정확한 위치와 번호를 클라이언트에 전달
        RPC_NotifyFloorCreated(playerColor, spawnPosition, CurrentFloorCount, (CurrentFloorCount - 1) * FloorHeight);
    }

    
    // 🔧 수정: 클라이언트 층 생성 - 마스터와 동일한 위치 적용
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_NotifyFloorCreated(EPlayerColor playerColor, Vector3 worldPosition, int floorIndex, float localYPosition)
    {
        if (Runner.IsSharedModeMasterClient) return;
    
        Debug.Log($"[RPC_NotifyFloorCreated] 클라이언트 층 생성: {floorIndex}층, 월드위치: {worldPosition}, 로컬Y: {localYPosition}, 색상: {playerColor}");
    
        GameObject floorPrefab = GetFloorPrefabByPlayerColor(playerColor);
        if (floorPrefab == null) 
        {
            Debug.LogError($"[RPC_NotifyFloorCreated] ❌ {playerColor} 프리팹을 찾을 수 없음!");
            return;
        }
    
        // 🔥 마스터와 동일한 월드 위치로 생성
        GameObject floorObj = Instantiate(floorPrefab, worldPosition, Quaternion.identity);
        if (floorObj != null)
        {
            floorObj.transform.SetParent(ApartBase);
        
            // 🔥 부모 설정 후 정확한 로컬 위치 설정
            floorObj.transform.localPosition = new Vector3(0, localYPosition, 0);
        
            SetFloorNumberWithSprite(floorObj, floorIndex);
        
            NetworkObject networkObj = floorObj.GetComponent<NetworkObject>();
            if (networkObj != null)
            {
                spawnedFloors.Add(networkObj);
            }
        
            Debug.Log($"[RPC_NotifyFloorCreated] ✅ 클라이언트 층 생성 완료: 로컬위치 {floorObj.transform.localPosition}");
        }
    }
    
    // 🔧 수정: 층 번호 설정 강화 - 디버깅 및 오류 처리 추가
    private void SetFloorNumberWithSprite(GameObject floorObj, int floorNumber)
    {
        Debug.Log($"[SetFloorNumberWithSprite] 시작: {floorNumber}층 번호 설정");
    
        // 방법 1: 직접 컴포넌트 검색
        NumberingSprite numberingSprite = floorObj.GetComponent<NumberingSprite>();
        if (numberingSprite != null)
        {
            numberingSprite.SetNumber(floorNumber);
            Debug.Log($"[SetFloorNumberWithSprite] ✅ 직접 컴포넌트에서 {floorNumber} 설정 완료");
            return;
        }
    
        // 방법 2: 자식 오브젝트에서 검색
        numberingSprite = floorObj.GetComponentInChildren<NumberingSprite>();
        if (numberingSprite != null)
        {
            numberingSprite.SetNumber(floorNumber);
            Debug.Log($"[SetFloorNumberWithSprite] ✅ 자식 컴포넌트에서 {floorNumber} 설정 완료");
            return;
        }
    
        // 방법 3: 이름으로 찾기
        string[] possibleNames = { "NumberDisplay", "Number", "FloorNumber", "Text", "NumberSprite" };
        foreach (string name in possibleNames)
        {
            Transform numberTransform = floorObj.transform.Find(name);
            if (numberTransform != null)
            {
                NumberingSprite foundSprite = numberTransform.GetComponent<NumberingSprite>();
                if (foundSprite != null)
                {
                    foundSprite.SetNumber(floorNumber);
                    Debug.Log($"[SetFloorNumberWithSprite] ✅ 이름 '{name}'에서 {floorNumber} 설정 완료");
                    return;
                }
            }
        }
    
        // 방법 4: 모든 자식을 재귀적으로 검색
        NumberingSprite[] allSprites = floorObj.GetComponentsInChildren<NumberingSprite>();
        if (allSprites.Length > 0)
        {
            allSprites[0].SetNumber(floorNumber);
            Debug.Log($"[SetFloorNumberWithSprite] ✅ 재귀 검색에서 {floorNumber} 설정 완료");
            return;
        }
    
        Debug.LogWarning($"[SetFloorNumberWithSprite] ⚠️ NumberingSprite를 찾을 수 없음! 프리팹: {floorObj.name}");
    
        // 🔥 디버깅용: 프리팹 구조 출력
        Debug.Log($"[SetFloorNumberWithSprite] 프리팹 구조:");
        PrintGameObjectHierarchy(floorObj.transform, 0);
    }
    
    // 🔧 추가: 디버깅용 계층구조 출력
    private void PrintGameObjectHierarchy(Transform parent, int depth)
    {
        string indent = new string(' ', depth * 2);
        var components = parent.GetComponents<Component>();
        string componentList = string.Join(", ", System.Array.ConvertAll(components, c => c.GetType().Name));

        Debug.Log($"{indent}├─ {parent.name} [{componentList}]");

        for (int i = 0; i < parent.childCount; i++)
        {
            PrintGameObjectHierarchy(parent.GetChild(i), depth + 1);
        }
    }

    // 🔥 수정: 저장된 완성 순서에서 다음 색상 가져오기
    private EPlayerColor GetNextPlayerColorFromSequence()
    {
        if (ApartPlayerManager.Instance == null || ApartPlayerManager.Instance.players.Count == 0)
        {
            return EPlayerColor.Red;
        }

        // 🔥 순서가 완성되었으면 저장된 순서를 반복 사용
        if (IsSequenceCompleted && CompletedSequenceLength > 0)
        {
            int nextIndex = CurrentFloorCount % CompletedSequenceLength;
            EPlayerColor nextColor = CompletedFloorSequence[nextIndex];
            
            Debug.Log($"[GetNextPlayerColorFromSequence] 🔥 완성된 순서 사용: 층 {CurrentFloorCount + 1} -> 인덱스 {nextIndex} -> 색상 {nextColor}");
            return nextColor;
        }
        
        // 🔥 순서가 아직 완성되지 않았으면 플레이어 순서대로
        if (CurrentFloorCount == 0)
        {
            return ApartPlayerManager.Instance.players[0].PlayerColor;
        }
        
        int playerIndex = CurrentFloorCount % ApartPlayerManager.Instance.players.Count;
        EPlayerColor color = ApartPlayerManager.Instance.players[playerIndex].PlayerColor;
        
        Debug.Log($"[GetNextPlayerColorFromSequence] 기본 순서: 층 {CurrentFloorCount + 1} -> 플레이어 인덱스 {playerIndex} -> 색상 {color}");
        return color;
    }

    private GameObject GetFloorPrefabByPlayerColor(EPlayerColor playerColor)
    {
        if (PlayerManager.Table != null)
        {
            PlayerColorSet colorSet = PlayerManager.Table.GetColorSet(playerColor);
            if (colorSet?.ApartPrefab != null)
            {
                return colorSet.ApartPrefab;
            }
        }

        if (PlayerPrefabTable != null) 
        {
            PlayerColorSet colorSet = PlayerPrefabTable.GetColorSet(playerColor);
            if (colorSet?.ApartPrefab != null)
            {
                return colorSet.ApartPrefab;
            }
        }

        if (ApartPlayerManager.Instance != null)
        {
            PlayerColorSet colorSet = ApartPlayerManager.Instance.GetPlayerColorSet(playerColor);
            if (colorSet?.ApartPrefab != null)
            {
                return colorSet.ApartPrefab;
            }
        }

        return null;
    }

    // 🔥 수정된 목표층까지 층 쌓기 (완성된 순서 사용)
    public void RaiseFloorsToTarget(int targetFloor, System.Action onComplete = null)
    {
        if (!Runner.IsSharedModeMasterClient) return;
        StartCoroutine(RaiseFloorsCoroutine(targetFloor, onComplete));
    }

    private IEnumerator RaiseFloorsCoroutine(int targetFloor, System.Action onComplete)
    {
        if (CurrentFloorCount >= targetFloor)
        {
            onComplete?.Invoke();
            yield break;
        }

        int missingFloors = targetFloor - CurrentFloorCount;
        
        if (ApartPlayerManager.Instance == null || ApartPlayerManager.Instance.players.Count == 0)
        {
            onComplete?.Invoke();
            yield break;
        }

        // 🔥 순서가 완성되지 않았으면 에러
        if (!IsSequenceCompleted)
        {
            Debug.LogError("[RaiseFloorsCoroutine] ❌ 순서가 완성되지 않았는데 추가층 요청됨!");
            onComplete?.Invoke();
            yield break;
        }

        Debug.Log($"[RaiseFloorsCoroutine] 🔥 저장된 순서로 {missingFloors}개 층 추가 생성");

        if (enableRealTimeFollow)
        {
            StartRealTimeFollow();
        }

        for (int i = 0; i < missingFloors; i++)
        {
            // 🔥 완성된 순서에서 다음 색상 가져오기
            EPlayerColor playerColor = GetNextPlayerColorFromSequence();
            Debug.Log($"[RaiseFloorsCoroutine] {i+1}/{missingFloors}: {playerColor}");
            AddFloor(playerColor);
            
            yield return new WaitForSeconds(0.05f);
        }

        yield return null;
        onComplete?.Invoke();
    }

    public void HighlightFloor(int floorNumber)
    {
        if (!Runner.IsSharedModeMasterClient) return;
        StopRealTimeFollow();
        RPC_HighlightFloor(floorNumber);
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_HighlightFloor(int floorNumber)
    {
        int index = floorNumber - 1;
        if (index < 0 || index >= spawnedFloors.Count) return;

        Transform targetFloor = spawnedFloors[index].transform;
        StartCoroutine(MovetoHighlight(targetFloor));
    }
    
    private IEnumerator MovetoHighlight(Transform targetFloor)
    {  
        yield return StartCoroutine(MoveApartToFloorSmooth(targetFloor));
        yield return StartCoroutine(HighlightAnima(targetFloor));
        yield return new WaitForSeconds(1f);
        
        if (Runner.IsSharedModeMasterClient)
        {
            EndGame();
        }
    }
    
    private IEnumerator MoveApartToFloorSmooth(Transform targetFloor)
    {
        float speed = 15f;
        
        float targetY = -targetFloor.localPosition.y + FloorHeight;
        Vector3 startPos = ApartBase.position;
        Vector3 targetPos = new Vector3(startPos.x, targetY, startPos.z);
       
        float distance = Vector3.Distance(startPos, targetPos);
        
        if (distance < FloorHeight * 2)
        {
            speed = 25f;
        }
        
        float duration = distance / speed;
        float time = 0f;

        while (time < duration)
        {
            ApartBase.position = Vector3.Lerp(startPos, targetPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        ApartBase.position = targetPos;
    }
    
    private IEnumerator HighlightAnima(Transform targetFloor)
    {
        Vector3 originalScale = targetFloor.localScale;
        Vector3 targetScale = originalScale * 1.2f;
        float duration = 0.2f;
        
        Vector3 originalPosition = targetFloor.position;
        targetFloor.position += new Vector3(0, 0, -0.1f);

        float time = 0f;
        while (time < duration)
        {
            targetFloor.localScale = Vector3.Lerp(originalScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        targetFloor.localScale = targetScale;
        
        yield return new WaitForSeconds(1f);
        
        time = 0f;
        while (time < duration)
        {
            targetFloor.localScale = Vector3.Lerp(targetScale, originalScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        targetFloor.localScale = originalScale;
        targetFloor.position = originalPosition;
    }
    
    // 🔧 수정: EndGame() - RPC로 모든 클라이언트 초기화
    private void EndGame()
    {
        if (!Runner.IsSharedModeMasterClient) return;

        Debug.Log("[EndGame] 🎮 게임 종료 시작 - 모든 매니저 초기화");

        StopRealTimeFollow();

        // 🔥 RPC로 모든 클라이언트에서 층 삭제 및 초기화
        RPC_ResetAllManagers();

        // 🔥 마스터에서도 로컬 초기화
        ResetManager();

        // 게임 상태 변경
        MainGameSceneManager.GameStateManager.ChangeState_RPC(EMainGameState.Board);
    }
    // 🔧 추가: 모든 매니저 초기화 RPC
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ResetAllManagers()
    {
        Debug.Log("[RPC_ResetAllManagers] 🔄 모든 매니저 초기화 시작");

        // 1. 층 오브젝트 삭제
        ClearAllFloorsLocal();

        // 2. 각 매니저 초기화
        ResetManager();
    
        if (ApartTurnManager.Instance != null)
        {
            ApartTurnManager.Instance.ResetManager();
        }
    
        if (ApartInputManager.Instance != null)
        {
            ApartInputManager.Instance.ResetManager();
        }
    
        if (ApartPlayerManager.Instance != null)
        {
            ApartPlayerManager.Instance.ResetManager();
        }

        Debug.Log("[RPC_ResetAllManagers] ✅ 모든 매니저 초기화 완료");
    }

// 🔧 추가: 로컬 층 오브젝트 삭제 (모든 클라이언트에서 실행)
    private void ClearAllFloorsLocal()
    {
        Debug.Log($"[ClearAllFloorsLocal] 층 삭제 시작 - 총 {spawnedFloors.Count}개");

        // 생성된 모든 층 오브젝트 삭제
        for (int i = spawnedFloors.Count - 1; i >= 0; i--)
        {
            if (spawnedFloors[i] != null && spawnedFloors[i].gameObject != null)
            {
                Debug.Log($"[ClearAllFloorsLocal] 층 {i+1} 삭제: {spawnedFloors[i].name}");
                Destroy(spawnedFloors[i].gameObject);
            }
        }
    
        spawnedFloors.Clear();
        Debug.Log("[ClearAllFloorsLocal] ✅ 모든 층 삭제 완료");
    }

// 🔧 추가: GameManager 자체 초기화
    public void ResetManager()
    {
        Debug.Log("[ApartGameManager.ResetManager] 🔄 GameManager 초기화");
    
        // 네트워크 변수 초기화 (마스터에서만)
        if (Runner.IsSharedModeMasterClient)
        {
            CurrentFloorCount = 0;
            IsGameEnded = true;
            IsSequenceCompleted = false;
            CompletedSequenceLength = 0;
            IsRealTimeFollowing = false;
        }
    
        // 코루틴 정리
        if (realTimeFollowCoroutine != null)
        {
            StopCoroutine(realTimeFollowCoroutine);
            realTimeFollowCoroutine = null;
        }
    
        Debug.Log("[ApartGameManager.ResetManager] ✅ GameManager 초기화 완료");
    }

}