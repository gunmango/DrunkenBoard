using System;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameManager : MonoBehaviour
{
    public Action OnMiniGameStart { get; set; }
    public Action OnMiniGameEnd { get; set; }

    public bool IsPlaying { get; private set; } = false;
    
    [SerializeField] private Button demoMiniGameButton;
    
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

    private void Start()
    {
        demoMiniGameButton.onClick.AddListener(() =>
        {
            if (IsPlaying)
            {
                EndMiniGame();
            }
            else
            {
                StartMiniGame();
            }
        });
    }
    
}
