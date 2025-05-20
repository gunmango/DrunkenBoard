using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.WebRTC;
using System.Collections;
using System.Linq;

public class WebRTCController : MonoBehaviour
{
    public Action<VideoStreamTrack,string> OnVideoReceived { get; set; }
    public Action<string> OnVideoDisconnect { get; set; }
    
    [SerializeField] private SignalingClient signaling = null;
    
    private readonly List<Peer> _peers = new List<Peer>();
    
    private VideoStreamTrack _videoTrack;

    public void SetSelfWebCamTexture(WebCamTexture texture)
    {
        _videoTrack = new VideoStreamTrack(texture);
    }
    
    private void Start()
    {
        signaling.OnMessageReceived += OnSignalingMessage;
        StartCoroutine(WebRTC.Update());
    }

    private IEnumerator OnSignalingMessage(SignalingMessage signalingMessage)
    {
        switch (signalingMessage.type)
        {
            case "new_client": yield return OnNewClient(signalingMessage); break;
            case "offer": yield return OnOffer(signalingMessage); break;
            case "answer": yield return OnAnswer(signalingMessage); break;
            case "candidate": yield return OnCandidate(signalingMessage); break;
            case "leave": yield return OnLeave(signalingMessage); break;
        }
    }
    
    private IEnumerator OnNewClient(SignalingMessage msg)
    {
        Debug.Log(msg.type);
        //이미 연결된 상대라면 무시
        foreach (var peer in _peers)
        {
            if (peer.RemoteUuid == msg.fromUuid)
            {
                Debug.Log($"이미 추가된 클라이언트: {peer.RemoteUuid}");
                yield break;
            }
        }

        Peer newPeer = CreatePeer(msg);
        _peers.Add(newPeer);

        // 5. Offer 생성 및 전송
        StartCoroutine(CreateAndSendOffer(newPeer));
    }

    private IEnumerator OnOffer(SignalingMessage msg)
    {
        Debug.Log(msg.type);
        
        //이미 연결된 상대라면 무시
        foreach (var peer in _peers)
        {
            if (peer.RemoteUuid == msg.fromUuid)
            {
                Debug.Log($"이미 받은 오퍼: {peer.RemoteUuid}");
                yield break;
            }
        }
        
        Peer newPeer = CreatePeer(msg);
        _peers.Add(newPeer);
        
        // 상대가 보낸 offer 적용
        var offerDesc = new RTCSessionDescription
        {
            type = RTCSdpType.Offer,
            sdp = msg.sdp
        };
        yield return newPeer.Connection.SetRemoteDescription(ref offerDesc);
        
        // Offer 설정 및 Answer 생성
        yield return newPeer.Connection.SetRemoteDescription(ref offerDesc);
        var answerOp = newPeer.Connection.CreateAnswer();
        yield return answerOp;
        var answerDesc = answerOp.Desc;
        yield return newPeer.Connection.SetLocalDescription(ref answerDesc);
        
        // 상대방에게 answer 전송
        signaling.SendAnswer(answerDesc,msg.fromUuid);
        Debug.Log("받은 Offer 처리");
    }

    private IEnumerator OnAnswer(SignalingMessage msg)
    {
        Debug.Log(msg.type);
        
        if (msg.toUuid != signaling.Uuid)
        {
            //여기로 온 메세지 아님
            Debug.Log($"여기로 온 Answer 아님 {msg.toUuid}");
            yield break;
        }
        
        var peer = _peers.FirstOrDefault(p => p.RemoteUuid == msg.fromUuid);
        if (peer == null)
        {
            Debug.LogWarning("Answer 보낸 peer 못찾음");
            yield break;
        }
        
        // answer 처리
        if (peer.Connection.SignalingState == RTCSignalingState.HaveLocalOffer)
        {
            var desc = new RTCSessionDescription
            {
                type = RTCSdpType.Answer,
                sdp = msg.sdp
            };
            
            yield return peer.Connection.SetRemoteDescription(ref desc);
            Debug.Log("Answer 처리");
        }
    }
    
    private IEnumerator OnCandidate(SignalingMessage msg)
    {
        Debug.Log(msg.type);
        
        if (msg.toUuid != signaling.Uuid)
        {
            //여기로 온 메세지 아님
            Debug.Log($"여기로 온 Candidate 아님 {msg.toUuid}");
            yield break;
        }
        
        var peer = _peers.FirstOrDefault(p => p.RemoteUuid == msg.fromUuid);
        if (peer == null)
        {
            Debug.LogWarning("Candidate 보낸 peer 못찾음");
            yield break;
        }
        
        // Candidate 처리
        RTCIceCandidateInit init = new RTCIceCandidateInit
        {
            candidate = msg.candidate,
            sdpMid = msg.sdpMid,
            sdpMLineIndex = msg.sdpMLineIndex
        };
        peer.Connection.AddIceCandidate(new RTCIceCandidate(init));
        
        Debug.Log("Candidate 처리");
    }

    private IEnumerator OnLeave(SignalingMessage msg)
    {
        Debug.Log(msg.type);
        
        var peer = _peers.FirstOrDefault(p => p.RemoteUuid == msg.fromUuid);
        if (peer == null)
        {
            Debug.LogWarning("Leave 보낸 peer 못찾음");
            yield break;
        }
        
        peer.Connection.Close();
        peer.Connection.Dispose();
        _peers.Remove(peer);

        OnVideoDisconnect(peer.RemoteUuid);
    }

    private Peer CreatePeer(SignalingMessage msg)
    {
        string newClientUuid = msg.fromUuid;
        
        // 1. 새 Peer 생성
        var config = new RTCConfiguration
        {
            iceServers = new[] { new RTCIceServer { urls = new[] { "stun:stun.l.google.com:19302" } } }
        };
        Peer newPeer = new Peer(ref config, newClientUuid);
        
        // 2. 상대방 ICE 후보 수신 시 처리
        newPeer.Connection.OnIceCandidate = candidate =>
        {
            signaling.SendCandidate(candidate, newClientUuid);
            Debug.Log($"Send Candidate");
        };
        
        // 3. 상대방 비디오 수신
        newPeer.Connection.OnTrack = e =>
        {
            Debug.Log("비디오 수신");
            if (e.Track is VideoStreamTrack track)
            {
                // foreach (var receiver in remoteReceivers)
                // {
                //     if (receiver.IsUsing == true)
                //         continue;
                //     
                //     receiver.SetTrack(track); // 상대방 캠 영상 표시
                //     break;
                // }
                OnVideoReceived?.Invoke(track,newPeer.RemoteUuid);
            }
        };
        
        // 4. 내 비디오 송출 등록 (AddTrack)
        newPeer.Connection.AddTrack(_videoTrack);

        return newPeer;
    }
    
    private IEnumerator CreateAndSendOffer(Peer peer)
    {
        var offerOp = peer.Connection.CreateOffer();
        yield return offerOp;

        var offerDesc = offerOp.Desc;
        yield return peer.Connection.SetLocalDescription(ref offerDesc);

        signaling.SendOffer(offerDesc, peer.RemoteUuid);
    }
    

    private void OnDestroy()
    {
        foreach (var peer in _peers)
        {
            peer.Connection.Close();
            peer.Connection.Dispose();
        }
        _peers.Clear();
    }
}
