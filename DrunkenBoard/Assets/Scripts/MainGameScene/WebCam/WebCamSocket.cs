using UnityEngine;

public class WebCamSocket : MonoBehaviour
{
    [SerializeField] private NumberDisplay cycleCount;
    [SerializeField] private NumberDisplay drinkCount;

    public void IncreaseCycleCount() => cycleCount.Increase();
    public void IncreaseDrinkCount() => drinkCount.Increase();
}
