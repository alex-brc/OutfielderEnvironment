using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathDisplay : MonoBehaviour
{
    private LineRenderer line;
    private int currentPosition = 0;

    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = 0;
        line.loop = false;
    }

    public void UpdateLine(Vector3 position)
    {
        if (position == Vector3.zero)
            return;
        line.positionCount++;
        line.SetPosition(currentPosition++, position.XZ());
    }

    public void Clear()
    {
        line.positionCount = 0;
        line.SetPositions(new Vector3[0]);
        currentPosition = 0;
    }

    public void ToggleShow(bool val)
    {
        line.enabled = val;
    }
}
