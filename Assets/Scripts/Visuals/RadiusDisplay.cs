using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component is used as a tool to draw a circle of specified radius
/// around the projection of this object on the XZ plane. 
/// 
/// It was used to help debug the alpha radius given by the OAC strategy.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class RadiusDisplay : MonoBehaviour
{
    public GameObject target;
    
    [Range(0, 50)]
    public int segments = 50;

    internal float radius = 5;
    private LineRenderer line;

    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        line.positionCount = segments + 1;
    }

    void Update()
    {
        DrawCircle();
    }

    private void DrawCircle()
    {
        float x,z;
        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = target.transform.position.x + Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            z = target.transform.position.z + Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            line.SetPosition(i, new Vector3(x, 0, z));

            angle += (360f / segments);
        }
    }
}
