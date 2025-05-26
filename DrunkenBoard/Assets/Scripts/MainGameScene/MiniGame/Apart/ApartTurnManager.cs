using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ApartTurnManager : MonoBehaviour
{
    public static ApartTurnManager Instance;

    [SerializeField] private float turnTimeLimit = 5f;
    public int SpacePressNo = 2;
    public int SpacePressCount = 0;
    private bool isTurnActive = false; 
    int countdown = 3;
    
    public Text timerText;

    private void Awake()
    {
        Instance = this;
        timerText.gameObject.SetActive(false);
   
    }

    public void StartTurn()
    {
       
        StartCoroutine(StartTurnCoroutine());
    }

    private IEnumerator StartTurnCoroutine()
    {
        timerText.gameObject.SetActive(true);

        for (int i = countdown; i > 0; i--)
        {
            timerText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        timerText.text = "START";
        yield return new WaitForSeconds(0.2f);
        
        int playerCount = ApartPlayerManager.Instance.players.Count;
        SpacePressNo = playerCount * 2;
        
        SpacePressCount = 0;
        isTurnActive = true;
        StartCoroutine(TurntimerCoroutine());
        
    }
    private IEnumerator TurntimerCoroutine()
    {
     
        float timer = turnTimeLimit;
        
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.RoundToInt(timer).ToString();
            yield return null;
        }
        isTurnActive = false;
        
        if (SpacePressCount < SpacePressNo)
        {
            int missingSpacePressCount = SpacePressNo - SpacePressCount;
            ApartGameManager.Instance.AddRandomFloors(missingSpacePressCount);
        }
        timerText.text = "END";

        ApartInputManager.Instance.ShowInputUI();
    }

    public void OnSpacePress()
    {   
        if(!isTurnActive) return;
        if (SpacePressCount >= SpacePressNo) return; // 제한 초과 방지
        SpacePressCount++;
        // Color mycolor = GetComponent<ApartPlayer>().PlayerColor;
        ApartGameManager.Instance.AddFloor();

    }
}
