using Fusion;
using UnityEngine;
using System.Collections;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    [field: SerializeField] public Player PlayerPrefab { get; private set; }


    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            StartCoroutine(SpawnRoutine(player));
        }
    }

    private IEnumerator SpawnRoutine(PlayerRef playerRef)
    {
        yield return new WaitUntil(() => GameManager.Instance != null);
        
        yield return Runner.SpawnAsync(
            prefab: PlayerPrefab,
            position: Vector3.zero,
            inputAuthority: playerRef,
            onCompleted: res =>
            {
                if (res.IsSpawned)
                {
                    var spawnedPlayer = res.Object.GetComponent<Player>();
                    if (spawnedPlayer.Object.HasStateAuthority)
                    {
                        spawnedPlayer.Uuid = playerRef.RawEncoded;
                        spawnedPlayer.PlayerName = GameManager.Instance.LocalPlayerName;
                        spawnedPlayer.PlayerColor = GameManager.Instance.LocalPlayerColor;
                    }
                    
                    Runner.SetPlayerObject(Runner.LocalPlayer, res.Object);
                }
            }
            );
    }
}
