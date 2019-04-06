using System;
using UnityEngine;

public class FoveCursor : MonoBehaviour
{
    public void Update()
    {
        FoveInterface.GazeConvergenceData gaze;
        try
        {
            // every frame, update position to where the player is looking
            gaze = FoveInterface.GetGazeConvergence();
        }
        catch (Exception)
        {
            return;
        }

        transform.position = gaze.ray.GetPoint(1.5f); // 3 units in front of camera
    }
}
