using System;
using UnityEngine;

public class MainGameSceneManager : ASceneManager<MainGameSceneManager>
{
    public Action ActInitialize { get; set; }
    
    [SerializeField] private WebCamSetter webCamSetter;
    
    protected override void Initialize()
    {
        base.Initialize();
        webCamSetter.Initialize();
        ActInitialize?.Invoke();
    }
}
