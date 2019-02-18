using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotkeyScript : MonoBehaviour
{
    [Header("References")]
    public GameObject UI;
    public GameObject overlay;
    public KeyCode toggleUIKey = KeyCode.Tab;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(toggleUIKey))
        {
            // Toggle UI active state
            UI.SetActive(!UI.activeSelf);
            // Toggle overlay
            overlay.SetActive(!overlay.activeSelf);
        }
    }
}
