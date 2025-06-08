// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Fusion;
//
// public class ApartmentCollectiveSystem : NetworkBehaviour
// {
//     public static ApartmentCollectiveSystem Instance;
//     
//     [Networked, Capacity(8)] public NetworkLinkedList<ApartmentCollectivePlayer> CollectivePlayers => default;
//     [Networked, Capacity(8)] public NetworkLinkedList<ApartmentCollectivePlayer> FinishedPlayers => default;
//     [Networked] private bool IsSystemActive { get; set; }
//     [Networked] public bool IsWaitingForGuess { get; set; }
//
//     public Action<List<ApartmentCollectivePlayer>> ActOnEndSystem { get; set; }
//
//     private void Awake()
//     {
//         Instance = this;
//     }
//
//     public override void Spawned()
//     {
//         gameObject.SetActive(false);
//     }
//
//     [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//     public void StartSystem_RPC(int totalPlayerCount)
//     {
//         if (!Object.HasStateAuthority) return;
//         
//         Debug.Log($"[CollectiveSystem] 시스템 시작 - 총 {totalPlayerCount}명");
//         
//         FinishedPlayers.Clear();
//         IsSystemActive = true;
//         IsWaitingForGuess = false;
//         
//         ActivateSystem_RPC(true);
//         StartCoroutine(PlayersTakeAction(totalPlayerCount));
//     }
//
//     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//     private void ActivateSystem_RPC(bool active)
//     {
//         gameObject.SetActive(active);
//         Debug.Log($"[CollectiveSystem] 시스템 활성화: {active}");
//     }
//
//     private IEnumerator PlayersTakeAction(int totalPlayerCount)
//     {
//         Debug.Log($"[CollectiveSystem] 플레이어 대기 중... 현재: {CollectivePlayers.Count}/{totalPlayerCount}");
//         
//         // 모든 플레이어가 등록될 때까지 대기
//         yield return new WaitUntil(() => CollectivePlayers.Count == totalPlayerCount);
//         
//         Debug.Log($"[CollectiveSystem] 모든 플레이어 준비 완료! 집단 행동 시작");
//         
//         // 카운트다운 표시
//         yield return StartCoroutine(ShowCountdown());
//         
//         // 모든 플레이어에게 집단 행동 시작 신호
//         foreach (var collectivePlayer in CollectivePlayers)
//         {
//             // ApartPlayerManager에서 해당 플레이어 찾기
//             var apartPlayer = ApartPlayerManager.Instance.GetPlayerByUuid(collectivePlayer.Uuid);
//             if (apartPlayer != null && apartPlayer.Object != null)
//             {
//                 apartPlayer.BeginCollectiveAction_RPC(0);
//             }
//         }
//         
//         Debug.Log($"[CollectiveSystem] 모든 플레이어에게 행동 시작 신호 전송");
//     }
//     
//     private IEnumerator ShowCountdown()
//     {
//         ShowCountdownMessage_RPC("3");
//         yield return new WaitForSeconds(1f);
//         
//         ShowCountdownMessage_RPC("2");
//         yield return new WaitForSeconds(1f);
//         
//         ShowCountdownMessage_RPC("1");
//         yield return new WaitForSeconds(1f);
//         
//         ShowCountdownMessage_RPC("START!");
//         yield return new WaitForSeconds(0.5f);
//         
//         HideCountdownMessage_RPC();
//     }
//     
//     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//     private void ShowCountdownMessage_RPC(string message)
//     {
//         Debug.Log($"[Countdown] {message}");
//         // UI에 카운트다운 표시하는 로직 추가 가능
//     }
//     
//     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//     private void HideCountdownMessage_RPC()
//     {
//         Debug.Log("[Countdown] 숨김");
//     }
//     
//     [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//     public void AddCollectivePlayer_RPC(ApartmentCollectivePlayer player)
//     {
//         if (!Object.HasStateAuthority) return;
//         
//         // 중복 체크
//         foreach (var existingPlayer in CollectivePlayers)
//         {
//             if (existingPlayer.PlayerRef == player.PlayerRef)
//             {
//                 Debug.LogWarning($"Player {player.PlayerRef} is already in collective system");
//                 return;
//             }
//         }
//         
//         CollectivePlayers.Add(player);
//         OnPlayerJoinedCollective_RPC(player);
//         
//         Debug.Log($"[CollectiveSystem] 플레이어 추가됨 - UUID: {player.Uuid} ({CollectivePlayers.Count}명)");
//     }
//
//     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//     private void OnPlayerJoinedCollective_RPC(ApartmentCollectivePlayer player)
//     {
//         Debug.Log($"Player {player.PlayerRef} (UUID: {player.Uuid}) joined collective system");
//     }
//     
//     [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//     public void EndAction_RPC(ApartmentCollectivePlayer finishedPlayer)
//     {
//         if (!Object.HasStateAuthority) return;
//         if (!IsSystemActive) return;
//         
//         Debug.Log($"[CollectiveSystem] 플레이어 행동 완료 - UUID: {finishedPlayer.Uuid}, ActionValue: {finishedPlayer.ActionValue}");
//         
//         // 중복 체크
//         bool alreadyFinished = false;
//         foreach (var existing in FinishedPlayers)
//         {
//             if (existing.Uuid == finishedPlayer.Uuid)
//             {
//                 alreadyFinished = true;
//                 break;
//             }
//         }
//         
//         if (alreadyFinished)
//         {
//             Debug.LogWarning($"Player {finishedPlayer.Uuid} has already finished action");
//             return;
//         }
//         
//         FinishedPlayers.Add(finishedPlayer);
//         OnPlayerFinishedAction_RPC(finishedPlayer);
//
//         if (FinishedPlayers.Count == CollectivePlayers.Count)
//         {
//             Debug.Log("[CollectiveSystem] 모든 플레이어 행동 완료! 시스템 종료 준비");
//             StartCoroutine(DelayedEndSystem());
//         }
//     }
//
//     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//     private void OnPlayerFinishedAction_RPC(ApartmentCollectivePlayer player)
//     {
//         Debug.Log($"Player UUID {player.Uuid} finished their action ({FinishedPlayers.Count}/{CollectivePlayers.Count})");
//     }
//     
//     private IEnumerator DelayedEndSystem()
//     {
//         yield return new WaitForSeconds(1f); // 1초 대기
//         EndSpacePhase();
//     }
//
//     private void EndSpacePhase()
//     {
//         Debug.Log("[CollectiveSystem] 스페이스바 단계 종료, 추측 단계 시작");
//         
//         IsWaitingForGuess = true;
//         
//         // 마스터 클라이언트에게 숫자 입력 UI 표시
//         if (Object.HasStateAuthority)
//         {
//             ShowGuessInput_RPC();
//         }
//     }
//     
//     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//     private void ShowGuessInput_RPC()
//     {
//         // 마스터 클라이언트(처음 시작 버튼 누른 사람)에게만 입력 UI 표시
//         if (Object.HasStateAuthority)
//         {
//             Debug.Log("[CollectiveSystem] 숫자 추측 입력 UI 표시 요청");
//             
//             // ApartInputManager를 통해 입력 UI 표시
//             if (ApartInputManager.Instance != null)
//             {
//                 ApartInputManager.Instance.ShowGuessInputUI();
//             }
//         }
//     }
//     
//     // 마스터가 숫자를 입력했을 때 호출
//     [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
//     public void SubmitGuess_RPC(int guessedFloor)
//     {
//         if (!Object.HasStateAuthority) return;
//         if (!IsWaitingForGuess) return;
//         
//         Debug.Log($"[CollectiveSystem] 숫자 추측: {guessedFloor}층");
//         ProcessGuessResult_RPC(guessedFloor);
//     }
//     
//     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//     private void ProcessGuessResult_RPC(int guessedFloor)
//     {
//         // 해당 층의 색상 확인
//         EPlayerColor floorColor = ApartGameManager.Instance.GetFloorColor(guessedFloor);
//         
//         // 해당 색상의 플레이어 찾기
//         ApartPlayer foundPlayer = ApartPlayerManager.Instance.GetPlayerByColor(floorColor);
//         
//         if (foundPlayer != null)
//         {
//             Debug.Log($"[CollectiveSystem] {guessedFloor}층의 주인: {foundPlayer.Uuid} ({floorColor})");
//             ShowResult_RPC(guessedFloor, foundPlayer.Uuid, floorColor, true);
//         }
//         else
//         {
//             Debug.Log($"[CollectiveSystem] {guessedFloor}층에 플레이어가 없음 ({floorColor})");
//             ShowResult_RPC(guessedFloor, -1, floorColor, false);
//         }
//         
//         // 층 하이라이트
//         if (ApartGameManager.Instance != null)
//         {
//             ApartGameManager.Instance.HighlightFloor(guessedFloor);
//         }
//         
//         // 잠시 후 시스템 종료
//         StartCoroutine(DelayedSystemEnd());
//     }
//     
//     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//     private void ShowResult_RPC(int floor, int playerUuid, EPlayerColor color, bool found)
//     {
//         if (found)
//         {
//             Debug.Log($"결과: {floor}층의 주인은 플레이어 {playerUuid} ({color})입니다!");
//         }
//         else
//         {
//             Debug.Log($"결과: {floor}층에는 플레이어가 없습니다 ({color})");
//         }
//     }
//     
//     private IEnumerator DelayedSystemEnd()
//     {
//         yield return new WaitForSeconds(3f); // 3초 대기
//         EndSystem();
//     }
//
//     private void EndSystem()
//     {
//         IsSystemActive = false;
//     
//         List<ApartmentCollectivePlayer> result = new List<ApartmentCollectivePlayer>();
//         foreach (var finishedPlayer in FinishedPlayers)
//         {
//             result.Add(finishedPlayer);
//         }
//     
//         EndSystemComplete_RPC(result.Count);
//     
//         // 결과를 실제 아파트에 반영
//         ApplyCollectiveResults(result);
//     
//         // 마스터에게 최종 답안 입력 UI 표시
//         ShowFinalAnswerUI_RPC();
//     
//         ActOnEndSystem?.Invoke(result);
//     
//         // 시스템 정리
//         FinishedPlayers.Clear();
//         CollectivePlayers.Clear();
//         ActivateSystem_RPC(false);
//     }
//
//     [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
//     private void EndSystemComplete_RPC(int totalFinishedPlayers)
//     {
//         Debug.Log($"Collective system ended with {totalFinishedPlayers} players");
//     }
//
//     public bool IsActive()
//     {
//         return IsSystemActive;
//     }
//
//     public int GetTotalPlayersCount()
//     {
//         return CollectivePlayers.Count;
//     }
//
//     public int GetFinishedPlayersCount()
//     {
//         return FinishedPlayers.Count;
//     }
//     
//     private void ApplyCollectiveResults(List<ApartmentCollectivePlayer> results)
//     {
//         // 각 플레이어의 결과를 아파트에 반영
//         foreach (var result in results)
//         {
//             var apartPlayer = ApartPlayerManager.Instance.GetPlayerByUuid(result.Uuid);
//             if (apartPlayer != null)
//             {
//                 // 플레이어가 누른 횟수만큼 추가로 층 생성
//                 for (int i = 0; i < result.ActionValue; i++)
//                 {
//                     ApartGameManager.Instance.AddFloor(apartPlayer.PlayerColor);
//                 }
//             }
//         }
//     }
//
//     [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
//     private void ShowFinalAnswerUI_RPC()
//     {
//         if (ApartInputManager.Instance != null)
//         {
//             ApartInputManager.Instance.ShowFinalAnswerInput();
//         }
//     }
// }
//
// [System.Serializable]
// public struct ApartmentCollectivePlayer : INetworkStruct
// {
//     public PlayerRef PlayerRef;
//     public int Uuid;
//     public int ActionValue; // 플레이어가 수행할 행동의 값 (예: 스페이스바 누른 횟수)
//     public bool HasTakenAction;
// }
//
