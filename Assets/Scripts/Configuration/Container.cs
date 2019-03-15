using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Container : MonoBehaviour, IPointerEnterHandler
{
    public Text infoText;
    public Image background;
    [Multiline]
    public string description;

    abstract public string RetrieveContent();
    
    abstract public void SetContent(string content);
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        infoText.text = description;
    }

    public void Flash(Color flashColor)
    {
        StartCoroutine(Animation(flashColor));
    } 

    private IEnumerator Animation(Color flashColor)
    {
        // Default background color
        Color color = new Color(168f / 255, 168f / 255, 168f / 255, 125f / 255);

        // Flash to red
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            background.color = new Color(flashColor.r, flashColor.g, flashColor.b, i);
            yield return null;
        }
        // Flash back
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            background.color = new Color(color.r, color.g, color.b, i);
            yield return null;
        }
    }
}