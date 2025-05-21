using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WebCamAnchoredPosition
{
    public int PlayerCount;
    
    public List<Vector2> BoardViewPositions;
    public List<Vector2> GameViewPositions;
}
