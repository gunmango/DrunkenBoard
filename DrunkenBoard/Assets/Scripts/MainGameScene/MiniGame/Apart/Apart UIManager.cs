using System;
using UnityEngine;
using UnityEngine.UI;

public class ApartUIManager : MonoBehaviour
{
    public static ApartUIManager Instance;
    public Button startButton;
    

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        startButton.onClick.AddListener(()=>ApartGameManager.Instance.StartGame());
    }

    public void SetHost(bool isHost)
    {
        //isHost = isHost;
        startButton.gameObject.SetActive(isHost);
    }
}
