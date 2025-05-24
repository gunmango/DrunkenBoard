using System.Collections;
using UnityEngine;

public class ReadyPopup : ABasePopup
{
    [SerializeField] private ReadyPopupUpdater updater;

    private void Awake()
    {
        updater.StartButton.onClick.AddListener(()=> GameManager.PopupManager.CloseTopPopup());
    }

    private IEnumerator Start()
    {
        yield return new WaitWhile(()=> MainGameSceneManager.GameStateManager.IsSpawned == false);
        GameManager.PopupManager.OpenPopup(this);
    }
    
    public override void Open()
    {
        if(MainGameSceneManager.GameStateManager.CurrentState != EMainGameState.Ready)
            return;
        updater.gameObject.SetActive(true);
    }

    public override void Close()
    {
        updater.gameObject.SetActive(false);
        MainGameSceneManager.GameStateManager.ChangeState(EMainGameState.Board);
    }
}
