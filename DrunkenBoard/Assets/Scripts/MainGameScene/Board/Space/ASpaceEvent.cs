using UnityEngine;
using UnityEngine.Serialization;

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
}
