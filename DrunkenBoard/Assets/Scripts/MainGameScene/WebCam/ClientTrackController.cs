using UnityEngine;
using Unity.WebRTC;

public class ClientTrackController : ATrackController
{
    private VideoStreamTrack _track = null;
    
    public override void SetTrack(VideoStreamTrack track)
    {
        _track = track;
        _track.OnVideoReceived += OnFrameReceived;
    }

    public override void UnsetTrack()
    {
        image.texture = null;
        
        if (_track == null)
            return;
        
        _track.OnVideoReceived -= OnFrameReceived;
        _track.Dispose();
        _track = null;
    }
    
    private void OnFrameReceived(Texture tex)
    {
        image.texture = tex;
    }
}
