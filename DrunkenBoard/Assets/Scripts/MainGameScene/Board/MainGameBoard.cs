using System.Collections.Generic;
using UnityEngine;

public class MainGameBoard : MonoBehaviour
{
    [SerializeField] private SpriteRendererTweener spriteRendererTweener;
    [SerializeField] private List<Space> spaces;
    
    public Vector2 GetSpaceSpotPosition(int spaceIndex, int spotIndex)
    {
        return spaces[spaceIndex].PieceSpot(spotIndex);
    }
    
    public int GetSpaceCount => spaces.Count;
    
    private void Start()
    {
        MainGameSceneManager.MiniGameManager.OnMiniGameStart += FadeOutBoard;
        MainGameSceneManager.MiniGameManager.OnMiniGameEnd += FadeInBoard;
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
