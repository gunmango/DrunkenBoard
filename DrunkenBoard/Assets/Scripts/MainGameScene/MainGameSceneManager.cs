using System;
using UnityEngine;

public class MainGameSceneManager : ASceneManager<MainGameSceneManager>
{
    public Action ActInitialize { get; set; }
    
    [SerializeField] private WebCamManager webCamManager;
    public static WebCamManager WebCamManager => Instance.webCamManager;
    
    [SerializeField] private MiniGameManager miniGameManager;
    public static MiniGameManager MiniGameManager => Instance.miniGameManager;
    
    [SerializeField] private MainGameStateManager gameStateManager;
    public static MainGameStateManager GameStateManager => Instance.gameStateManager;
    
    protected void Start()
    {
        webCamManager.Initialize();
        ActInitialize?.Invoke();
    }
}
