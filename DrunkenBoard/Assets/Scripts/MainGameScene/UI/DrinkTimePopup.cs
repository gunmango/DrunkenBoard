using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class DrinkTimePopupData : PopupDataBase
{
    public int OpenerUuid;
    public List<string> DrinkerNames = new List<string>();
    public Action ClosePopupAction;
}

public class DrinkTimePopup : NetworkBehaviour, IBasePopup
{
    [SerializeField] private DrinkTimePopupUpdater updater;

    private Action _closePopupAction = null;
    private bool _isStartButtonActive = false;
    private DrinkTimePopupData _popupData = null;
    
    public override void Spawned()
    {
        updater.StartButton.onClick.AddListener(ClosePopup_RPC);
        updater.Animator.ActOnEndOpen = OnOpenAnimationEnd;
        updater.Animator.ActOnEndClose = ()=> updater.gameObject.SetActive(false);
    }
    
    public void Open(PopupDataBase data = null)
    {
        if (data is DrinkTimePopupData popupData == false)
        {
            Debug.LogError("wrong data type");
            return;
        }

        _popupData = popupData;
        //이름세팅
        for (int i = 0; i < updater.NameTexts.Length; i++)
        {
            if (i < _popupData.DrinkerNames.Count)
            {
                updater.NameTexts[i].gameObject.SetActive(true);
                updater.NameTexts[i].text = _popupData.DrinkerNames[i];
                continue;
            }

            updater.NameTexts[i].gameObject.SetActive(false);
        }

        _closePopupAction = popupData.ClosePopupAction;

        _isStartButtonActive = popupData.OpenerUuid == Runner.LocalPlayer.RawEncoded;
        
        updater.gameObject.SetActive(true);
        updater.Animator.PlayOpenAnimation();    
    }

    private void OnOpenAnimationEnd()
    {
        //엑티브
        updater.NameListParent.SetActive(true);
        updater.StartButton.gameObject.SetActive(_isStartButtonActive);
    }
    
    public void Close()
    {
        updater.StartButton.gameObject.SetActive(false);
        updater.NameListParent.SetActive(false);

        updater.Animator.PlayCloseAnimation();
        _closePopupAction?.Invoke();
    }
    
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void ClosePopup_RPC()
    {
        GameManager.PopupManager.CloseTopPopup();
    }
}
