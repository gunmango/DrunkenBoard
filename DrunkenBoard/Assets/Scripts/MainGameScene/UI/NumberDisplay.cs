using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class NumberDisplay : MonoBehaviour
{
    [SerializeField] private List<Image> digitImages; // 자릿수별 Image (ex: 3자리 숫자면 3개)
    [SerializeField] private Sprite[] digitSprites;   // 0~9 숫자 이미지들

    private int currentValue = 0;

    public void SetNumber(int value)
    {
        currentValue = Mathf.Clamp(value, 0, Mathf.FloorToInt(Mathf.Pow(10, digitImages.Count)) - 1);
        
        string numberStr = currentValue.ToString().PadLeft(digitImages.Count, '0');

        for (int i = 0; i < digitImages.Count; i++)
        {
            int digit = numberStr[i] - '0';
            digitImages[i].sprite = digitSprites[digit];
        }
    }
    
    public void Increase()
    {
        SetNumber(currentValue + 1);
    }
}
