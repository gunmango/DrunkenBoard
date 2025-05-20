using TMPro;
using UnityEngine;

public class UuidListUpdater : MonoBehaviour
{
    [SerializeField] private Transform contentTransform;
    [SerializeField] private TextMeshProUGUI originalText;

    public TextMeshProUGUI CreateUuidText(string uuid)
    {
        TextMeshProUGUI newText = Instantiate(originalText, contentTransform);
        newText.text = uuid;
        newText.gameObject.SetActive(true);
        
        return newText;
    }
}
