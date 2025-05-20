using System;
using UnityEngine;

public class Tooth : MonoBehaviour
{
    [SerializeField] private Sprite upToothSprite;
    [SerializeField] private Sprite downToothSprite;

    
    public bool IsPressed = false;
    public bool Istrap = false;
    public CrocodileGameManager gameManager;
    
    private SpriteRenderer _toothRenderer;
    public Crocodile Croc;
    private void Start()
    {
        _toothRenderer = GetComponent<SpriteRenderer>();
        _toothRenderer.sprite = upToothSprite;
    }

    private void OnMouseDown()
    {
        if (!IsPressed&&gameManager.IsCurrentPlayerTurn)
        {
            gameManager.PressTooth(this);
        }
    }

    public void ForcePress()
    {
        _toothRenderer.sprite = downToothSprite;
        IsPressed = true; 
        _toothRenderer.color = Color.white;
        OnToothpressed();
    }

    private void OnMouseEnter()
    {
        if(!IsPressed)
            _toothRenderer.color = Color.blue;
    }

    private void OnMouseExit()
    {
        if (!IsPressed)
        {
            _toothRenderer.color = Color.white;
        }
    }

    protected virtual void OnToothpressed()
    {
        if (Istrap)
        {
          gameManager.OnTrapTriggered();
        }
    }

    public void HideTooth()
    {
        gameObject.SetActive(false);
    }
}
