using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script simply copies the position and orientation of 
/// a given camera to the one this is attached to.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CopyCamera : MonoBehaviour
{
    public Transform originalCamera;

    void Update()
    {
        transform.position = originalCamera.position;
        transform.rotation = originalCamera.rotation;
    }
    
}
