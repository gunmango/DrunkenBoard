using System;
using UnityEngine;

public class MainGameSceneManager : ASceneManager<MainGameSceneManager>
{
    public Action ActInitialize { get; set; }
    
    [SerializeField] private WebCamManager webCamManager;
    public static WebCamManager WebCamManager => Instance.webCamManager;
    
    [SerializeField] private MiniGameManager miniGameManager;
    public static MiniGameManager MiniGameManager => Instance.miniGameManager;
    
    protected override void Initialize()
    {
        base.Initialize();
        webCamManager.Initialize();
        ActInitialize?.Invoke();
    }
}
