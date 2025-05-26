using UnityEngine;
using Fusion;


public class ReadyPopup : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(UpdateUI))] public bool IsOpen { get; set; }
    
    [SerializeField] private ReadyPopupUpdater updater;
    
    public override void Spawned()
    {
        if (GameManager.FusionSession.Runner.IsSharedModeMasterClient)
        {
            updater.gameObject.SetActive(true);
            updater.StartButton.gameObject.SetActive(true);
            updater.StartButton.onClick.AddListener(OnClickStartButton);
            IsOpen = true;
            return;
        }
        
        updater.StartButton.gameObject.SetActive(false);
 
        updater.gameObject.SetActive((IsOpen));
    }
    
    private void OnClickStartButton()
    {
        if (GameManager.FusionSession.Runner.IsSharedModeMasterClient == false)
            return;

        IsOpen = false;
        updater.gameObject.SetActive(false);
        MainGameSceneManager.GameStateManager.ChangeState_RPC(EMainGameState.Board);
    }

    private void UpdateUI()
    {
        if (!GameManager.FusionSession.Runner.IsRunning)
            return;
        
        updater.gameObject.SetActive(IsOpen);
    }
}
