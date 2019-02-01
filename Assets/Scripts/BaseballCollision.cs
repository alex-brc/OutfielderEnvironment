using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseballCollision : MonoBehaviour
{
    public GameObject managerHolder;
    TrialsManager manager;

    void Start()
    {
        manager = managerHolder.GetComponent<TrialsManager>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if we hit the player or not
        bool result = false;
        if (collision.collider.gameObject.name == "PlayerContainer")
            result = true;
        // Finish the trial
        manager.CompleteTrial(result);
    }   
}
