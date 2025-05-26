using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using static ApartTurnManager;

public class ApartPlayerManager : MonoBehaviour
{
    public static ApartPlayerManager Instance;
    public List<ApartPlayer> players;
    private int currentPlayerIndex = 0;
    
    public Button startButton;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        startButton.gameObject.SetActive(true);
        startButton.onClick.AddListener(onStartButtonClicked);
    }
    
    public ApartPlayer GetCurrentPlayer() => players[currentPlayerIndex];

    public void PlayerReachedTile()
    {
        startButton.gameObject.SetActive(true);
    }

    private void onStartButtonClicked()

    {
        startButton.gameObject.SetActive(false);
        ApartTurnManager.Instance.StartTurn();
        
        Debug.Log($"플레이어 {currentPlayerIndex + 1} 턴 시작");
    }

    public void NextPlayer()
    {
        currentPlayerIndex=(currentPlayerIndex + 1) % players.Count;
        Debug.Log($"다음 플레이어 턴: {currentPlayerIndex + 1}");
    }

    public Color GetPlayerColor(int playerName)
    {
        var player = players.Find(p => p.playerName == name);
        return player != null ? player.PlayerColor : Color.gray;
    }

}
