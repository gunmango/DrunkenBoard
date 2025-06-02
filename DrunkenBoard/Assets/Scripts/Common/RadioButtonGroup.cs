using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RadioButtonGroup: MonoBehaviour
{
    [SerializeField] private SimpleRadioButton[] _buttons = null;
    public SimpleRadioButton SelectedButton { get; private set; } = null;
    public int SelectedIndex { get; private set; } = -1;  //-1 선택안됨
    public UnityAction<int> OnValueChanged { get; set; }    // 선택된 값이 바뀔때 발생하는 이벤트

    private void Start()
    {
        for (int i = 0; i < _buttons.Length; i++)
        {
            int index = i;
            _buttons[i].ActOnClick += () =>
            {
                SetSelectedButton(index);
            };
        }    
    }

    private void SetSelectedButton(int selectedIndex)
    {
        // 눌렀던 거 다시 누를 때
        if (SelectedButton == _buttons[selectedIndex])
        {
            SelectedButton.PlayDeSelectAnimation();
            SelectedButton = null;
            SelectedIndex = -1;
        }
        else
        {
            // 다른 거 누를 때
            if(SelectedButton != null)
                SelectedButton.PlayDeSelectAnimation();
        
            SelectedButton = _buttons[selectedIndex];
            SelectedIndex = selectedIndex;
            SelectedButton.PlaySelectAnimation();
        }

        OnValueChanged?.Invoke(SelectedIndex);
    }

    public void EnableAllButtons()
    {
        foreach (var button in _buttons)
        {
            button.enabled = true;
        }
    }

    public void DisableAllButtons()
    {
        foreach (var button in _buttons)
        {
            button.enabled = false;
        }
    }
}