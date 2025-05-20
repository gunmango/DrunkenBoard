using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.WebRTC;
using UnityEngine;
using WebSocketSharp;

public class SignalingClient : MonoBehaviour
{
    [SerializeField] private GameObject reconnectCanvas;
    public string Uuid { get; } = Guid.NewGuid().ToString();

    public Func<SignalingMessage, IEnumerator> OnMessageReceived { get; set; }
    public Action OnConnected { get; set; }

    private WebSocket _websocket;
    
    private readonly Queue<SignalingMessage> _messageQueue = new();
    private string _signalingUrl = string.Empty;
    private string _roomId = string.Empty;

    private bool _isOpened;


    public void SetRoomId(string roomId)
    {
        _roomId = roomId;
    }

    public void SetSignalingUrl(string hostIp)
    {
        _signalingUrl = NetWorkConstants.GetSignalingUrl(hostIp);
    }

    public void SetHostSignalingUrl()
    {
        _signalingUrl = NetWorkConstants.GetSignalingUrl();
    }
    
    public void JoinRoom()
    {
        InitializeWebSocket();
        _websocket.ConnectAsync();
        
        StartCoroutine(ReconnectProcess());
        StartCoroutine(ReceiveProcess());
        StartCoroutine(OpenProcess());
    }

    public void LeaveRoom(string reason = null)
    {
        if (_websocket != null && _websocket.IsAlive)
        {
            SendLeave();
            _websocket.Close(CloseStatusCode.Normal, reason);
        }

        StopAllCoroutines();
    }
    
    private void InitializeWebSocket()
    {
        _websocket = new WebSocket(_signalingUrl);

        _websocket.OnOpen += (sender, args) =>
        {
            Debug.Log("WebSocket Connected"); 
            SendJoin();
            _isOpened = true;
        };

        _websocket.OnClose += (sender, args) =>
        {
            Debug.LogWarning("WebSocket Disconnected: " + args.Reason);
        };

        _websocket.OnMessage += (sender, args) =>
        {
            Debug.Log("WebSocket Message: " + args.Data);
            if (args.IsText)
            {
                var signalingMessage = JsonConvert.DeserializeObject<SignalingMessage>(args.Data);
                _messageQueue.Enqueue(signalingMessage);
                _ = StartCoroutine(OnMessageReceived(signalingMessage));
            }
        };
    }

    private IEnumerator OpenProcess()
    {
        yield return new WaitUntil(() => _isOpened);
        OnConnected?.Invoke();
    }

    private IEnumerator ReconnectProcess()
    {
        var wfs = new WaitForSeconds(1f);
        while (true)
        {
            yield return wfs;

            if (_websocket.IsAlive)
            {
                if(reconnectCanvas.activeSelf)
                    reconnectCanvas.SetActive(false);
                continue;
            }
            
            if(reconnectCanvas.activeSelf == false)
                reconnectCanvas.SetActive(true);
            
            Debug.Log("Reconnecting...");
            _websocket.ConnectAsync();
        }
        
    }

    private IEnumerator ReceiveProcess()
    {
        while (true)
        {
            yield return null;
            
            if (!_websocket.IsAlive)
                continue;

            while (_messageQueue.Count > 0)
            {
                if (_messageQueue.TryDequeue(out var signalingMessage))
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
        _websocket.Send(json);
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
