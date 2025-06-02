using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "WebCamAnchoredPosition", menuName = "WebCamAnchoredPosition Table")]
public class WebCamAnchoredPositionTable : ScriptableObject
{
    public List<WebCamAnchoredPosition> Positions;
    public WebCamSize OriginalSize;
    public WebCamSize OnStageSize;
    public Vector2 OnStageAnchoredPosition;
    public WebCamAnchoredPosition GetAnchoredPosition(int playerCount)
    {
        return Positions.Find(x => x.PlayerCount == playerCount);
    }
}
