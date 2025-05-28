using System;
using System.Collections;
using DG.Tweening;
using Fusion;
using UnityEngine;

public class PlayerPiece : NetworkBehaviour
{
    [SerializeField] private PlayerPieceAnimator animator;
    [SerializeField] private float moveSpeed = 2f;
    public int SpaceIndex { get; private set; } = 0;
    
    private int _spotIndex = -1;

    public override void Spawned()
    {
        //_spotIndex 설정
        _spotIndex = 0;
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
            float distance = Vector3.Distance(transform.position, targetPos);
            float duration = distance / moveSpeed;

            Tween tween = transform.DOMove(targetPos, duration).SetEase(Ease.Linear).SetUpdate(UpdateType.Late);

            yield return tween.WaitForCompletion();
        }
    }

    private void OnCycleCompleted()
    {
        //1바퀴 완주
    }
}
