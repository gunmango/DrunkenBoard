using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class CrocodilePlayer 
{
    public string PlayerName;
    public int PlayerOrder;
    public bool TurnPlayer;

    public CrocodilePlayer(string name)
    {
        PlayerName = name;
        TurnPlayer = false;
    }
}
