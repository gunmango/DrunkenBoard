using UnityEngine;
using UnityEngine.UI;

public class SelfWebCamPlayer : MonoBehaviour
{
    [SerializeField] private int deviceNum = 0;
    [SerializeField] private RawImage image;
    
    private WebCamTexture _webCamTexture;

    private void Awake()
    {
        MainGameSceneManager.Instance.ActInitialize += Initialize;
    }
    
    private void Initialize()
    {
        _webCamTexture = new WebCamTexture(WebCamTexture.devices[deviceNum].name);
        _webCamTexture.Play();
        image.texture = _webCamTexture;
        
        GameManager.WebRtcController.SetSelfWebCamTexture(_webCamTexture);
    }

    private void OnDestroy()
    {
        MainGameSceneManager.Instance.ActInitialize -= Initialize;
    }
}
