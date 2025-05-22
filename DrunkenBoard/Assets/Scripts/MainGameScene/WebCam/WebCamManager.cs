using System.Collections.Generic;
using Unity.WebRTC;
using UnityEngine;

public class WebCamManager : MonoBehaviour
{
    [SerializeField] private WebCamCanvasUpdater webCamCanvasUpdater;
    [SerializeField] private WebCamUnit selfWebCamUnit;
    [SerializeField] private WebCamUnit originalPrefab;
    [SerializeField] private WebCamAnchoredPositionTable _positionTable;
    
    private readonly List<WebCamUnit> _webCamUnits = new List<WebCamUnit>();
    
    public void Initialize()
    {
        _webCamUnits.Add(selfWebCamUnit);
        SetCamsToBoardView();

        GameManager.WebRtcController.OnVideoReceived += OnVideoReceived;
        GameManager.WebRtcController.OnVideoDisconnect += OnVideoDisconnect;

        MainGameSceneManager.MiniGameManager.OnMiniGameStart += MoveCamsToGameView;
        MainGameSceneManager.MiniGameManager.OnMiniGameEnd += MoveCamsToBoardView;
    }
    
    private void OnVideoReceived(VideoStreamTrack track, string uuid)
    {
        WebCamUnit webCamUnit = CreateUnit(uuid);
        webCamUnit.SetTrack(track);
    }

    public WebCamUnit CreateUnit(string uuid)
    {
        WebCamUnit webCamUnit = Instantiate(originalPrefab, webCamCanvasUpdater.ContentTransform);
        webCamUnit.SetUuid(uuid);
        originalPrefab.gameObject.SetActive(true);
        
        _webCamUnits.Add(webCamUnit);
        
        SetCamsToBoardView();
        
        return webCamUnit;
    }

    private void OnVideoDisconnect(string obj)
    {
        for (int i = 0; i < _webCamUnits.Count; i++)
        {
            if (_webCamUnits[i].Uuid == obj)
            {
                _webCamUnits[i].UnSetTrack();
                _webCamUnits.RemoveAt(i);
                break;
            }
        }
    }

    private void SetCamsToBoardView()
    {
        WebCamAnchoredPosition camAnchoredPosition = _positionTable.GetAnchoredPosition(_webCamUnits.Count);
        
        for (int i = 0; i < _webCamUnits.Count; i++)
        {
            _webCamUnits[i].Mover.SetTo(camAnchoredPosition.BoardViewPositions[i]);
        }
    }

    private void MoveCamsToBoardView()
    {
        WebCamAnchoredPosition camAnchoredPosition = _positionTable.GetAnchoredPosition(_webCamUnits.Count);
        
        for (int i = 0; i < _webCamUnits.Count; i++)
        {
            _webCamUnits[i].Mover.MoveTween(camAnchoredPosition.BoardViewPositions[i]);
            _webCamUnits[i].ShowItemSocket();
        }
    }
    
    private void MoveCamsToGameView()
    {
        WebCamAnchoredPosition camAnchoredPosition = _positionTable.GetAnchoredPosition(_webCamUnits.Count);
        
        for (int i = 0; i < _webCamUnits.Count; i++)
        {
            _webCamUnits[i].Mover.MoveTween(camAnchoredPosition.GameViewPositions[i]);
            _webCamUnits[i].HideItemSocket();
        }
    }
}
