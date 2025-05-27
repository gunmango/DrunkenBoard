using System;
using System.Collections;
using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [Networked] public int Uuid { get; set; }
    [Networked] public EPlayerColor PlayerColor { get; set; }

    [Networked] public int BoardCycleCount { get; set; }
    [Networked] public int DrinkCount { get; set; }
    [Networked] public int ItemCount { get; set; }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            Array colors = Enum.GetValues(typeof(EPlayerColor));
            PlayerColor = (EPlayerColor)colors.GetValue(UnityEngine.Random.Range(0, colors.Length));

            StartCoroutine(AddSelfCo());
        }
    }

    private IEnumerator AddSelfCo()
    {
        yield return new WaitUntil(() => PlayerManager.Instance.Object);
        yield return new WaitUntil(() => PlayerManager.Instance.Object.IsValid);
        PlayerManager.Instance.AddPlayer_RPC(this);
    }
}
