using UnityEngine;
using UnityEngine.UI;

public class WebCamCanvasUpdater : MonoBehaviour
{
    [SerializeField] private WebCamUpdater selfUpdater;
    [SerializeField] private WebCamUpdater originalUpdater;
    [SerializeField] private Transform contentTransform;

    public WebCamUpdater SelfUpdater => selfUpdater;
    public WebCamUpdater CreateUpdater()
    {
        WebCamUpdater updater = Instantiate(originalUpdater, contentTransform);
        return updater;
    }
}
