using Fusion;
using TMPro;
using UnityEngine;

public class DemoTriangle : NetworkBehaviour
{
    [Networked] public bool Count { get; set; }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Count = !Count;
        }
    }
}
