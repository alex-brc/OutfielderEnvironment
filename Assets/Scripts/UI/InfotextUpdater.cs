using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InfotextUpdater : MonoBehaviour, IPointerEnterHandler
{
    public Text infoText;
    [Multiline]
    public string description;

    public void OnPointerEnter(PointerEventData eventData)
    {
        infoText.text = description;
    }
    
}