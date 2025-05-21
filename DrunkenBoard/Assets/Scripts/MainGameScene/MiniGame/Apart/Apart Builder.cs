using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ApartBuilder : MonoBehaviour
{
    public static ApartBuilder Instance;
    
    public GameObject ApartPrefab;
    public float floorH = 2f;
    
    private int currentFloor = 0;
    private List<GameObject> allfloors = new();
    public List<GameObject> FloorList => new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    public void BuildFromInput()
    {
        var order = ApartInputManager.instance.inputOrder;

        foreach (var playerId in order)
        {
            BuildFloor(playerId);
        }

        ApartGameManager.Instance.CurrentState = EGameState.Selscting;
    }

    private void BuildFloor(string playerId)
    {
        Vector3 pos = new Vector3(0, currentFloor*floorH, 0);
        GameObject floor= Instantiate(ApartPrefab, pos, Quaternion.identity);
        floor.GetComponent<SpriteRenderer>().color = ApartPlayerManager.Instance.GetColor(playerId);

        var floorComp = floor.GetComponent<ApartFloor>();
        floorComp.Initialize(currentFloor + 1, playerId);
        
        allfloors.Add(floor);
        currentFloor++;
    }
    
    public void HighlightFloor(int floorNumber)
    {
        if (floorNumber <= 0 || floorNumber > FloorList.Count)
            return;

        // 모든 층 scale 초기화
        foreach (var f in allfloors)
        {
            f.transform.localScale = Vector3.one;
        }

        // 해당 층만 확대
        foreach (var singleFloor in allfloors)
        {
            var comp = singleFloor.GetComponent<ApartFloor>();
            if (comp.floorNumber == floorNumber)
            {
                singleFloor.transform.localScale = Vector3.one * 1.2f;
                // singleFloor.transform.DOScale(Vector3.one * 1.2f, 0.2f).SetEase(Ease.OutBack);
                break;
            }
        }
    }
    public void ReshuffleFloors(int selectedFloor)
    {
        int totalFloors = FloorList.Count;

        if (selectedFloor <= totalFloors)
        {
            HighlightFloor(selectedFloor);
            return;
        }

        int moveUpCount = selectedFloor - totalFloors;

        // 리스트에서 아래서 moveUpCount개 추출
        List<GameObject> bottomFloors = FloorList.Take(moveUpCount).ToList();

        // 리스트에서 제거하고 맨 뒤에 추가
        FloorList.RemoveRange(0, moveUpCount);
        FloorList.AddRange(bottomFloors);

        // 아파트 위치 재정렬
        for (int i = 0; i < FloorList.Count; i++)
        {
            Vector3 newPos = new Vector3(0, i * floorH, 0);
            FloorList[i].transform.position = newPos;
        }

        HighlightFloor(selectedFloor);
    }

}
