using UnityEngine;

public static class NetWorkConstants
{
    private const int SignalingPort = 8765;
    private const string DefaultHost = "localhost";
    private const string WebSocketProtocol = "ws";

    public static string GetSignalingUrl(string host = DefaultHost)
    {
        return $"{WebSocketProtocol}://{host}:{SignalingPort}";
    }
}
