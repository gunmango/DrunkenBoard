using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class MainGameBoard : MonoBehaviour
{
    [SerializeField] private SpriteRendererTweener spriteRendererTweener;
    [SerializeField] private List<Space> spaces;
    [SerializeField] private GameBoardNetworkHelper networkHelper;
    public Vector2 GetSpaceSpotPosition(int spaceIndex, int spotIndex)
    {
        return spaces[spaceIndex].GetPieceSpotPos(spotIndex);
    }
    
    public int GetSpaceCount => spaces.Count;

    public int GetSpaceSpotCount(int spaceIndex)
    {
        return spaces[spaceIndex].GetSpotCount();
    }

    public void PlaySpaceEvent(int spaceIndex, int enteredPlayerUuid)
    {
        ESpaceEventType spaceEventType = spaces[spaceIndex].SpaceEventType;
        MainGameSceneManager.SpaceEventManager.PlayEvent(spaceEventType, enteredPlayerUuid);
    }
    
    private void Start()
    {
        MainGameSceneManager.GameStateManager.ActOnSpaceEvent += FadeOutBoard;
        MainGameSceneManager.GameStateManager.ActOnBoard += FadeInBoard;
    }
    
    //rpc로 각각받은 이벤트 세팅
    public void SetEvents(List<int> eventIndexes)
    {
        List<ASpaceEvent> spaceEvents = MainGameSceneManager.SpaceEventManager.SpaceEvents;

        for (int i = 0; i < spaces.Count; i++)
        {
            ASpaceEvent randomEvent = spaceEvents[eventIndexes[i]];
            spaces[i].SetEvent(randomEvent.EventType, randomEvent.EventName);
        }
    }
    
    private void FadeOutBoard()
    {
        spriteRendererTweener.FadeOut();

        foreach (var space in spaces)
        {
            space.SpriteTweener.FadeOut();
        }
    }

    private void FadeInBoard()
    {
        spriteRendererTweener.FadeIn();

        foreach (var space in spaces)
        {
            space.SpriteTweener.FadeIn();
        }
    }
}
