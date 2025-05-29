using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Space : MonoBehaviour
{
    [SerializeField] private SpriteRendererTweener spriteTweener;
    [SerializeField] private List<Transform> pieceSpots;
    [SerializeField] private TextMeshPro eventName;
    public SpriteRendererTweener SpriteTweener => spriteTweener;
    
    public ESpaceEventType SpaceEventType { get; set; } = ESpaceEventType.None;
    
    public Vector3 GetPieceSpotPos(int index)
    {
        return pieceSpots[index].position;
    }

    public int GetSpotCount()
    {
        return pieceSpots.Count;
    }
}
