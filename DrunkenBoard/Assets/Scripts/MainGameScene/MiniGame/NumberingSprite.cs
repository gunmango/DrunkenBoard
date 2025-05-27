using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NumberingSprite : MonoBehaviour
{
    public Sprite[] NumberSprites;
    public GameObject NumberPrefab;
    public Transform NumberParent;
    
    private void Start()
    {
        // SetNumber(12345);  // 게임 시작할 때 바로 테스트해보기
    }

    public void SetNumber(int number)
    {
        foreach (Transform child in NumberParent)
            Destroy(child.gameObject);
        
        char[] numberArray = number.ToString().ToCharArray();

        float spacing = 1f;
        float totalWidth = (numberArray.Length - 1) * spacing;
        float startX = -totalWidth / 2f;  // 왼쪽에서 시작하도록 계산

        for (int i = 0; i < numberArray.Length; i++)
        {
            int numbers = numberArray[i] - '0';

            GameObject numberObject = Instantiate(NumberPrefab, NumberParent);
            numberObject.SetActive(true);

            float xPos = startX + i * spacing;
            numberObject.transform.localPosition = new Vector3(xPos, 0, 0);
            numberObject.transform.localScale = Vector3.one;

            SpriteRenderer numberImage = numberObject.GetComponent<SpriteRenderer>();

            if (numberImage == null) continue;
            if (numbers < 0 || numbers >= NumberSprites.Length) continue;

            numberImage.sprite = NumberSprites[numbers];
        }
    }
}
