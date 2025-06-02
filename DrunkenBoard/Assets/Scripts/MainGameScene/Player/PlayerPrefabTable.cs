using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "PlayerPrefabTable", menuName = "Player Prefab Table")]
public class PlayerPrefabTable : ScriptableObject
{
    [System.Serializable]
    private struct Entry
    {
        public EPlayerColor Key;
        public PlayerColorSet Data;
    }
    
    [SerializeField] private List<Entry> entries = new List<Entry>();
    
    private Dictionary<EPlayerColor, PlayerColorSet> _lookup;
    
    private void OnEnable()
    {
        // 중복 키 체크 포함
        _lookup = entries
            .GroupBy(e => e.Key)
            .ToDictionary(
                g => g.Key,
                g =>
                {
                    if (g.Count() > 1)
                        Debug.LogWarning($"ItemTableSO: 중복된 키가 있습니다 → {g.Key}");
                    return g.First().Data;
                }
            );
    }
    
    /// <summary>
    /// 키에 해당하는 데이터를 가져옵니다. 없으면 하얀색
    /// </summary>
    public PlayerColorSet GetColorSet(EPlayerColor key)
    {
        if (_lookup == null)
            OnEnable();

        if (_lookup.TryGetValue(key, out var value))
        {
            return value;
        }
        
        return _lookup[EPlayerColor.White];
    }

    public PlayerPiece GetPiece(EPlayerColor key)
    {
        return GetColorSet(key).PlayerPiece;
    }
}
