using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LobbySceneManager : ASceneManager<LobbySceneManager>
{
    [SerializeField] private UuidListSetter uuidListSetter;
    [SerializeField] private Button startButton;
    
    protected override void Initialize()
    {
        base.Initialize();
        uuidListSetter.Initialize();
        GameManager.SignalingClient.OnMessageReceived += OnSignalingMessage;
       
        startButton.onClick.AddListener(GameStart);
    }
    
    private IEnumerator OnSignalingMessage(SignalingMessage signalingMessage)
    {
        switch (signalingMessage.type)
        {
            case "new_client": uuidListSetter.AddNewClient(signalingMessage.fromUuid); break;
            case "leave": uuidListSetter.RemoveClient(signalingMessage.fromUuid); break;
        }
        yield return null;
    }

    private void OnDestroy()
    {
        GameManager.SignalingClient.OnMessageReceived -= OnSignalingMessage;
    }

    private void GameStart()
    {
    }
}
