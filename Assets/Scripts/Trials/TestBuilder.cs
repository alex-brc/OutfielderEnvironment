using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBuilder : MonoBehaviour
{
    public TrialsManager manager;
    public GameObject testGroupPrefab;

    // Make tests
    void Start()
    {
        manager.testCases = new TestCase[8];
        Vector3 catcherStartPosition = manager.catcherStartPosition;
        float distanceToTargets = manager.distanceToTargets;
        // 8 positions around the catcher
        Vector3[] positions =
        {
            new Vector3(catcherStartPosition.x - distanceToTargets, 0, catcherStartPosition.z),
            new Vector3(catcherStartPosition.x - distanceToTargets, 0, catcherStartPosition.z - distanceToTargets),              //   o    o    o  
            new Vector3(catcherStartPosition.x, 0, catcherStartPosition.z - distanceToTargets),                                  //        
            new Vector3(catcherStartPosition.x + distanceToTargets, 0, catcherStartPosition.z - distanceToTargets),              //   o    X    o  ---> X is player, o are targets
            new Vector3(catcherStartPosition.x + distanceToTargets, 0, catcherStartPosition.z),                                  //  
            new Vector3(catcherStartPosition.x + distanceToTargets, 0, catcherStartPosition.z + distanceToTargets),              //   o    o    o
            new Vector3(catcherStartPosition.x, 0, catcherStartPosition.z + distanceToTargets),
            new Vector3(catcherStartPosition.x - distanceToTargets, 0, catcherStartPosition.z + distanceToTargets)
        };

        for (int i = 0; i < manager.testCases.Length; i++)
        {
            // Make one
            manager.testCases[i] = new TestCase(positions[i], manager.maximumBallHeight, 2);
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
