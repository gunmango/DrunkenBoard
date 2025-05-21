using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WebCamAnchoredPosition", menuName = "Create New WebCamAnchoredPosition Table")]
public class WebCamAnchoredPositionTable : ScriptableObject
{
    public List<WebCamAnchoredPosition> Positions;

    public WebCamAnchoredPosition GetAnchoredPosition(int playerCount)
    {
        return Positions.Find(x => x.PlayerCount == playerCount);
    }
}
