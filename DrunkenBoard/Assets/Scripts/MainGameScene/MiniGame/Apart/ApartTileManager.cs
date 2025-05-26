using System.Collections.Generic;
using UnityEngine;

public class ApartTileManager : MonoBehaviour
{
  public  List<ApartTile> tiles;

  public ApartTile GetTileAtPosition(Vector3 pos)
  {
    foreach (var tile in tiles)
    {
      if(Vector3.Distance(tile.transform.position, pos) < 0.1f)
        return tile;
    }
    
    return null;
  }
  
}
