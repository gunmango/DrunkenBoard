using UnityEngine;

public class DevGameSceneHelper : MonoBehaviour
{
    [SerializeField] private int totalPlayerCount = 8;

    private void Awake()
    {
        MainGameSceneManager.Instance.ActInitialize += Initialize;
    }

    private void Start()
    {
        //임의로 플레이어 만들기
        for (int i = 0; i < totalPlayerCount - 1; i++)
        {
            //MainGameSceneManager.WebCamManager.CreateUnit(i.ToString());
        }
    }

    private void Initialize()
    {

    }
}
