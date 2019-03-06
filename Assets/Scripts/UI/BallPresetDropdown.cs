using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class BallPresetDropdown : MonoBehaviour
{
    public Selectable[] ballFields;

    private Dropdown thisDropdown;

    private void Start()
    {
        thisDropdown = gameObject.GetComponent<Dropdown>();
    }

    public void OnValueChanged()
    {
        if (thisDropdown.value == 0) // Custom
            foreach (Selectable element in ballFields)
                element.interactable = true;
        else
            foreach (Selectable element in ballFields)
                element.interactable = false;

        // THIS NEEDS FURTHER WORK

    }
}
