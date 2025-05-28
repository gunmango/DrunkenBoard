using System;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameManager : MonoBehaviour
{
    public Action OnMiniGameStart { get; set; }
    public Action OnMiniGameEnd { get; set; }

    public bool IsPlaying { get; private set; } = false;
    
    public void StartMiniGame()
    {
        IsPlaying = true;
        OnMiniGameStart?.Invoke();
    }

    public void EndMiniGame()
    {
        IsPlaying = false;
        OnMiniGameEnd?.Invoke();
    }
}
