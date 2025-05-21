using System.Collections;
using UnityEngine;

public class TitleSceneLoader : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;
        
        if (GameManager.SceneController.CurrentScene == ESceneType.None)
        {
            GameManager.SceneController.LoadTitle();
        }
        Destroy(this);
    }
}
