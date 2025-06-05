using System;
using ExitGames.Client.Photon;
using UnityEngine;

public class MainGameSceneManager : ASceneManager<MainGameSceneManager>
{
    public Action ActInitialize { get; set; }
    
    [SerializeField] private WebCamManager webCamManager;
    public static WebCamManager WebCamManager => Instance.webCamManager;
    
    [SerializeField] private MiniGameManager miniGameManager;
    public static MiniGameManager MiniGameManager => Instance.miniGameManager;
    
    [SerializeField] private MainGameStateManager gameStateManager;
    public static MainGameStateManager GameStateManager => Instance.gameStateManager;
    
    [SerializeField] private MainGameBoard mainGameBoard;
    public static MainGameBoard Board => Instance.mainGameBoard;
    
    [SerializeField] private SpaceEventManager spaceEventManager;
    public static SpaceEventManager SpaceEventManager => Instance.spaceEventManager;
    
    [SerializeField] private EventReadyPopup eventReadyPopup;
    public static EventReadyPopup EventReadyPopup => Instance.eventReadyPopup;
    
    protected void Start()
    {
        webCamManager.Initialize();
        ActInitialize?.Invoke();
    }

    public void OpenSpaceEventReadyPopup(EventReadyPopupData popupData)
    {
        GameManager.PopupManager.OpenPopup(EventReadyPopup, popupData);
    }
}
