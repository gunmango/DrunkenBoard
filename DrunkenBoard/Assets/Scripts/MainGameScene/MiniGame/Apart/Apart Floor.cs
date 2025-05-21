using UnityEngine;

public class ApartFloor : MonoBehaviour
{
    public int floorNumber;
    public string playerId;

    public void Initialize(int number, string firstPlayer)
    {
        floorNumber = number;
        playerId = firstPlayer;
    }
}
