using UnityEngine;

[System.Serializable]
public class Player 
{
    public string PlayerName;
    public int PlayerOrder;
    public bool HasPlayer;

    public Player(string name)
    {
        PlayerName = name;
        HasPlayer = false;
    }
}
