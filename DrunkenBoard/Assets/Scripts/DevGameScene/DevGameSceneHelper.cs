using UnityEngine;
using UnityEngine.UI;

public class DevGameSceneHelper : MonoBehaviour
{
    [SerializeField] private Button miniGameStartButton;
    private void Awake()
    {
        MainGameSceneManager.Instance.ActInitialize += Initialize;
    }
    
    private void Initialize()
    { ;
    }
}
