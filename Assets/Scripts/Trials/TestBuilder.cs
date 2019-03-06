using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestBuilder : MonoBehaviour
{
    public TrialsManager manager;
    public GameObject testGroupPrefab;
    public ManualsToggler toggler;

    // Make tests
    void Intialise()
    {
        Button[] testCaseButtons = new Button[2*manager.testCases.Length];
        for (int i = 0; i < manager.testCases.Length; i++)
        {
            GameObject go = Instantiate(testGroupPrefab);
            // Grab references to the buttons
            testCaseButtons[2 * i] = go.transform.Find("LoadButton").GetComponent<Button>();
            testCaseButtons[2 * i + 1] = go.transform.Find("UnloadButton").GetComponent<Button>();
            // Set its parent to the tests group
            go.transform.SetParent(manager.testsGroup.transform, false);
            // Set its position and fix rotation and scale
            go.transform.localPosition = manager.firstTestPosition +
                new Vector3(0, i * manager.verticalOffset, 0);
            // Initialise it
            manager.testCases[i].Initialise(go, i + 1, manager);
        }

        // Give the buttons to the toggler
        toggler.testCaseButtons = testCaseButtons;
    }
}
