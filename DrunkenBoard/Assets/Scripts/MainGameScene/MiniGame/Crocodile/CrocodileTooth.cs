using UnityEngine;
using Fusion;
using System;

public class CrocodileTooth : NetworkBehaviour
{
    public int toothIndex;
    private SpriteRenderer spriteRenderer;

    public Sprite normalSprite;
    public Sprite downSprite;

    [SerializeField] private CrocodileMouth crocodileMouth;
    [SerializeField] private CrocodileGameManager gameManager;

    [Networked]
    private bool isClicked { get; set; }

    [Networked]
    private bool isTrap { get; set; }

    [Networked]
    private bool isGameEnded { get; set; }
    
    public event Action<int> OnToothClicked;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void Spawned()
    {
        UpdateVisuals();
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetTrap(bool trap)
    {
        isTrap = trap;

        if (isTrap)
        {
            //Debug.Log($"🧨 이빨 {toothIndex}이 트랩으로 설정됨!");
        }
    }

    // 모든 클라이언트에서 StateAuthority로 클릭 요청
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_RequestClickTooth(int requesterPlayerId)
    {
        // Debug.Log($"🔄 이빨 {toothIndex} 클릭 요청 받음 from player {requesterPlayerId}");
        
        // StateAuthority에서만 실제 처리
        if (Object.HasStateAuthority && !isClicked && !isGameEnded)
        {
            // 즉시 상태 업데이트
            isClicked = true;
            
            // 모든 클라이언트에게 결과 전송
            RPC_ExecuteClickTooth(isTrap);
        }
    }

    // StateAuthority에서 모든 클라이언트로 결과 전송
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ExecuteClickTooth(bool wasTrap)
    {
        // Debug.Log($"🖱️ 이빨 {toothIndex} 클릭 처리. 트랩인가? {wasTrap}");

        // 상태 확실히 동기화
        isClicked = true;
        
        // 즉시 시각적 업데이트
        UpdateVisuals();

        if (wasTrap)
        {
            // Debug.Log($"💥 트랩 이빨 눌림! 게임 종료 처리 시작");
            
            isGameEnded = true;
            UpdateVisuals(); // 게임 종료 상태 즉시 반영
            
            crocodileMouth.CloseMouth();
          
            // StateAuthority에서만 게임 종료 처리
            if (Object.HasStateAuthority)
            {
                gameManager.EndGame();
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_EndGame()
    {
        isGameEnded = true;
        UpdateVisuals();
        // Debug.Log($"🔥 이빨 {toothIndex} 게임 종료 처리됨");
    }
    
    public void UpdateVisuals()
    {
        if (spriteRenderer == null) return;

        if (isGameEnded)
        {
            isClicked = false;
            isTrap = false;
            isGameEnded = false;
            spriteRenderer.sprite = normalSprite;
            gameObject.SetActive(false);
            return;
        }

        spriteRenderer.sprite = isClicked ? downSprite : normalSprite;
        
        // Debug.Log($"🎨 이빨 {toothIndex} 시각적 업데이트: {(isClicked ? "DOWN" : "UP")}");
    }

    private void OnMouseDown()
    {
        if (isClicked || isGameEnded) return;
        OnToothClicked?.Invoke(toothIndex);
    }

    public void ProcessClick()
    {
        //Debug.Log($"ProcessClick called on tooth {toothIndex} by {Runner.LocalPlayer.RawEncoded}");
        
        // 이미 클릭되었거나 게임이 끝났으면 무시
        if (isClicked || isGameEnded) return;
        
        int playerId = Runner.LocalPlayer.RawEncoded;
        RPC_RequestClickTooth(playerId);
        
        // Debug.Log($"✅ 이빨 {toothIndex} 클릭 요청 전송됨");
    }
    

    public bool IsClicked() => isClicked;
}