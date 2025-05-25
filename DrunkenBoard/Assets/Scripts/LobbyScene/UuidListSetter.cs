using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UuidListSetter : MonoBehaviour
{
    [SerializeField] private UuidListUpdater updater;

    private readonly List<TextMeshProUGUI> uuidList = new List<TextMeshProUGUI>();
    
    public void Initialize()
    {
        var uuid = GameManager.SignalingClient.Uuid;
        //uuidList.Add(updater.CreateUuidText(uuid));
    }

    public void AddNewClient(string uuid)
    {
        uuidList.Add(updater.CreateUuidText(uuid));
    }

    public void RemoveClient(string removeUuid)
    {
        for(int i = 0; i<uuidList.Count; i++)
        {
            if (uuidList[i].text == removeUuid)
            {
                uuidList.RemoveAt(i);
                break;
            }
        }
    }
}
