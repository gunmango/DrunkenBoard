using System.Collections;
using UnityEngine;

public class BoardGamePlayer : ATurnPlayer
{
    protected override IEnumerator TakeTurn()
    {
        Debug.Log($"{Uuid} is taking turn");
        yield return new WaitForSeconds(5f);
        Debug.Log($"{Uuid} turn ended");
        EndTurn();
    }
}
