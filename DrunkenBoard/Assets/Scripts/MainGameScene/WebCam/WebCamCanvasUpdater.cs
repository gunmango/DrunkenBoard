using UnityEngine;
using UnityEngine.UI;

public class WebCamCanvasUpdater : MonoBehaviour
{
    [SerializeField] private Transform contentTransform;
    
    public Transform ContentTransform => contentTransform;
}
