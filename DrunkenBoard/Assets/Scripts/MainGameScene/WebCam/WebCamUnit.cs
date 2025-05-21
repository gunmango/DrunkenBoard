using Unity.WebRTC;
using UnityEngine;
using UnityEngine.Serialization;

public class WebCamUnit : MonoBehaviour
{
    [SerializeField] private WebCamUpdater webCamUpdater;
    [SerializeField] private ATrackController clientTrackController;
    [SerializeField] private WebCamMover mover;
    public string Uuid { get; private set; }
    public int Index { get; private set; }

    public void SetUuid(string uuid)
    {
        this.Uuid = uuid;
    }
    
    public void SetTrack(VideoStreamTrack track) => clientTrackController.SetTrack(track);
    //플레이어 webrtc 연결끊김
    public void UnSetTrack()
    {
        clientTrackController.UnsetTrack();
        Destroy(gameObject);
    }
    
    public void MoveTo(Vector2 anchoredPosition) => mover.MoveTo(anchoredPosition);
}
