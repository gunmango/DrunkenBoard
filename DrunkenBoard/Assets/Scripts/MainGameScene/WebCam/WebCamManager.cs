using System.Collections.Generic;
using Unity.WebRTC;
using UnityEngine;

public class WebCamManager : MonoBehaviour
{
    [SerializeField] private WebCamCanvasUpdater webCamCanvasUpdater;
    [SerializeField] private WebCamUnit selfWebCamUnit;
    [SerializeField] private WebCamUnit originalPrefab;
    
    private readonly List<WebCamUnit> _webCamUnits = new List<WebCamUnit>();
    
    public void Initialize()
    {
        _webCamUnits.Add(selfWebCamUnit);

        GameManager.WebRtcController.OnVideoReceived += OnVideoReceived;
        GameManager.WebRtcController.OnVideoDisconnect += OnVideoDisconnect;
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
}
