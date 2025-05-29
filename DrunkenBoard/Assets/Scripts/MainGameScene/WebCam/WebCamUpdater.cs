using UnityEngine;
using UnityEngine.UI;

public class WebCamUpdater : MonoBehaviour
{
    [SerializeField] private GameObject itemSocket;
    [SerializeField] private RawImage rawImage;
    [SerializeField] private Image boundaryBasic;
    [SerializeField] private Image boundarySelected;
    
    public GameObject ItemSocket => itemSocket;
    public RawImage RawImage => rawImage;
    public Image BoundaryImage => boundaryBasic;
    public Image BoundarySelectedImage => boundarySelected;
}
