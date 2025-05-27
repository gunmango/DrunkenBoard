using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ApartInputManager : MonoBehaviour
{
   public static ApartInputManager Instance;

   [SerializeField] private GameObject ApartPlayerManager;

   public GameObject InputPanel;
   public InputField NumberInputField;
   public Button CheckeButton;
   private ApartPlayerManager apartPlayerManager;

   private void Awake()
   {
      Instance = this;
   }

   private void Start()
   {
      InputPanel.SetActive(false);
      CheckeButton.onClick.AddListener(OnClicked);
      apartPlayerManager = ApartPlayerManager.GetComponent<ApartPlayerManager>();
   }

   public void ShowInputUI()
   {
      NumberInputField.text = "";
      InputPanel.SetActive(true);
   }

   private void OnClicked()
   {
      if (int.TryParse(NumberInputField.text, out int number))
      {
         InputPanel.SetActive(false);
         ApartGameManager.Instance.RaiseFloorsTo(number, () =>
         {
            // 층 다 쌓고 나서 코루틴으로 한 프레임 기다린 후 Highlight
            StartCoroutine(DelayedHighlight(number));
         });
         
         apartPlayerManager.NextPlayer();
      }
      else
      {
         Debug.Log("유효한 숫자를 입력해주세요.");
      }

      
   }
   private IEnumerator DelayedHighlight(int floorNumber)
   {
      yield return null; // 한 프레임 기다려서 층이 모두 등록되게 함
      ApartGameManager.Instance.HighlightFloor(floorNumber);
   }

   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.Space))
      {
         ApartTurnManager.Instance.OnSpacePress();
      }
   }
}
