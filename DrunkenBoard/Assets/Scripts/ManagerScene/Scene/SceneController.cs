using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    private readonly List<AsyncOperation> _loadingOperations = new List<AsyncOperation>();
    
    public ESceneType CurrentScene { get; private set; } = ESceneType.None;
    
    public void LoadTitle()
    {
        SceneManager.LoadSceneAsync((int)ESceneType.Title, LoadSceneMode.Additive);
        CurrentScene = ESceneType.Title;
    }
    
    public void LoadScene(ESceneType scene)
    {
        _loadingOperations.Add(SceneManager.UnloadSceneAsync((int)CurrentScene));
        _loadingOperations.Add(SceneManager.LoadSceneAsync((int)scene, LoadSceneMode.Additive));
        StartCoroutine(GetSceneLoading(scene));
    }

    /// <summary>
    /// 매니저씬 처음 로드될때 현재씬 저장
    /// </summary>
    /// <param name="scene"></param>
    public void SetScene(ESceneType scene)
    {
        CurrentScene = scene;
    }
    
    private IEnumerator GetSceneLoading(ESceneType scene)
    {

        for (int i = 0; i < _loadingOperations.Count; i++)
        {
            while (!_loadingOperations[i].isDone)
            {
                // 나중에 로딩하는데 오래 걸린다면
                yield return null;
            }
        }
        
        CurrentScene = scene;
        // 로딩 끝
    }
}
