using System;
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
         ApartGameManager.Instance.RaiseFloorsTo(number);
         ApartGameManager.Instance.HighlightFloor(number);
         
         apartPlayerManager.NextPlayer();
      }
      else
      {
         Debug.Log("유효한 숫자를 입력해주세요.");
      }
   }

   private void Update()
   {
      if (Input.GetKeyDown(KeyCode.Space))
      {
         ApartTurnManager.Instance.OnSpacePress();
      }
   }
}
