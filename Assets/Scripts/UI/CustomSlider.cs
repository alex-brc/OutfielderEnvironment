using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Slider))]
public class CustomSlider : MonoBehaviour
{
    public string floatFormat;
    private Slider thisSlider;
    private Text display;

    public void Awake()
    {
        thisSlider = gameObject.GetComponent<Slider>();
        display = transform.Find("Value").GetComponent<Text>();
    }

    public void SetValue(float value)
    {
        thisSlider.value = value;
        OnValueChanged();
    }

    public float GetValue()
    {
        return thisSlider.value;
    }

    public void OnValueChanged()
    {
        display.text = thisSlider.value.ToString(floatFormat);
    }
}
