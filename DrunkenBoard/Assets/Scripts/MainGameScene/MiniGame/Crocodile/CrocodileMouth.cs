using Fusion;
using UnityEngine;

public class CrocodileMouth : NetworkBehaviour
{
    public Sprite openMouthSprite;
    public Sprite closedMouthSprite;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    
        if (spriteRenderer == null)
        {
            Debug.LogError("[CrocodileMouth] SpriteRenderer가 없습니다!");
        }
    }


   
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_CloseMouth()
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

    // 로컬에서 바로 바꾸지 말고, RPC 호출로 모두 동기화!
    public void CloseMouth()
    {
        Debug.Log($"CloseMouth 호출, HasStateAuthority: {Object.HasStateAuthority}");

        if (Object.HasStateAuthority)
        {
            RPC_CloseMouth();
        }
        else
        {
            Debug.LogWarning("CloseMouth 호출했지만 StateAuthority가 없습니다.");
        }
    }

    public void OpenMouth()
    {
        if (Object.HasStateAuthority)
        {
            RPC_OpenMouth();
        }
    }
}