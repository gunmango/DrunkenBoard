using Unity.WebRTC;
using UnityEngine;
using UnityEngine.Serialization;

public class WebCamUnit : MonoBehaviour
{
    [SerializeField] private WebCamUpdater webCamUpdater;
    [SerializeField] private ATrackController clientTrackController;
    [SerializeField] private WebCamSocket webCamSocket;
    [FormerlySerializedAs("mover")] [SerializeField] private WebCamTweener tweener;
    public int Uuid { get; private set; }
    public int Index { get; private set; }
    public WebCamTweener Tweener => tweener;
    public WebCamSocket WebCamSocket => webCamSocket;
    
    public void SetUuid(int uuid)
    {
        this.Uuid = uuid;
    }
    
    public void SetTrack(VideoStreamTrack track) => clientTrackController.SetTrack(track);

    public void SetColor(EPlayerColor color)
    {
        PlayerColorSet playerColorSet = PlayerManager.Table.GetColorSet(color);
        webCamUpdater.BoundaryImage.sprite = playerColorSet.WebCamBoundaryBasic;
        webCamUpdater.BoundarySelectedImage.sprite = playerColorSet.WebCamBoundarySelected;
    }
    
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
