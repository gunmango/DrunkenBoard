using System.Collections.Generic;
using Unity.WebRTC;
using UnityEngine;

public class WebCamSetter : MonoBehaviour
{
    [SerializeField] private WebCamCanvasUpdater webCamCanvasUpdater;
    
    private List<WebCamViewer> _webCamViewers = new List<WebCamViewer>();

    private void Initialize()
    {
        GameManager.WebRtcController.OnVideoReceived += OnVideoReceived;
        GameManager.WebRtcController.OnVideoDisconnect += OnVideoDisconnect;
        
        
    }

    private void OnVideoReceived(VideoStreamTrack arg1, string arg2)
    {
        var newUpdater = webCamCanvasUpdater.CreateUpdater();
        newUpdater.gameObject.SetActive(true);
        
        WebCamViewer webCamViewer = new WebCamViewer(newUpdater, arg2);
        webCamViewer.SetTrack(arg1);
        
        _webCamViewers.Add(webCamViewer);
    }

    private void OnVideoDisconnect(string obj)
    {
        for (int i = 0; i < _webCamViewers.Count; i++)
        {
            if (_webCamViewers[i].Uuid == obj)
            {
                _webCamViewers[i].UnSetTrack();
                _webCamViewers.RemoveAt(i);
                break;
            }
        }
    }
}
