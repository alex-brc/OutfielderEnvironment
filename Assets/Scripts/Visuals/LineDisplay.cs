using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineDisplay : MonoBehaviour
{
    internal Vector3 pointA, pointB;

    private LineRenderer line;

    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = 2;

        pointA = pointB = Vector3.zero;
    }

    void Update()
    {
        DrawLine();
    }

    private void DrawLine()
    {
        line.SetPosition(0, pointA);
        line.SetPosition(1, pointB);
    }
}
