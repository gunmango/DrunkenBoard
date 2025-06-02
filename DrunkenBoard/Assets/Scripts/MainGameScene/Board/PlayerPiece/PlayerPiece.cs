using System;
using System.Collections;
using DG.Tweening;
using Fusion;
using UnityEngine;

public class PlayerPiece : NetworkBehaviour, IDespawned
{
    [Networked] public int Uuid { get; set; }
    
    [SerializeField] private PlayerPieceAnimator animator;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private EPlayerColor playerColor;
    [SerializeField] private SpriteRendererTweener spriteRendererTweener;
    
    public int SpaceIndex { get; private set; } = 0;  //보드게임 몇번째 칸에 있는지
    
    private int _spotIndex = -1;  //한 칸 안의 위치

    public override void Spawned()
    {
        _spotIndex = Mathf.Clamp((int)playerColor - 1, 0, MainGameSceneManager.Board.GetSpaceSpotCount(0) - 1);

        MainGameSceneManager.GameStateManager.ActOnBoard += spriteRendererTweener.FadeIn;
        MainGameSceneManager.GameStateManager.ActOnSpaceEvent += spriteRendererTweener.FadeOut;
    }

    public IEnumerator MoveSpace(int spaceCount)
    {
        while (spaceCount > 0)
        {
            SpaceIndex++;

            if (SpaceIndex >= MainGameSceneManager.Board.GetSpaceCount)
            {
                SpaceIndex = 0;
                OnCycleCompleted();
            }
            
            spaceCount--;
            
            Vector3 targetPos = MainGameSceneManager.Board.GetSpaceSpotPosition(SpaceIndex, _spotIndex);
            
            //애니메이션
            Vector3 dir = (targetPos - transform.position).normalized;
            animator.SetMovement(dir);
            
            //이동
            float distance = Vector3.Distance(transform.position, targetPos);
            float duration = distance / moveSpeed;
            
            Tween tween = transform.DOMove(targetPos, duration).SetEase(Ease.Linear).SetUpdate(UpdateType.Late);

            yield return tween.WaitForCompletion();
        }
        
        MainGameSceneManager.Board.PlaySpaceEvent(SpaceIndex, Uuid);
    }

    private void OnCycleCompleted()
    {
        //1바퀴 완주
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        MainGameSceneManager.GameStateManager.ActOnBoard -= spriteRendererTweener.FadeIn;
        MainGameSceneManager.GameStateManager.ActOnSpaceEvent -= spriteRendererTweener.FadeOut;
    }
}
