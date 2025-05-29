using System;
using Fusion;
using UnityEngine;

public class Tooth : NetworkBehaviour
{
    [SerializeField] private Sprite upToothSprite;
    [SerializeField] private Sprite downToothSprite;
    [Networked] public bool IsPressed { get; set; }
    [Networked] public bool Istrap { get; set; }

    public CrocodileGameManager gameManager;
    
    private SpriteRenderer _toothRenderer;
    public Crocodile Croc;
    
    private bool _isSpawned = false;

    private void Awake()
    {
        _toothRenderer = GetComponent<SpriteRenderer>();
        // if (_toothRenderer == null)
        // {
        //     Debug.LogError("Tooth: SpriteRenderer 컴포넌트가 없습니다!");
        // }
        // else
        // {
        //     Debug.Log("Tooth: SpriteRenderer 정상 할당됨");
        // }
    }

    public override void Spawned()
    {
        base.Spawned();

        if (HasStateAuthority)
        {
            IsPressed = false;
            Istrap = false;

            // 처음엔 권한 제거 또는 초기화
            Object.RemoveInputAuthority();
        }

        ShowTooth();
        _isSpawned = true;
    }
   

    private void OnMouseDown()
    {
        // Debug.Log($"[OnMouseDown] 클릭됨. InputAuthority={Object.HasInputAuthority}, IsPressed={IsPressed}, IsMyTurn={gameManager.IsCurrentPlayerTurn}");

        if (Object.HasInputAuthority && !IsPressed && gameManager.IsCurrentPlayerTurn)
        {
            // Debug.Log($"[OnMouseDown] 조건 통과 → PressTooth 호출");
            gameManager.PressTooth(this);
        }
    }

    public void AssignInputAuthorityTo(Player player, NetworkRunner runner)
    {   
        // Debug.Log($"Tooth {name} 현재 권한: {Object.InputAuthority}, 할당할 대상: {player.Object.InputAuthority}");
        if (player == null || player.Object == null)
        {
            Object.RemoveInputAuthority();
            // Debug.LogWarning("AssignInputAuthorityTo: player 또는 player.Object가 null임");
        }
        else
        {
            // Debug.Log($"AssignInputAuthorityTo: 권한을 플레이어 {player.Uuid} 에게 할당");
            Object.AssignInputAuthority(player.Object.InputAuthority);
        }
    }

    
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_PressVisual()
    {
        // Debug.Log("RPC_PressVisual 호출됨");
        _toothRenderer.sprite = downToothSprite;
        _toothRenderer.color = Color.white;
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestPressTooth()
    {
        // Debug.Log($"[RPC_RequestPressTooth] 실행됨? HasStateAuthority={HasStateAuthority}, IsPressed={IsPressed}");
    
        if (!IsPressed)
        {
            IsPressed = true;
            // Debug.Log("[RPC_RequestPressTooth] 눌림 처리 중");
            RPC_PressVisual();
            OnToothpressed();
        }
    }

    private void OnMouseEnter()
    {
        if (!_isSpawned)
        {
            // Debug.LogWarning("Tooth: Not spawned yet, ignoring mouse enter");
            return;
        }

        if (_toothRenderer == null)
        {
            // Debug.LogWarning("Tooth: _toothRenderer is null!");
            return;
        }
        if(!IsPressed)
            _toothRenderer.color = Color.blue;
    }

    private void OnMouseExit()
    {
        if (!_isSpawned)
        {
            // Debug.LogWarning("Tooth: Not spawned yet, ignoring mouse enter");
            return;
        }

        if (_toothRenderer == null)
        {
            // Debug.LogWarning("Tooth: _toothRenderer is null!");
            return;
        }
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
    
    public void ShowTooth()
    {
        gameObject.SetActive(true);
        _toothRenderer.sprite = upToothSprite;
        _toothRenderer.color = Color.white;
    }
}
