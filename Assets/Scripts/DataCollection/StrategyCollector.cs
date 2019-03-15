using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Collects data from the object it's attached to during a trial/practice.
/// </summary>
[RequireComponent(typeof(IStrategy))]
public class StrategyCollector : Collector
{
    public PathDisplay path;
    private IStrategy strategy;
    private bool first;
    
    public override string GetColumns()
    {
        return 
            "Time,Frame," +
            "Position_X,Position_Y,Position_Z\r\n";
    }

    public override object[] GetData()
    {
        return new object[] {
            (Time.time - startingTime),
            (Time.frameCount - startingFrame),
            strategy.GetPrediction().ToCSVFormat()
            };
    }
    
    new void Start()
    {
        base.Start();

        // Grab strategy
        strategy = GetComponent<IStrategy>();
        first = false;
    }

    new public void StartCollecting()
    {
        base.StartCollecting();

        // Clear the strategy path display
        path.Clear();
    }

    new public void StopCollecting()
    {
        base.StopCollecting();

        // Skip one write on the next trial
        first = true;
    }

    new void FixedUpdate()
    {
        // If the writer is off don't write anything. 
        // This is only a thing inside the editor for testing purposes.
        if (!dataManager.isActive())
            return;

        if (dataManager.Running() && strategy.IsReady())
        {
            // We need to skip one write on subsequent trials, 
            // since it has the position from the last run. 
            if(first)
            {
                first = false;
                return;
            }
            // Add this to be written
            stringBuilder.Append(Record());
            // Also add position to the path
            path.UpdateLine(strategy.GetPrediction());
        }
    }
}
