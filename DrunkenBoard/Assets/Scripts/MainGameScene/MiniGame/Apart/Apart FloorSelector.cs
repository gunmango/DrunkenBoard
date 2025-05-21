using System;
using UnityEngine;
using UnityEngine.UI;

public class ApartFloorSelector : MonoBehaviour
{
    public GameObject inputPanel;
    public InputField floorInput;
    public Button confirmButton;

    private void Start()
    {
        inputPanel.SetActive(false);
        confirmButton.onClick.AddListener(selectFloor);
    }

    public void ShowInputUI()
    {
        inputPanel.SetActive(true);
        floorInput.text = "";
    }

    private void OnconfirClick()
    {
        string inputTexT = floorInput.text;

        if (int.TryParse(inputTexT, out int floorNumber))
        {
            if (floorNumber <= 0) 
                return;
            ApartBuilder.Instance.ReshuffleFloors(floorNumber);
            inputPanel.SetActive(false);
        }
    }

    private void selectFloor()
    {
        if (int.TryParse(floorInput.text, out int floorNumber))
        {
            ApartBuilder.Instance.HighlightFloor(floorNumber);
        }
    }
}
