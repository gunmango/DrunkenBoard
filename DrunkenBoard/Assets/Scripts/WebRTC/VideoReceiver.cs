using UnityEngine;
using UnityEngine.UI;
using Unity.WebRTC;

[RequireComponent(typeof(RawImage))]
public class VideoReceiver : MonoBehaviour
{
    private RawImage _image;
    public bool IsUsing { get; private set; } 
    public string Uuid { get; private set; }
    
    void Awake()
    {
        _image = GetComponent<RawImage>();
        IsUsing = false;
    }
    public void SetTrack(VideoStreamTrack track)
    {
        IsUsing = true;
        track.OnVideoReceived += tex => _image.texture = tex;
    }
}