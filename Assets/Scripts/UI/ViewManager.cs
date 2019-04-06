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
    public Camera copyCamera;
    public Camera foveCamera;
    public Light lightSource;

    internal Configurable<CVector> skyboxColor = new Configurable<CVector>("0,149,255");
    internal Configurable<bool> skyBoxOn = new Configurable<bool>(false);
    internal Configurable<CVector> lightColor = new Configurable<CVector>("255,244,214");
    internal Configurable<CVector> lightRotation = new Configurable<CVector>("75,-90,0");

    // Some rough state management out here, but it's stable
    void Update()
    {
        if (Input.GetKeyUp(view1)) // Menu/UI view
        {
            UIManager.Show(UI);
            
            copyCamera.enabled = false;
            UICamera.enabled = true;
        }
        else if (Input.GetKeyUp(view2)) // Overview camera
        {
            UIManager.Hide(UI);

            copyCamera.enabled = false;
            UICamera.enabled = true;
        }
        else if (Input.GetKeyUp(view3)) // Player camera
        {
            UIManager.Hide(UI);

            copyCamera.enabled = true;
            UICamera.enabled = false;
        }
    }

    public void ResetView()
    {
        UIManager.Show(UI);

        copyCamera.enabled = false;
        UICamera.enabled = true;
    }

    public void Refresh(float distance)
    {
        // Move camera depending on the starting distance
        // this transform
        transform.position = new Vector3(distance * P_X, distance * P_Y, distance * P_Z);
    }

    public void SetSkyboxOn()
    {
        if (skyBoxOn.Get())
        {
            UICamera.clearFlags = CameraClearFlags.Skybox;
            foveCamera.clearFlags = CameraClearFlags.Skybox;
            copyCamera.clearFlags = CameraClearFlags.Skybox;
        }
        else
        {
            UICamera.clearFlags = CameraClearFlags.SolidColor;
            foveCamera.clearFlags = CameraClearFlags.SolidColor;
            copyCamera.clearFlags = CameraClearFlags.SolidColor;
        }
    }

    public void SetSkyboxColor()
    {
        Vector3 v = skyboxColor.Get();
        Color c = new Color(v.x / 255, v.y / 255, v.z / 255);

        UICamera.backgroundColor = c;
        foveCamera.backgroundColor = c;
        copyCamera.backgroundColor = c;
    }

    public void SetLightColor()
    {
        Vector3 v = lightColor.Get();
        Color c = new Color(v.x / 255, v.y / 255, v.z / 255);

        lightSource.color = c;

    }

    public void SetLightRotation()
    {
        lightSource.transform.localEulerAngles = lightRotation.Get();
    }
}