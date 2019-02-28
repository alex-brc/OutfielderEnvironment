using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPhysics : MonoBehaviour
{
    [Tooltip("The number of previous samples to be used in smoothing the acceleration data.")]
    [Range(1,5)]
    public int sampleSize = 3;


    private Rigidbody rb;
    private Vector3 lastVelocity;
    private Vector3 rawAcceleration;
    private Vector3 filteredAcceleration;
    private Vector3[] prev = new Vector3[10];

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Called every Time.fixedDeltaTime
    void FixedUpdate()
    {
        // Get instantaneous acceleration (unfiltered, noisy)
        rawAcceleration = 1.0f * (rb.velocity - lastVelocity)/Time.fixedDeltaTime;
        lastVelocity = rb.velocity;

        Vector3 sum = new Vector3();
        for(int i = 0; i <= sampleSize - 2; i++)
        {
            prev[i] = prev[i + 1];
            sum += prev[i];
        }
        prev[sampleSize - 1] = rawAcceleration;
        sum += rawAcceleration;

        // Obtain filtered acceleration
        sum /= sampleSize;
        filteredAcceleration = sum;
    }

    /// <summary>
    /// Gives the instantaneous acceleration of this physics object, obtained by differentiating the velocity.
    /// 
    /// This is unfiltered, so it is quite noisy and barely usable.
    /// </summary>
    /// <returns>A Vector3 representing the instantaneous acceleration</returns>
    public Vector3 GetRawAcceleration()
    {
        return rawAcceleration;
    }
    /// <summary>
    /// Returns the filtered acceleration. 
    /// </summary>
    /// <returns></returns>
    public Vector3 GetAcceleration()
    {
        return filteredAcceleration;
    }

    internal void Reset()
    {
        lastVelocity = new Vector3();
        rawAcceleration = new Vector3();
        filteredAcceleration = new Vector3();
    }
}
