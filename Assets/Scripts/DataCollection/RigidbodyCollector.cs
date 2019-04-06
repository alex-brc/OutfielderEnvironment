using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Collects data from the object it's attached to during a trial/practice.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class RigidbodyCollector : Collector
{
    public override string GetColumns()
    {
        return 
            "Time,Frame," +
            "Position_X,Position_Y,Position_Z," +
            "Velocity_X,Velocity_Y,Velocity_Z\r\n";
    }

    public override object[] GetData()
    {
        return new object[] {
            (Time.time - startingTime),
            (Time.frameCount - startingFrame),
            transform.position.ToCSVFormat(),
            GetComponent<Rigidbody>().velocity.ToCSVFormat()};
    }
    
}
