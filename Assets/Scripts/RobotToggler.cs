using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotToggler : MonoBehaviour
{
    [Header("References")]
    public Toggle toggler;
    public GameObject OACButton1;


    void Start()
    {
        // Turn it off on start if it isn't
        toggler.isOn = false;
        ToggleValueChanged(false);
    }

    public void ToggleValueChanged(bool on)
    {
        if (on)
        {
            // Show all the buttons
            OACButton1.SetActive(true);
        }
        else
        {
            // Hide all the buttons
            OACButton1.SetActive(false);
        }
    }
}
