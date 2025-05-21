using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ApartCountdowTimer : MonoBehaviour
{
    public static ApartCountdowTimer Instance;
    
    public Text CountdowText;
    public float countdownTime = 5f;

    private void Awake()
    {
        Instance = this;
    }

    public void StartCountdown()
    {
        StartCoroutine(RunCountdown());
    }

    private IEnumerator RunCountdown()
    {
        float timer = countdownTime;
        while (timer>0)
        {
            CountdowText.text = timer.ToString("F0");
            yield return new WaitForSeconds(1f);
            timer--;
        }
        
        CountdowText.text = "START";
        ApartGameManager.Instance.OnCountdownFinished();
    }
    
}
