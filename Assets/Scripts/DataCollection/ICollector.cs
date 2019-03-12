using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Collects data from the object it's attached to during a trial/practice.
/// </summary>
public interface ICollector
{
    /// <summary>
    /// Begins collecting data from the attached gameobject
    /// </summary>
    void StartCollecting();

    /// <summary>
    /// Stops collecting data from the attached gameobject
    /// </summary>
    void StopCollecting();
}
