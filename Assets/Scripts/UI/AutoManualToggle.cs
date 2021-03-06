﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class AutoManualToggle : Container
{
    public InputField configField;
    public Button loadButton;
    public Button saveButton;
    public Selectable[] settingElements;
    [Header("Ball fields")]
    public Dropdown ballPreset;
    public InputField ballSize;
    public InputField ballMass;
    public InputField ballDrag;

    private bool interactable;
    
    public bool Interactable
    {
        get
        {
            return interactable;
        }
        set
        {
            if (value == true)
            {
                thisSlider.interactable = true;
                OnValueChanged();
            }
            else
            {
                thisSlider.interactable = false;
                // set everything to not interactable until subject is set
                configField.interactable = false;
                loadButton.interactable = false;
                saveButton.interactable = false;
                foreach (Selectable element in settingElements)
                    element.interactable = false;
            }

        }
    }
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

            if (ballPreset.value == 0)
                return;
            ballSize.interactable = false;
            ballMass.interactable = false;
            ballDrag.interactable = false;
        }
    }

    public override void SetContent(string content)
    {
        SetValue(bool.Parse(content));
    }

    public override string RetrieveContent()
    {
        return GetValue().ToString();
    }
}
