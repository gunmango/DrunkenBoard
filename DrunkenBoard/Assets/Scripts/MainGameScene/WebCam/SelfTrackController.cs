using Unity.WebRTC;
using UnityEngine;

public class SelfTrackController : ATrackController
{
    [SerializeField] private int deviceNum = 0;
    
    private WebCamTexture _webCamTexture;

    private void Awake()
    {
        MainGameSceneManager.Instance.ActInitialize += Initialize;
    }
    
    private void Initialize()
    {
        SetTrack(null);
    }
    
    public override void SetTrack(VideoStreamTrack track)
    {
        _webCamTexture = new WebCamTexture(WebCamTexture.devices[deviceNum].name);
        _webCamTexture.Play();
        image.texture = _webCamTexture;
        
        GameManager.WebRtcController.SetSelfWebCamTexture(_webCamTexture);    
    }

    public override void UnsetTrack()
    {
        MainGameSceneManager.Instance.ActInitialize -= Initialize;
    }
}
