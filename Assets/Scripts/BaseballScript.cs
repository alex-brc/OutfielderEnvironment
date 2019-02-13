using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CustomPhysics))]
public class BaseballScript : MonoBehaviour
{
    [Header("References")]
    public TrialsManager manager;

    private CustomPhysics physics;

    private void Start()
    {
        physics = gameObject.GetComponent<CustomPhysics>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if we hit the player or not
        bool result = false;
        if (collision.collider.gameObject.tag == "Catcher")
            result = true;
        // Finish the trial and clean the physics sampler
        manager.CompleteTrial(result);
        physics.Reset();
    }   
}
