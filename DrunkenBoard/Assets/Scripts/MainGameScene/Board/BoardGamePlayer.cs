using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BoardGamePlayer : ATurnPlayer
{
    [SerializeField] private Button rollDiceButton;
    private DiceSetter _diceSetter = null;
    private DiceDisplayer _diceDisplayer = null;
    private PlayerPiece _piece = null;
    private bool _clicked = false;
    private int _diceResult = 0;

    public void Initialize(DiceSetter diceSetter, DiceDisplayer diceDisplayer, TurnSystem turnSystem, int uuid)
    {
        TurnSystem = turnSystem;
        _diceSetter = diceSetter;
        _diceDisplayer = diceDisplayer;
        Uuid = uuid;
    }

    public void SetPiece(PlayerPiece piece)
    {
        _piece = piece;
    }
    
    public override void Spawned()
    {
        base.Spawned();
        rollDiceButton.onClick.AddListener(()=>
        {
            rollDiceButton.gameObject.SetActive(false);
            _clicked = true;
            _diceDisplayer.IsRolling = true;
            _diceSetter.SetRandomResult_RPC();
        });
        rollDiceButton.gameObject.SetActive(false);
    }
    
    protected override IEnumerator TakeTurnCoroutine()
    {
        //주사위 굴리고
        yield return new WaitWhile(() => _diceDisplayer.IsRolling);
        rollDiceButton.gameObject.SetActive(true);
        
        yield return new WaitUntil(() => _clicked);
        yield return new WaitWhile(() => _diceDisplayer.IsRolling);
        
        //말 움직이기
        _diceResult = _diceSetter.DiceResult;
        yield return StartCoroutine(_piece.MoveSpace(_diceResult));
        
        //스페이스 이벤트 기다리기
        bool ready = false;
        MainGameSceneManager.GameStateManager.ActOnBoard += () => ready = true;
        yield return new WaitUntil(() => ready);
        
        
        MainGameSceneManager.GameStateManager.ActOnBoard -= () => ready = true;
        _clicked = false;
        EndTurn();
    }
}
