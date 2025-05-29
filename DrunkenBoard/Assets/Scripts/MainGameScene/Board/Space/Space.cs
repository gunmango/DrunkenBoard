using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Space : MonoBehaviour
{
    [SerializeField] private SpriteRendererTweener spriteTweener;
    [SerializeField] private List<Transform> pieceSpots;
    [SerializeField] private TextMeshPro eventNameText;
    public SpriteRendererTweener SpriteTweener => spriteTweener;
    
    public ESpaceEventType SpaceEventType { get; private set; } = ESpaceEventType.None;
    
    public Vector3 GetPieceSpotPos(int index)
    {
        return pieceSpots[index].position;
    }

    public int GetSpotCount()
    {
        return pieceSpots.Count;
    }

    public void SetEvent(ESpaceEventType eventType, string eventName)
    {
        SpaceEventType = eventType;
        eventNameText.text = eventName;
    }
}
