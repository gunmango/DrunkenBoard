using UnityEngine;

public class CrocodileEvent : ASpaceEvent
{
    [SerializeField] private CrocodileGameManager crocodileGameManager;
    [SerializeField] private GameObject crocodile;
    protected override void PlayEvent()
    {
        Debug.Log("CrocodileEvent");
        crocodile.SetActive(true);
        crocodileGameManager.SetPlayerAndStart();
        MainGameSceneManager.GameStateManager.ActOnBoard += OnEndEvent;
    }

    private void OnEndEvent()
    {
        MainGameSceneManager.GameStateManager.ActOnBoard -= OnEndEvent;
        crocodile.SetActive(false);
    }
}
