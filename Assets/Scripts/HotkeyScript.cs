using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotkeyScript : MonoBehaviour
{
    public GameObject UI;
    public KeyCode key = KeyCode.Tab;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(key))
        {
            // Togle UI active state
            UI.SetActive(!UI.activeSelf);
        }
    }
}
