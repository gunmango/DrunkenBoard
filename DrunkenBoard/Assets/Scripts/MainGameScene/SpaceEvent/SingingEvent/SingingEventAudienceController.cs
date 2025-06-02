using Fusion;
using UnityEngine;
using System.Collections;

public class SingingEventAudienceController : NetworkBehaviour
{
    [Networked, Capacity(8)] public NetworkLinkedList<SingingEventAudience> Audiences => default;

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Initialize_RPC()
    {
        Audiences.Clear();
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void AddAudience_RPC(SingingEventAudience audience)
    {
        Audiences.Add(audience);
    }

    public void StartVoting(int totalPlayerCount)
    {
        StartCoroutine(PlayersTakeAction(totalPlayerCount));
    }
    
    private IEnumerator PlayersTakeAction(int totalPlayerCount)
    {
        yield return new WaitUntil(() => Audiences.Count == totalPlayerCount);
        
        foreach (var audience in Audiences)
        {
            audience.StartVoting_RPC();
        }  
    }

    public void StartCountDown()
    {
        foreach (var audience in Audiences)
        {
            audience.StartCountDown_RPC();
        }
    }
}
