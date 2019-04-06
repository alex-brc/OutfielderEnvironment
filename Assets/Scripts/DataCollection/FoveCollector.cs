using Fove.Managed;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Collects data from the object it's attached to during a trial/practice.
/// </summary>
[RequireComponent(typeof(FoveInterface))]
public class FoveCollector : Collector
{
    public Transform ball;
    public Transform cursor;

    public override string GetColumns()
    {
        return 
            "Time,Frame," +
            "LeftEyeClosed,RightEyeClosed," +
            "BallScreenPosition_X,BallScreenPosition_Y," +
            "GazeScreenPosition_X,GazeScreenPosition_Y," +
            "HeadsetPosition_X,HeadsetPosition_Y,HeadsetPosition_Z," +
            "HeadsetFacing_X,HeadsetFacing_Y,HeadsetFacing_Z," +
            "LeftEyeVector_X,LeftEyeVector_Y,LeftEyeVector_Z," +
            "RightEyeVector_X,RightEyeVector_Y,RightEyeVector_Z," +
            "GazeDirection_X,GazeDirection_Y,GazeDirection_Z," +
            "GazePoint_X,GazePoint_Y,GazePoint_Z," +
            "GazeAccuracy\r\n";
    }

    public override object[] GetData()
    {
        // Get eye status
        bool leftEyeClosed = false;
        bool rightEyeClosed = false;
        switch (FoveInterface.CheckEyesClosed())
        {
            case EFVR_Eye.Both:
                leftEyeClosed = rightEyeClosed = true;
                break;
            case EFVR_Eye.Left:
                leftEyeClosed = true;
                break;
            case EFVR_Eye.Right:
                rightEyeClosed = true;
                break;
            default: // neither
                break;
        }

        // Get gaze vectors
        Vector3 leftEye = FoveInterface.GetLeftEyeVector();
        Vector3 rightEye = FoveInterface.GetRightEyeVector();
        
        // Get convergence data
        FoveInterface.GazeConvergenceData gazeConvergenceData = FoveInterface.GetGazeConvergence();

        // Get screen points
        Vector3 ballToScreen = GetComponent<Camera>().WorldToScreenPoint(ball.position);
        Vector3 gazeToScreen = GetComponent<Camera>().WorldToScreenPoint(cursor.position);


        // Make the list
        return new object[] {
            (Time.time - startingTime),
            (Time.frameCount - startingFrame),
            leftEyeClosed, rightEyeClosed,
            ballToScreen.x, ballToScreen.y,
            gazeToScreen.x, gazeToScreen.y,
            transform.position.ToCSVFormat(),
            transform.forward.ToCSVFormat(),
            transform.TransformDirection(leftEye).ToCSVFormat(),
            transform.TransformDirection(rightEye).ToCSVFormat(),
            gazeConvergenceData.ray.direction.ToCSVFormat(),
            gazeConvergenceData.ray.GetPoint(gazeConvergenceData.distance).ToCSVFormat(),
            gazeConvergenceData.accuracy // Largely useless
        };
    }
}
