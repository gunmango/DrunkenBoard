using UnityEngine;
using UnityEngine.UI;

public class WebCamCanvasUpdater : MonoBehaviour
{
    [SerializeField] private WebCamUpdater originalUpdater;
    [SerializeField] private Transform contentTransform;

    public WebCamUpdater CreateUpdater()
    {
        WebCamUpdater updater = Instantiate(originalUpdater, contentTransform);
        return updater;
    }
}
