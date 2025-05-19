using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.WebRTC;
using UnityEngine;
using WebSocketSharp;

public class SignalingClient : MonoBehaviour
{
    public string signalingUrl = "ws://localhost:8765";
    private string _roomId = "room123";
    public string Uuid { get; } = Guid.NewGuid().ToString();

    public Func<SignalingMessage, IEnumerator> OnMessageReceived;

    private WebSocket websocket;
    
    private readonly Queue<SignalingMessage> messageQueue = new();

    public void SetRoomId(string roomId)
    {
        _roomId = roomId;
    }
    
    public void JoinRoom()
    {
        websocket.ConnectAsync();
        
        StartCoroutine(ReconnectProcess());
        StartCoroutine(ReceiveProcess());
    }

    public void LeaveRoom(string reason = null)
    {
        if (websocket != null && websocket.IsAlive)
        {
            SendLeave();
            websocket.Close(CloseStatusCode.Normal, reason);
        }

        StopAllCoroutines();
    }
    
    private void Start()
    {
        websocket = new WebSocket(signalingUrl);

        websocket.OnOpen += (sender, args) =>
        {
            Debug.Log("WebSocket Connected"); 
            SendJoin();
        };

        websocket.OnClose += (sender, args) =>
        {
            Debug.LogWarning("WebSocket Disconnected: " + args.Reason);
        };

        websocket.OnMessage += (sender, args) =>
        {
            Debug.Log("WebSocket Message: " + args.Data);
            if (args.IsText)
            {
                var signalingMessage = JsonConvert.DeserializeObject<SignalingMessage>(args.Data);
                messageQueue.Enqueue(signalingMessage);
                _ = StartCoroutine(OnMessageReceived(signalingMessage));
            }
        };
    }
    
    private IEnumerator ReconnectProcess()
    {
        var wfs = new WaitForSeconds(1f);
        while (true)
        {
            yield return wfs;

            if (websocket.IsAlive)
                continue;
            
            Debug.Log("Reconnecting...");
            websocket.ConnectAsync();
        }
    }

    private IEnumerator ReceiveProcess()
    {
        while (true)
        {
            yield return null;
            
            if (!websocket.IsAlive)
                continue;

            while (messageQueue.Count > 0)
            {
                if (messageQueue.TryDequeue(out var signalingMessage))
                {
                    yield return OnMessageReceived(signalingMessage);
                }
            }
        }
    }
    private void SendJson(object obj)
    {
        string json = JsonConvert.SerializeObject(obj);
        Debug.Log($"SendJson : {json}");
        websocket.Send(json);
    }
    
    private void SendJoin() =>
        SendJson(new
        {
            type = "join", room = _roomId, fromUuid = Uuid 
        });
    
    private void SendLeave() =>
        SendJson(new
        {
            type = "leave",
            room = _roomId,
            fromUuid = Uuid        
        });
    
    public void SendOffer(RTCSessionDescription desc, string targetUuid) =>
        SendJson(new
        {
            type = "offer",
            sdp = desc.sdp,
            room = _roomId,
            fromUuid = Uuid,
            toUuid = targetUuid
        });

    public void SendAnswer(RTCSessionDescription desc, string targetUuid) =>
        SendJson(new
        { 
            type = "answer",
            sdp = desc.sdp,
            room = _roomId,
            fromUuid = Uuid,
            toUuid = targetUuid
        });

    public void SendCandidate(RTCIceCandidate candidate, string targetUuid) =>
        SendJson(new
        {
            type = "candidate",
            candidate = candidate.Candidate,
            sdpMid = candidate.SdpMid,
            sdpMLineIndex = candidate.SdpMLineIndex,
            room = _roomId,
            fromUuid = Uuid,
            toUuid = targetUuid
        });

    private void OnDestroy()
    {
        LeaveRoom("Application Quit");
    }
    
}
