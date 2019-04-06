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
        if(background != null)
            StartCoroutine(Animation(flashColor));
    } 

    private IEnumerator Animation(Color flashColor)
    {
        // Default background color
        Color color = new Color(200f / 255, 200f / 255, 200f / 255, 0.49f);
        float rDiff = flashColor.r - 200f/255;
        float gDiff = flashColor.g - 200f/255;
        float bDiff = flashColor.b - 200f/255;

        // Fade to flashColor
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            background.color = new Color(200f / 255 + i * rDiff, 200f / 255 + i * gDiff, 200f / 255 + i * bDiff, 0.49f);
            yield return null;
        }
        // Fade back
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            background.color = new Color(flashColor.r - i * rDiff, flashColor.g - i * gDiff, flashColor.b - i * bDiff, 0.49f);
            yield return null;
        }
    }
}