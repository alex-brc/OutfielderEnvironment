using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour
{
    private const float P_X = 2f / 3f;
    private const float P_Y = 5f / 6f;
    private const float P_Z = 2f / 3f;

    private int STATE = 0;

    public KeyCode view1 = KeyCode.Alpha1;
    public KeyCode view2 = KeyCode.Alpha2;
    public KeyCode view3 = KeyCode.Alpha3;

    [Header("References")]
    public CanvasGroup UI;
    public Camera UICamera;
    public Camera playerCamera; 

    // Some rough state management out here, but it's stable
    void Update()
    {
        if (Input.GetKeyUp(view1)) // Menu/UI view
        {
            UIManager.Show(UI);
            
            playerCamera.enabled = false;
            UICamera.enabled = true;
        }
        else if (Input.GetKeyUp(view2)) // Overview camera
        {
            UIManager.Hide(UI);

            playerCamera.enabled = false;
            UICamera.enabled = true;
        }
        else if (Input.GetKeyUp(view3)) // Player camera
        {
            UIManager.Hide(UI);

            playerCamera.enabled = true;
            UICamera.enabled = false;
        }
    }

    public void Refresh(float distance)
    {
        // Move camera depending on the starting distance
        // this transform
        transform.position = new Vector3(distance * P_X, distance * P_Y, distance * P_Z);
    }
}