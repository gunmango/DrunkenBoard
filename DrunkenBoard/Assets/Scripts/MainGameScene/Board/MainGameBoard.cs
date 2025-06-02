using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class MainGameBoard : MonoBehaviour
{
    [SerializeField] private SpriteRendererTweener spriteRendererTweener;
    [SerializeField] private List<Space> spaces;
    
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
        MainGameSceneManager.GameStateManager.ChangeState_RPC(EMainGameState.SpaceEvent);
        ESpaceEventType spaceEventType = spaces[spaceIndex].SpaceEventType;
        MainGameSceneManager.SpaceEventManager.PlayEvent(spaceEventType, enteredPlayerUuid);
    }
    
    private void Start()
    {
        MainGameSceneManager.GameStateManager.ActOnSpaceEvent += FadeOutBoard;
        MainGameSceneManager.GameStateManager.ActOnBoard += FadeInBoard;
        InitializeSpaceEvents();
    }

    private void InitializeSpaceEvents()
    {
        List<ASpaceEvent> spaceEvents = MainGameSceneManager.SpaceEventManager.SpaceEvents;

        foreach (var space in spaces)
        {
            ASpaceEvent randomEvent = spaceEvents[Random.Range(0, spaceEvents.Count)];
            space.SetEvent(randomEvent.EventType, randomEvent.EventName);
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
