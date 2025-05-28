using Fusion;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);  
        
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    

    [SerializeField] private PopupManager popupManager;
    public static PopupManager PopupManager => Instance.popupManager;
    
    [SerializeField] private SignalingClient signalingClient;
    public static SignalingClient SignalingClient => Instance.signalingClient;
    
    [SerializeField] private WebRTCController webRtcController;
    public static WebRTCController WebRtcController => Instance.webRtcController;
    
    [SerializeField] private FusionSession fusionSession;
    public static FusionSession FusionSession => Instance.fusionSession;
    
    
    public bool IsSignaling = false;
    

    private void Start()
    {
        if (IsSignaling)
            FusionSession.ActOnPlayerJoined += ConnectFusionAndSignalingClient;
    }

    private void ConnectFusionAndSignalingClient(NetworkRunner runner, PlayerRef player)
    {
        if (player == runner.LocalPlayer)
        {
            SignalingClient.Uuid = player.RawEncoded.ToString();
        }    
    }
}
