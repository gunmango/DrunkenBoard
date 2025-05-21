using System;
using UnityEngine;

public class MainGameSceneManager : ASceneManager<MainGameSceneManager>
{
    public Action ActInitialize { get; set; }
    
    [SerializeField] private WebCamManager webCamManager;
    public static WebCamManager WebCamManager => Instance.webCamManager;
    
    protected override void Initialize()
    {
        base.Initialize();
        webCamManager.Initialize();
        ActInitialize?.Invoke();
    }
}
