using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ManualsToggler : MonoBehaviour
{
    [Header("References")]
    public GameObject[] manualButtons;
    public Button[] autoButtons;

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
            foreach (GameObject button in manualButtons)
            {
                button.SetActive(true);
                button.GetComponent<Button>().interactable = true;
            }

            foreach (Button button in autoButtons)
                button.interactable = false;
        }
        else
        {
            foreach (GameObject button in manualButtons)
                button.SetActive(false);

            foreach (Button button in autoButtons)
                button.interactable = true;
        }
    }
}
