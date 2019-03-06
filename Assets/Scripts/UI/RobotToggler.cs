using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class RobotToggler : MonoBehaviour
{
    [Header("References")]
    public GameObject[] buttons;

    private Toggle toggler;
    void Start()
    {
        toggler = transform.GetComponent<Toggle>();
        // Turn it off on start if it isn't
        toggler.isOn = false;
        ToggleValueChanged(false);
    }

    public void ToggleValueChanged(bool on)
    {
        if (on)
        {
            foreach (GameObject button in buttons)
                button.SetActive(true);
        }
        else
        {
            foreach (GameObject button in buttons)
                button.SetActive(false);
        }
    }
}
