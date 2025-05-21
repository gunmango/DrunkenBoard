using System.Collections.Generic;
using UnityEngine;

public class ApartPlayerManager : MonoBehaviour
{
    public static ApartPlayerManager Instance;

    private Dictionary<string, Color> playerColors = new();
    
    public bool isHost = false;

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterPlayer(string playerId, Color color)
    {
        if (!playerColors.ContainsKey(playerId))
            playerColors.Add(playerId, color);
    }

    public Color GetColor(string playerId)
    {
        return playerColors.ContainsKey(playerId) ? playerColors[playerId] : Color.white;
    }

    public void ArrivedatPlayer()
    {
        ApartGameManager.Instance.SetHostPlayer(this);
    }
}
