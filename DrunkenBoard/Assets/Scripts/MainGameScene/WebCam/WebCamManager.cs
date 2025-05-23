using System.Collections.Generic;
using Fusion;
using Unity.WebRTC;
using UnityEngine;
using System.Collections;

public class WebCamManager : SimulationBehaviour
{
    [SerializeField] private WebCamCanvasUpdater webCamCanvasUpdater;
    [SerializeField] private WebCamUnit selfWebCamUnit;
    [SerializeField] private WebCamUnit originalWebCamUnit;
    [SerializeField] private WebCamAnchoredPositionTable positionTable;
    
    private readonly List<WebCamUnit> _webCamUnits = new List<WebCamUnit>();

    private void OnEnable()
    {
        Debug.Log("onEnable");
    }
    public void Initialize()
    {
        Debug.Log("initialize");
        SetCamsToBoardView();

        GameManager.FusionSession.ActOnPlayerJoined += OnNewPlayer;
        
        GameManager.WebRtcController.OnVideoReceived += OnVideoReceived;
        GameManager.WebRtcController.OnVideoDisconnect += OnVideoDisconnect;

        MainGameSceneManager.MiniGameManager.OnMiniGameStart += MoveCamsToGameView;
        MainGameSceneManager.MiniGameManager.OnMiniGameEnd += MoveCamsToBoardView;
    }
    
    private WebCamUnit CreateUnit(NetworkRunner runner, PlayerRef playerRef)
    {
        Debug.Log("create unit");
        WebCamUnit webCamUnit;
        if (playerRef == runner.LocalPlayer)
        {
            webCamUnit = Instantiate(selfWebCamUnit, webCamCanvasUpdater.ContentTransform);
            webCamUnit.SetTrack(null);
        }
        else
        {
            webCamUnit = Instantiate(originalWebCamUnit, webCamCanvasUpdater.ContentTransform);
        }
        
        webCamUnit.gameObject.SetActive(true);
        _webCamUnits.Add(webCamUnit);
        
        SetCamsToBoardView();
        
        return webCamUnit;
    }
    
    private void OnNewPlayer(NetworkRunner runner, PlayerRef playerRef)
    {
        Debug.Log("OnNewPlayer");
        StartCoroutine(CreateUnitCo(runner, playerRef));
    }

    private IEnumerator CreateUnitCo(NetworkRunner runner, PlayerRef playerRef)
    {
        yield return new WaitUntil(()=> MainGameSceneManager.GameStateManager.IsSpawned);
        yield return new WaitUntil(()=> MainGameSceneManager.GameStateManager.CurrentState == EMainGameState.Board);
        
        CreateUnit(runner, playerRef);
    }
    
    
    private void OnVideoReceived(VideoStreamTrack track, string uuid)
    {
        
    }

    private void OnVideoDisconnect(string obj)
    {
        for (int i = 0; i < _webCamUnits.Count; i++)
        {
            if (_webCamUnits[i].Uuid == obj)
            {
                _webCamUnits[i].UnSetTrack();
                break;
            }
        }
    }
    
    
    
    
    #region 위치옮기기

    private void SetCamsToBoardView()
    {
        WebCamAnchoredPosition camAnchoredPosition = positionTable.GetAnchoredPosition(_webCamUnits.Count);
        
        for (int i = 0; i < _webCamUnits.Count; i++)
        {
            _webCamUnits[i].Mover.SetTo(camAnchoredPosition.BoardViewPositions[i]);
        }
    }

    private void MoveCamsToBoardView()
    {
        WebCamAnchoredPosition camAnchoredPosition = positionTable.GetAnchoredPosition(_webCamUnits.Count);
        
        for (int i = 0; i < _webCamUnits.Count; i++)
        {
            WebCamUnit unit = _webCamUnits[i];
            unit.Mover.MoveTween(camAnchoredPosition.BoardViewPositions[i], ()=>
            {
                unit.ShowItemSocket();
            });
        }
    }
    
    private void MoveCamsToGameView()
    {
        WebCamAnchoredPosition camAnchoredPosition = positionTable.GetAnchoredPosition(_webCamUnits.Count);
        
        for (int i = 0; i < _webCamUnits.Count; i++)
        {
            _webCamUnits[i].Mover.MoveTween(camAnchoredPosition.GameViewPositions[i]);
            _webCamUnits[i].HideItemSocket();
        }
    }
    
    #endregion
    
}
