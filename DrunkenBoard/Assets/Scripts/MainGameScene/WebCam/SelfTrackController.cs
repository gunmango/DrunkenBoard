using Unity.WebRTC;
using UnityEngine;
using UnityEngine.UI;

public class SelfTrackController : ATrackController
{
    [SerializeField] private int deviceNum = 0;
    [SerializeField] private Button activationButton;
    
    private WebCamTexture _webCamTexture;
    private VideoStreamTrack _track;
    private bool _isCamOn = false;

    public override void SetTrack(VideoStreamTrack track)
    {
        _webCamTexture = new WebCamTexture(WebCamTexture.devices[deviceNum].name);
        _webCamTexture.Play();
        image.texture = _webCamTexture;
        
        _track = new VideoStreamTrack(_webCamTexture);
        
        GameManager.WebRtcController.SetSelfWebCamTexture(_track);    
    }

    public override void UnsetTrack()
    {
        MainGameSceneManager.Instance.ActInitialize -= Initialize;
    }
    
    private void Awake()
    {
        MainGameSceneManager.Instance.ActInitialize += Initialize;
        activationButton.onClick.AddListener(ToggleActivation);
    }

    private void Initialize()
    {
        SetTrack(null);
    }

    private void ToggleActivation()
    {
        _isCamOn = !_isCamOn;
        image.enabled = _isCamOn;
        _track.Enabled = _isCamOn;
    }
}
