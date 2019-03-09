using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class AutoManualToggle : MonoBehaviour
{
    public InputField configField;
    public Button loadButton;
    public Button saveButton;
    public BallPresetDropdown presets;
    public Selectable[] settingElements;
    
    private Slider thisSlider;

    public void Awake()
    {
        thisSlider = gameObject.GetComponent<Slider>();
    }

    public void SetValue(bool auto)
    {
        if (auto)
            thisSlider.value = 0;
        else
            thisSlider.value = 1;
        OnValueChanged();
    }

    public bool GetValue()
    {
        if (thisSlider.value == 0)
            return true;
        else
            return false;
    }

    public void OnValueChanged()
    {
        if (thisSlider.value == 1) // MANUAL
        {
            configField.interactable = true;
            loadButton.interactable = true;
            saveButton.interactable = false;
            foreach (Selectable element in settingElements)
                element.interactable = false;
        }
        else if(thisSlider.value == 0) // AUTO
        {
            configField.interactable = false;
            loadButton.interactable = false;
            saveButton.interactable = true;
            foreach (Selectable element in settingElements)
                element.interactable = true;
            presets.OnValueChanged();
        }
    }


}
