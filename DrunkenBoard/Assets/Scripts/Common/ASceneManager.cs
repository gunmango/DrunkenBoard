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

    /// <summary>
    /// 매니저 씬이 로드 안되어있다면 로드
    /// </summary>
    protected virtual void Start()
    { 
        if (IsSceneLoaded("ManagerScene") == false)
        {
            StartCoroutine(LoadManagerSceneAsync());
            return;
        }
        Initialize();
    }

    /// <summary>
    /// ManagerScene 이 필요한 초기화 작업
    /// </summary>
    protected virtual void Initialize()
    {
        
    }
    
    private static bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName)
                return true;
        }
        return false;
    }

    private IEnumerator LoadManagerSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync((int)ESceneType.Manager, LoadSceneMode.Additive);
        
        // 씬 로드 도중에는 isDone이 false
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        GameManager.SceneController.SetScene(sceneType);
        Initialize();
    }
}