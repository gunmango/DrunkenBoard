using UnityEngine;

public abstract class ABasePopup : MonoBehaviour
{
    public bool IsOpen { get; private set; }

    public abstract void Open();
    public abstract void Close();
    public virtual void Set(object data)
    {
        
    }
}
