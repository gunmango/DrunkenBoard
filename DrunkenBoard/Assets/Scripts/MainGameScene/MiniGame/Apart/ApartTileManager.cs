using System.Collections.Generic;
using UnityEngine;

public class ApartTileManager : MonoBehaviour
{
    public List<ApartTile> tiles;

    public ApartTile GetTileAtPosition(Vector3 pos)
    {
        foreach (var tile in tiles)
        {
            if (Vector3.Distance(tile.transform.position, pos) < 0.1f)
                return tile;
        }
        
        return null;
    }
    
    // 타일 번호로 타일 찾기 메서드 추가
    public ApartTile GetTileByNumber(int tileNumber)
    {
        foreach (var tile in tiles)
        {
            if (tile._tileNumber == tileNumber)
                return tile;
        }
        return null;
    }
}