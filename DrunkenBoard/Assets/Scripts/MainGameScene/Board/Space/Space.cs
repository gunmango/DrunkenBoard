using UnityEngine;

public class Space : MonoBehaviour
{
    [SerializeField] private SpriteRendererTweener spriteTweener;
    
    public SpriteRendererTweener SpriteTweener => spriteTweener;
}
