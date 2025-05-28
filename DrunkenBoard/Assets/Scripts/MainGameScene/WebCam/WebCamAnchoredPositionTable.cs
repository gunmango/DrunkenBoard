using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WebCamAnchoredPosition", menuName = "WebCamAnchoredPosition Table")]
public class WebCamAnchoredPositionTable : ScriptableObject
{
    public List<WebCamAnchoredPosition> Positions;

    public WebCamAnchoredPosition GetAnchoredPosition(int playerCount)
    {
        return Positions.Find(x => x.PlayerCount == playerCount);
    }
}
