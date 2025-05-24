using Fusion;
using TMPro;
using UnityEngine;

public class DemoTriangle : NetworkBehaviour
{
    [Networked] public int Count { get; set; }

    public override void FixedUpdateNetwork()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Count++;
        }
    }
}
