using Unity.WebRTC;
using UnityEngine;

public class WebCamUnit : MonoBehaviour
{
    [SerializeField] private WebCamUpdater webCamUpdater;
    [SerializeField] private ATrackController clientTrackController;
    [SerializeField] private WebCamMover mover;
    public int Uuid { get; private set; }
    public int Index { get; private set; }
    public WebCamMover Mover => mover;
    
    public void SetUuid(int uuid)
    {
        this.Uuid = uuid;
    }
    
    public void SetTrack(VideoStreamTrack track) => clientTrackController.SetTrack(track);
    
    //플레이어 webrtc 연결끊김
    public void UnSetTrack()
    {
        clientTrackController.UnsetTrack();
    }

    public void HideItemSocket()
    {
        webCamUpdater.ItemSocket.SetActive(false);
    }

    public void ShowItemSocket()
    {
        webCamUpdater.ItemSocket.SetActive(true);
    }
}
