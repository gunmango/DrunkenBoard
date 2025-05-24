using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-900)]
public class ASceneManager<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] protected ESceneType sceneType = ESceneType.None;

    public static T Instance;
    
    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        
    }
}