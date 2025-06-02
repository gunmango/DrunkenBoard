using UnityEngine;
using UnityEngine.Serialization;

public abstract class ASpaceEvent : MonoBehaviour
{
    public ESpaceEventType EventType = ESpaceEventType.None;
    public string EventName = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="enteredPlayerUuid">칸 밟은 플레이어의 uuid</param>
    public abstract void PlayEvent(int enteredPlayerUuid);
    
    //나중에 팝업 등
}
