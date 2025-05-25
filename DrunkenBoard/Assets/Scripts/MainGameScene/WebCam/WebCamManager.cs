using System.Collections.Generic;
using Fusion;
using Unity.WebRTC;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class WebCamManager : SimulationBehaviour
{
    [SerializeField] private WebCamCanvasUpdater webCamCanvasUpdater;
    [SerializeField] private WebCamUnit selfWebCamUnit;
    [SerializeField] private WebCamUnit originalWebCamUnit;
    [SerializeField] private WebCamAnchoredPositionTable positionTable;
    
    private readonly List<WebCamUnit> _webCamUnits = new List<WebCamUnit>();
    
    public void Initialize()
    {
        SetCamsToBoardView();

        GameManager.FusionSession.ActOnPlayerJoined += OnNewPlayer;
        GameManager.FusionSession.ActOnPlayerLeft += OnPlayerLeft;
        
        GameManager.WebRtcController.OnVideoReceived += OnVideoReceived;
        GameManager.WebRtcController.OnVideoDisconnect += OnVideoDisconnect;

        MainGameSceneManager.MiniGameManager.OnMiniGameStart += MoveCamsToGameView;
        MainGameSceneManager.MiniGameManager.OnMiniGameEnd += MoveCamsToBoardView;
    }



    private WebCamUnit CreateUnit(NetworkRunner runner, PlayerRef playerRef)
    {
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
        
        webCamUnit.SetUuid(playerRef.RawEncoded);
        webCamUnit.gameObject.SetActive(true);
        _webCamUnits.Add(webCamUnit);
        
        SetCamsToBoardView();
        
        return webCamUnit;
    }
    
    private void OnNewPlayer(NetworkRunner runner, PlayerRef playerRef)
    {
        StartCoroutine(CreateUnitCo(runner, playerRef));
    }

    private IEnumerator CreateUnitCo(NetworkRunner runner, PlayerRef playerRef)
    {
        yield return new WaitUntil(()=> MainGameSceneManager.GameStateManager.IsSpawned);
        yield return new WaitUntil(()=> MainGameSceneManager.GameStateManager.CurrentState == EMainGameState.Board);
        
        CreateUnit(runner, playerRef);
    }

    private void OnPlayerLeft(NetworkRunner arg1, PlayerRef arg2)
    {
        for (int i = 0; i < _webCamUnits.Count; i++)
        {
            if (_webCamUnits[i].Uuid == arg2.RawEncoded)
            {
                _webCamUnits[i].UnSetTrack();
                Destroy(_webCamUnits[i].gameObject);
                _webCamUnits.RemoveAt(i);
                SetCamsToBoardView();
                break;
            }
        }
    }
    
    private void OnVideoReceived(VideoStreamTrack track, string uuid)
    {
        foreach (WebCamUnit webCamUnit in _webCamUnits)
        {
            if (webCamUnit.Uuid.ToString() == uuid)
            {
                webCamUnit.SetTrack(track);
                break;
            }
        }
    }

    private void OnVideoDisconnect(string obj)
    {
        for (int i = 0; i < _webCamUnits.Count; i++)
        {
            if (_webCamUnits[i].Uuid.ToString() == obj)
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
