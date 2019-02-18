using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotToggler : MonoBehaviour
{
    [Header("References")]
    public Toggle toggler;
    public GameObject[] buttons;


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
            foreach (GameObject go in buttons)
                go.SetActive(true);
        }
        else
        {
            foreach (GameObject go in buttons)
                go.SetActive(false);
        }
    }
}
