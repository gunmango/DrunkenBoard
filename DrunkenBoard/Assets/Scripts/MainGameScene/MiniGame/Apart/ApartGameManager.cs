using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ApartGameManager : MonoBehaviour
{
    public static ApartGameManager Instance;
    
    public Transform ApartBase;
    public GameObject FloorPrefab;
    public float FloorHeight = 1.0f;
    
    private int _currentFloorCount;
    
    private void Awake()
    {
        Instance = this;
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public void AddFloor()
    {
        
        _currentFloorCount++;
        Vector3 floorPos = ApartBase.position + Vector3.up * (FloorHeight * (_currentFloorCount-1));
        GameObject newFloor = Instantiate(FloorPrefab,ApartBase);
        newFloor.transform.localPosition = floorPos;
        newFloor.transform.localRotation = Quaternion.identity;

        NumberingSprite display = newFloor.GetComponentInChildren<NumberingSprite>();
        if (display)
        {
            display.SetNumber(_currentFloorCount);
        }

    }

    public void AddRandomFloors(int floorCount)
    {
        for (int i = 0; i < floorCount; i++)
        {
            AddFloor();
        }
    }

    public void RaiseFloorsTo(int targetFloor, System.Action onComplete = null)
    {
        Debug.Log($"RaiseFloorsTo 호출됨. 목표층: {targetFloor}");
        StartCoroutine(RaiseFloorsCoroutine(targetFloor, onComplete));
    }

    private IEnumerator RaiseFloorsCoroutine(int targetFloor, System.Action onComplete)
    {
        if (_currentFloorCount >= targetFloor)
        {
            Debug.Log("이미 목표층 도달. 콜백 바로 실행!");
            onComplete?.Invoke();
            yield break;
        }

        while (_currentFloorCount < targetFloor)
        {
            AddFloor();
            yield return new WaitForSeconds(0.1f); // 층 쌓는 속도
        }

        // ✅ 한 프레임 기다리기 (등록 다 될 때까지)
        yield return null;

        Debug.Log("층 다 쌓음! 콜백 실행");
        onComplete?.Invoke();
    }

    public void HighlightFloor(int floorNumber)
    { 
        int index = floorNumber - 1;

        if (index < 0 || index >= ApartBase.childCount)
        {
            Debug.LogWarning($"[HighlightFloor] 유효하지 않은 층 번호: {floorNumber} (현재 층 수: {ApartBase.childCount})");
            return;
        }

        Transform targetFloor = ApartBase.GetChild(index);
        StartCoroutine(MovetoHighlignt(targetFloor));
    }
    private IEnumerator MovetoHighlignt(Transform targetFloor)
    {  
        yield return StartCoroutine(MoveApartToFloor(targetFloor));
        yield return StartCoroutine(HighlightAnima(targetFloor));
        
    }

    private IEnumerator HighlightAnima(Transform targetFloor)
    {
        Vector3 originalScale = targetFloor.localScale;
        Vector3 targetScale = originalScale * 1.2f;
        float time = 0f;
        float duration = 0.2f;
        
        Vector3 originalPosition = targetFloor.position;
        targetFloor.position += new Vector3(0, 0, -0.1f);

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
            targetFloor.localScale = Vector3.Lerp(targetScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        targetFloor.localScale = originalScale;
        
       // targetFloor.position = originalPosition;
        
    }

    private IEnumerator MoveApartToFloor(Transform targetFloor)
    {
       float speed = 10f;
       
        float targetY = -targetFloor.localPosition.y+FloorHeight;
        Vector3 startPos = ApartBase.position;
        Vector3 targetPos = new Vector3(startPos.x, targetY, startPos.z);  // y 위치 변경!
       
        
        float distance = Vector3.Distance(startPos, targetPos);
        float duration = distance/speed;
        float time = 0f;


        while (time < duration)
        {
            ApartBase.position = Vector3.Lerp(startPos, targetPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        ApartBase.position = targetPos;
    }
    
}
