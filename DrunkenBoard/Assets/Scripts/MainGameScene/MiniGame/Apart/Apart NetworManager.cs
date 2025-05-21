using System.Collections.Generic;
using UnityEngine;

public class ApartNetworkManager : MonoBehaviour
{
    public static ApartNetworkManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SendInputToHost(string playerId)
    {
        
    }

    public void BroadcastInputToHost(List<string> inputOrder)
    {
        
    }
}
