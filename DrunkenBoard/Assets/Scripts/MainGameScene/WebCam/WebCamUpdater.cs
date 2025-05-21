using UnityEngine;
using UnityEngine.UI;

public class WebCamUpdater : MonoBehaviour
{
    [SerializeField] private GameObject itemSocket;
    [SerializeField] private RawImage rawImage;
    
    public GameObject ItemSocket => itemSocket;
    public RawImage RawImage => rawImage;
}
