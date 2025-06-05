using UnityEngine;

public interface IBasePopup
{
    public void Open(PopupDataBase data = null);
    public void Close();
}
