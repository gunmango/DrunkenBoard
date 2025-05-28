using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerManager : NetworkBehaviour
{
    #region 싱글톤

    public static PlayerManager Instance;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);  
        
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        GameManager.FusionSession.ActOnPlayerLeft += OnPlayerLeft;
    }

    #endregion

    [SerializeField] private PlayerPrefabTable playerPrefabTable;
    public static PlayerPrefabTable Table => Instance.playerPrefabTable;
    
    [Networked, Capacity(8)] public NetworkLinkedList<Player> Players => default;

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void AddPlayer_RPC(Player player)
    {
        Players.Add(player);
    }

    public EPlayerColor GetPlayerColor(int uuid)
    {
        foreach (var player in Players)
        {
            if (player.Uuid == uuid)
                return player.PlayerColor;
        }
        
        return EPlayerColor.None;
    }
    
    private void OnPlayerLeft(NetworkRunner arg1, PlayerRef arg2)
    {
        Players.Remove(null);    
    }
}
