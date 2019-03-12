using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour
{
    private int STATE = 0;

    public KeyCode view1 = KeyCode.Alpha1;
    public KeyCode view2 = KeyCode.Alpha2;
    public KeyCode view3 = KeyCode.Alpha3;

    [Header("References")]
    public Canvas UI;
    public Camera UICamera;
    public Camera playerCamera; 

    // Some rough state management out here, but it's stable
    void Update()
    {
        if (Input.GetKeyUp(view1)) // Menu/UI view
        {
            UI.enabled = true;
            
            playerCamera.enabled = false;
            UICamera.enabled = true;
        }
        else if (Input.GetKeyUp(view2)) // Overview camera
        {
            UI.enabled = false;

            playerCamera.enabled = false;
            UICamera.enabled = true;
        }
        else if (Input.GetKeyUp(view3)) // Player camera
        {
            UI.enabled = false;

            playerCamera.enabled = true;
            UICamera.enabled = false;
        }
    }
}