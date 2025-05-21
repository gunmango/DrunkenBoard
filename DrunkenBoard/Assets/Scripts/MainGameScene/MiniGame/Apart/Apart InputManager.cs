using System.Collections.Generic;
using UnityEngine;

public class ApartInputManager : MonoBehaviour
{
    public static ApartInputManager instance;

    private Dictionary<string, int> inputCounts = new();
    public List<string> inputOrder = new();

    public int maxInputsPerPlayer = 2;

    private void Awake()
    {
        instance = this;
    }

    public void StartInputPhase()
    {
        inputOrder.Clear();
        inputCounts.Clear();
    }

    private void Update()
    {
        if(ApartGameManager.Instance.CurrentState != EGameState.Input) return;
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            string myId = "MyPlayerID";//네크워크에서 받아오기
            
            if(!inputCounts.ContainsKey(myId)) inputCounts[myId] = 0;

            if (inputCounts[myId] < maxInputsPerPlayer)
            {
                inputCounts[myId]++;
                inputOrder.Add(myId);
                ApartNetworkManager.Instance.SendInputToHost(myId);
            }
        }
    }

    public bool IsInputFinished()
    {
        return false;
    }
}
