using System.Collections.Generic;
using UnityEngine;

public abstract class ASpaceEvent : MonoBehaviour
{
    public ESpaceEventType EventType = ESpaceEventType.None;
    public string EventName = string.Empty;
    
    protected int _enteredPlayerUuid = 0;

    private float _startDuration = 0.5f;  //웹캠이동후 이벤트 시작까지 대기시간
    
    /// <summary>
    /// 팝업 닫았을때 자동실행
    /// </summary>
    protected abstract void PlayEvent();

    /// <summary>
    /// 팝업열기
    /// </summary>
    /// <param name="enteredPlayerUuid">칸 밟은 플레이어의 uuid</param>
    public void ReadyEvent(int enteredPlayerUuid)
    {
        _enteredPlayerUuid = enteredPlayerUuid;
        
        EventReadyPopupData data = new EventReadyPopupData();
        data.EventName = EventName;
        data.OpenerUuid = enteredPlayerUuid;
        data.ClosePopupAction = ()=>
        {
            MainGameSceneManager.GameStateManager.ChangeState_RPC(EMainGameState.SpaceEvent);
            Invoke(nameof(PlayEvent), WebCamConstants.MoveDuration + _startDuration);
        };
        
        MainGameSceneManager.Instance.OpenSpaceEventReadyPopup(data);
    }
    
    /// <summary>
    /// 마시는타임 팝업열기
    /// </summary>
    /// <param name="drinkerUuids">마실사람 id</param>
    public void EndEvent(List<int> drinkerUuids)
    {
        DrinkTimePopupData data = new DrinkTimePopupData();
        data.OpenerUuid = _enteredPlayerUuid;
        
        List<string>drinkerNames = new List<string>();
        
        for (int i = 0; i < drinkerUuids.Count; i++)
        {
            drinkerNames.Add(PlayerManager.Instance.GetPlayerName(drinkerUuids[i]));
            MainGameSceneManager.WebCamManager.StartBlinkingBoundary(drinkerUuids[i]);
        }
        
        data.DrinkerNames = drinkerNames;
        data.ClosePopupAction = () =>
        {
            for (int i = 0; i < drinkerUuids.Count; i++)
            {
                MainGameSceneManager.WebCamManager.StopBlinkingBoundary(drinkerUuids[i]);
            }
            MainGameSceneManager.GameStateManager.ChangeState_RPC(EMainGameState.Board);
        };
        
        MainGameSceneManager.Instance.OpenDrinkTimePopup(data);
    }
}
