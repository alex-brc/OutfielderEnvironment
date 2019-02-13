using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeTestMover : MonoBehaviour
{
    [Header("References")]
    public FoveInterface fove;
    
    // Update is called once per frame
    void Update()
    {
        transform.position = FoveInterface.GetGazeConvergence().ray.GetPoint(2.0f);
    }
}
