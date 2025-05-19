using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class WebCamPlayer : MonoBehaviour
{
    [SerializeField] private int deviceNum = 0;
    private RawImage image;
    private WebCamTexture webCamTexture;

    public WebCamTexture CamTexture => webCamTexture;
    private void Awake()
    {
        image = GetComponent<RawImage>();
    }

    private void Start()
    {
        webCamTexture = new WebCamTexture(WebCamTexture.devices[deviceNum].name);
        webCamTexture.Play();
        image.texture = webCamTexture;
    }
}