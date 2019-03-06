using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class AutoManualToggle : MonoBehaviour
{
    public InputField configField;

    public Selectable[] settingElements;

    private Slider thisSlider;

    private void Start()
    {
        thisSlider = gameObject.GetComponent<Slider>();
    }

    public void OnValueChanged()
    {
        if (thisSlider.value == 1) // MANUAL
        {
            configField.interactable = true;
            foreach (Selectable element in settingElements)
                element.interactable = false;
        }
        else if(thisSlider.value == 0) // AUTO
        {
            configField.interactable = false;
            foreach (Selectable element in settingElements)
                element.interactable = true;
        }
    }


}
