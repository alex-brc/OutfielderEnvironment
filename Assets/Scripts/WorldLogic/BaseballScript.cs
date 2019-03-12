using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseballScript : MonoBehaviour
{
    [Header("References")]
    public TrialsManager manager;

    void OnCollisionEnter(Collision collision)
    {
        // Check if we hit the player or not
        bool result = false;
        if (collision.collider.gameObject.tag == "Catcher")
            result = true;
        // Finish the trial and clean the physics sampler
        manager.CompleteTrial(result);
    }   
}
