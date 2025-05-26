using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ApartGameManager : MonoBehaviour
{
    public static ApartGameManager Instance;
    
    public Transform ApartBase;
    public GameObject FloorPrefab;
    public float FloorHeight = 1.0f;
    
    private int currentFloorCount ;
    
    private void Awake()
    {
        Instance = this;
    }

    public void AddFloor()
    {
        
        currentFloorCount++;
        Debug.Log("확인용");
        Vector3 floorPos = ApartBase.position + Vector3.up * FloorHeight*(currentFloorCount-1);
        GameObject newFloor = Instantiate(FloorPrefab, floorPos, Quaternion.identity,ApartBase);
        
        
        // 층 번호 표시 (예: 텍스트 컴포넌트에 층 번호 세팅)
        Text floorNumText = newFloor.GetComponentInChildren<Text>();
        if(floorNumText != null)
            floorNumText.text = currentFloorCount.ToString();
    }

    public void AddRandomFloors(int floorCount)
    {
        for (int i = 0; i < floorCount; i++)
        {
            AddFloor();
        }
    }

    public void RaiseFloorsTo(int targetFloor)
    {
        StartCoroutine(RaiseFloorsCoroutine(targetFloor));
    }

    private IEnumerator RaiseFloorsCoroutine(int targetFloor)
    {
        while (currentFloorCount<targetFloor)
        {
            AddFloor();
            yield return new WaitForSeconds(0.1f);//층올리는 속도
        }
    }

    public void HighlightFloor(int floorNumber)
    {
        Transform targetFloor = ApartBase.GetChild(floorNumber-1);
        if (targetFloor != null)
        {
            StartCoroutine(HighlightAnima(targetFloor));
        }
    }

    private IEnumerator HighlightAnima(Transform targetFloor)
    {
        Vector3 originalScale = targetFloor.localScale;
        Vector3 targetScale = originalScale * 1.2f;
        float time = 0f;
        float duration = 0.2f;

        while (time < duration)
        {
            targetFloor.localScale = Vector3.Lerp(originalScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        targetFloor.localScale = targetScale;
        
        yield return new WaitForSeconds(0.2f);
        
        time = 0f;
        while (time < duration)
        {
            targetFloor.localScale = Vector3.Lerp(targetScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        targetFloor.localScale = originalScale;
    }
    
}
