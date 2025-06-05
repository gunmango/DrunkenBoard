using System;
using Fusion;
using UnityEngine;

public class EventReadyPopupData : PopupDataBase
{
    public string EventName;
    public int OpenerUuid;
    public Action ClosePopupAction;
}

public class EventReadyPopup : NetworkBehaviour, IBasePopup
{
    [SerializeField] private EventReadyPopupUpdater updater;
    private Action _closePopupAction = null;
    private bool _isStartButtonActive = false;
    
    public override void Spawned()
    {
        updater.StartButton.onClick.AddListener(ClosePopup_RPC);
        updater.Animator.ActOnEndOpen = ()=> updater.StartButton.gameObject.SetActive(_isStartButtonActive);
        updater.Animator.ActOnEndClose = ()=> updater.gameObject.SetActive(false);
        ;
    }

    public void Open(PopupDataBase data = null)
    {
        if (data is EventReadyPopupData popupData == false)
        {
            Debug.LogError("wrong data type");
            return;
        }
        
        updater.EventNameText.text = popupData.EventName;
        _closePopupAction = popupData.ClosePopupAction;
        
        _isStartButtonActive = popupData.OpenerUuid == Runner.LocalPlayer.RawEncoded;
        
        updater.gameObject.SetActive(true);
        updater.Animator.PlayOpenAnimation();
    }

    public void Close()
    {
        updater.StartButton.gameObject.SetActive(false);
        updater.Animator.PlayCloseAnimation();
        _closePopupAction?.Invoke();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void ClosePopup_RPC()
    {
        GameManager.PopupManager.CloseTopPopup();
    }
    
}
