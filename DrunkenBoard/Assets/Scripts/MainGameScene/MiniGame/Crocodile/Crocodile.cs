using UnityEngine;

public class Crocodile : MonoBehaviour
{
    [SerializeField] private Sprite openMouth;
    [SerializeField] private Sprite closeMouth;
    private SpriteRenderer _mouthRenderer;

    private void Start()
    {
        _mouthRenderer = GetComponent<SpriteRenderer>();
        _mouthRenderer.sprite = openMouth;
    }

    public void CloseMouth()
    {
        _mouthRenderer.sprite = closeMouth;
    }
}
