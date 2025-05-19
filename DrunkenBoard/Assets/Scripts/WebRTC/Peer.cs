using Unity.WebRTC;

public class Peer
{
    public readonly RTCPeerConnection Connection;
    public readonly string RemoteUuid;

    public Peer(ref RTCConfiguration config, string remoteUuid)
    {
        Connection = new RTCPeerConnection(ref config);;
        RemoteUuid = remoteUuid;
    }
}