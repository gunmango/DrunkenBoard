using System;
using UnityEngine;

public class MainGameSceneManager : ASceneManager<MainGameSceneManager>
{
    public Action ActInitialize { get; set; }

    protected override void Initialize()
    {
        base.Initialize();
        ActInitialize?.Invoke();
    }
}
