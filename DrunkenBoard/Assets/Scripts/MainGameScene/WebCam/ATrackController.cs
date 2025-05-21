using Unity.WebRTC;
using UnityEngine;
using UnityEngine.UI;

public abstract class ATrackController : MonoBehaviour
{
    [SerializeField] protected RawImage image;
    
    public abstract void SetTrack(VideoStreamTrack track);
    public abstract void UnsetTrack();
}
