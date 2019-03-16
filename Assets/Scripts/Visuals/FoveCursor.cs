using System;
using UnityEngine;

public class FoveCursor : MonoBehaviour
{
    public void Update()
    {
        try
        {
            // every frame, update position to where the player is looking
            FoveInterface.GazeConvergenceData gaze = FoveInterface.GetGazeConvergence();
            transform.position = transform.position + gaze.ray.direction * gaze.distance;
        }
        catch (Exception)
        {
            // nada
        }
    }
}
