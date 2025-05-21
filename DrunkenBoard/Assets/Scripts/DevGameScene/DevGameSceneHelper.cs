using UnityEngine;

public class DevGameSceneHelper : MonoBehaviour
{
    //[SerializeField] private int playerCount = 8;

    private void Awake()
    {
        MainGameSceneManager.Instance.ActInitialize += Initialize;
    }

    private void Initialize()
    {
        //임의로 플레이어 만들기
        
    }
}
