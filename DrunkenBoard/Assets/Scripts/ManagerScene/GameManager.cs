using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private SceneController sceneController;
    public static SceneController SceneController => Instance.sceneController;

    [SerializeField] private PopupManager popupManager;
    public static PopupManager PopupManager => Instance.popupManager;
    
    [SerializeField] private SignalingClient signalingClient;
    public static SignalingClient SignalingClient => Instance.signalingClient;
}
