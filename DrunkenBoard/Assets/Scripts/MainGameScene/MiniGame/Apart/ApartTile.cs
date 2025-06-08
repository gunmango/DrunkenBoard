using UnityEngine;

public class ApartTile : MonoBehaviour
{
    [SerializeField] public int _tileNumber;
    [SerializeField] private Vector3 _tilePosition;
    
    public int TileNumber 
    { 
        get => _tileNumber; 
        set => _tileNumber = value; 
    }
    
    public Vector3 TilePosition 
    { 
        get => _tilePosition; 
        set => _tilePosition = value; 
    }
    
    private void Start()
    {
        // 타일 위치를 현재 Transform 위치로 자동 설정
        _tilePosition = transform.position;
    }
    
    // 타일 초기화 메서드
    public void InitializeTile(int number, Vector3 position)
    {
        _tileNumber = number;
        _tilePosition = position;
        transform.position = position;
    }
}