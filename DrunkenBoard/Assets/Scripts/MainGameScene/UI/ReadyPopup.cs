using System.Collections;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using Unity.VisualScripting;


public class ReadyPopup : SimulationBehaviour
{
    [Networked] public int Count { get; set; } = 0;
    
    [SerializeField] private ReadyPopupUpdater updater;
    
    private void Start()
    {
        if (GameManager.FusionSession.Runner.IsSharedModeMasterClient)
        {
            updater.GameObject().SetActive(true);
            updater.StartButton.gameObject.SetActive(true);
            updater.StartButton.onClick.AddListener(OnClickStartButton);
            Count++;
            return;
        }
        
        updater.StartButton.gameObject.SetActive(false);
 
        updater.gameObject.SetActive((Count != 0));
    }
    
    private void OnClickStartButton()
    {
        if (GameManager.FusionSession.Runner.IsSharedModeMasterClient == false)
            return;

        Count = 0;
        updater.gameObject.SetActive(false);
        MainGameSceneManager.GameStateManager.ChangeState(EMainGameState.Board);
    }
}
