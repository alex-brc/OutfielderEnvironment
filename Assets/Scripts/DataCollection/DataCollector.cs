using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Collects data from the object it's attached to.
public class DataCollector : MonoBehaviour
{
    public enum Type { GameObject, Fove };

    public Type type;

    [Header("References")]
    public DataWriter writer;
    public FoveInterface fove;
    
    // Send data to the writer every physics update.
    void Update()
    {
        // If the writer is off don't write anything. This is only a thing inside the editor for testing purposes.
        if (!writer.isActive())
            return;
        
        switch (type)
        {
            case Type.GameObject:
                writer.Record(gameObject, Time.time);
                break;
            case Type.Fove:
                writer.RecordFOVE(fove, Time.time);
                break;

            default: goto case Type.GameObject;
        }
    }
}
