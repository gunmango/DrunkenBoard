using System.Collections.Generic;
using UnityEngine;

public class Space : MonoBehaviour
{
    [SerializeField] private SpriteRendererTweener spriteTweener;
    [SerializeField] private List<Transform> PieceSpots;
    public SpriteRendererTweener SpriteTweener => spriteTweener;

    public Vector3 PieceSpot(int index)
    {
        return PieceSpots[index].position;
    }
}
