using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICatcher
{
    /// <summary>
    /// Called in FixedUpdate(), this should implement mobility for the catcher.
    /// </summary>
    void Move();

    /// <summary>
    /// Called on trial completion, this should send it to its home position.
    /// </summary>
    void SendHome();

    /// <summary>
    /// Returns the rigidbody of this catcher.
    /// </summary>
    Rigidbody GetRigidbody();
    
    void StartDataCollector();
    void StopDataCollector();

}
