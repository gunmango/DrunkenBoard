using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopupManager : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;
    
    private readonly Stack<ABasePopup> _popupStack = new();
    public bool IsAnyPopupOpen => _popupStack.Count > 0;

    public void OpenPopup(ABasePopup popup)
    {
        popup.Open();
        _popupStack.Push(popup);
    }

    public void CloseTopPopup()
    {
        if (_popupStack.TryPeek(out var popup))
        {
            popup.Close();
            _popupStack.Pop();
        }
    }

    public void CloseAllPopups()
    {
        while (_popupStack.Count > 0)
        {
            var popup = _popupStack.Pop();
            popup.Close();
        }
    }
    
    public void ToggleInteraction(bool isOn)
    {
        eventSystem.enabled = isOn;
    }}
