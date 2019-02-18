using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlViewBuilder : MonoBehaviour
{
    public TrialsManager manager;
    public GameObject testGroupPrefab;

    void Start()
    {
        for(int i = 0; i < manager.testCases.Length; i++)
        {
            // Make one
            GameObject go = Instantiate(testGroupPrefab);
            // Set its parent to the tests group
            go.transform.SetParent(manager.testsGroup.transform, false);
            // Set its position and fix rotation and scale
            go.transform.localPosition = manager.firstTestPosition + 
                new Vector3(0, i * manager.verticalOffset, 0);
            // Initialise it
            manager.testCases[i].Initialise(go, i + 1, manager);
        }
    }
}
