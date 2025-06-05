using System;
using Fusion;
using UnityEngine;

public class CrocodileMouth : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    public Sprite openMouthSprite;
    public Sprite closedMouthSprite;
    
    private void Awake()
    {
        gameObject.SetActive(true);
    }

    public void CloseMouth()
    {
        Debug.Log("RPC_CloseMouth 실행됨");

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = closedMouthSprite;
            Debug.Log("입 닫힘: 스프라이트 변경 완료");
        }
        else
        {
            Debug.LogError("RPC_CloseMouth에서 spriteRenderer가 null입니다!");
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_OpenMouth()
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = openMouthSprite;
    }

    private void OnEnable()
    {
        spriteRenderer.sprite = openMouthSprite;
    }
}