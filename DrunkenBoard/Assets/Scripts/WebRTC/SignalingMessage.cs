using UnityEngine;

[System.Serializable]
public class SignalingMessage
{
    public string type;
    public string sdp;
    public string candidate;
    public string sdpMid;
    public int sdpMLineIndex;
    public string room;
    public string fromUuid;
    public string toUuid;
}