using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PathDisplay : MonoBehaviour
{
    public GameObject target;

    private LineRenderer line;
    private int currentPosition = 0;

    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = 0;
        line.loop = false;
    }

    private void Update()
    {
        line.positionCount++;
        line.SetPosition(currentPosition++, target.transform.position);
    }

    private void Clear()
    {
        line.positionCount = 0;
    }
}
