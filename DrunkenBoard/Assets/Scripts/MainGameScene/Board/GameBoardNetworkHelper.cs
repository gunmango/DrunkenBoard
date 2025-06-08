using System.Collections;
using Fusion;
using UnityEngine;
using System.Collections.Generic;

public class GameBoardNetworkHelper : NetworkBehaviour
{
    [SerializeField] private MainGameBoard mainGameBoard;
    [Networked, Capacity(20)] public NetworkLinkedList<int> EventIndexes => default;
    
    public override void Spawned()
    {
        Initialize();
    }

    private void Initialize()
    {
        List<int> eventList = new List<int>();
        
        if (Object.HasStateAuthority == false)
        {
            if (EventIndexes.Count != 20)
            {
                StartCoroutine(WaitUntilIndexesToBeInitialized());
                return;
            }
            foreach (int index in EventIndexes)
            {
                eventList.Add(index);
            }
            mainGameBoard.SetEvents(eventList);
            return;
        }
        
        List<ASpaceEvent> spaceEvents = MainGameSceneManager.SpaceEventManager.SpaceEvents;

        for (int i = 0; i < mainGameBoard.GetSpaceCount; i++)
        {
            EventIndexes.Add(Random.Range(0, spaceEvents.Count));
        }
        
        foreach (int index in EventIndexes)
        {
            eventList.Add(index);
        }
        mainGameBoard.SetEvents(eventList);
    }

    private IEnumerator WaitUntilIndexesToBeInitialized()
    {
        yield return new WaitUntil(()=> EventIndexes.Count == 20);
        
        List<int> eventList = new List<int>();
        foreach (int index in EventIndexes)
        {
            eventList.Add(index);
        }
        mainGameBoard.SetEvents(eventList);
    }
}
