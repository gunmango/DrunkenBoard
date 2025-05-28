using System.Collections.Generic;
using UnityEngine;

public class Space : MonoBehaviour
{
    [SerializeField] private SpriteRendererTweener spriteTweener;
    [SerializeField] private List<Transform> PieceSpots;
    public SpriteRendererTweener SpriteTweener => spriteTweener;
    
    public ESpaceEventType SpaceEventType { get; set; } = ESpaceEventType.None;
    
    public Vector3 GetPieceSpotPos(int index)
    {
        return PieceSpots[index].position;
    }

    public int GetSpotCount()
    {
        return PieceSpots.Count;
    }
}
