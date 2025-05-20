using Unity.WebRTC;
using UnityEngine;

public class WebCamViewer
{
    public string Uuid { get; private set; }

    private readonly WebCamUpdater _webCamUpdater = null;
    private VideoStreamTrack _track = null;
    public WebCamViewer(WebCamUpdater webCamUpdater, string uuid)
    {
        _webCamUpdater = webCamUpdater;
        Uuid = uuid;
    }
    
    public void SetTrack(VideoStreamTrack track)
    {
        _track = track;
        _track.OnVideoReceived += OnFrameReceived;
    }

    public void UnSetTrack()
    {
        _webCamUpdater.RawImage.texture = null;
        UnityEngine.Object.Destroy(_webCamUpdater.gameObject);
        
        if (_track == null)
            return;
        
        _track.OnVideoReceived -= OnFrameReceived;
        _track.Dispose();
        _track = null;
    }
    
    private void OnFrameReceived(Texture tex)
    {
        _webCamUpdater.RawImage.texture = tex;
    }
}
