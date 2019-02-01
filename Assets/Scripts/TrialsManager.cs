using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialsManager : MonoBehaviour
{
    public int secondsCountdownBeforeStart = 2;
    public GameObject baseball;
    public UnityEngine.UI.Text infoBox;
    public GameObject UI;
    public GameObject testsGroup;
    public GameObject dataWriterObject;

    internal TestCase testCase;

    private GameObject initial;
    private DataWriter dataWriter;

    void Start()
    {
        initial = this.gameObject;
        dataWriter = dataWriterObject.GetComponent<DataWriter>();
    }

    public static TrialsManager GetTrialsManager()
    {
        return GameObject
            .FindGameObjectWithTag("ManagerHolder")
            .GetComponent<TrialsManager>();
    }

    public void StartButton()
    {
        // If nothing's loaded, stop
        if (testCase != null)
        {
            StartCoroutine(StartTest());
        }
        else
        {
            infoBox.text = "No test loaded";
        }
    }

    public void Reset()
    {
        // Go through all the test cases and call reset on them
        foreach(Transform child in testsGroup.transform)
        {
            if (child.gameObject.name.Contains("TestGroup"))
            {
                // It's a test case. Reset it
                child.GetComponent<TestCase>().ResetTest();
            }
        }
    }

    internal IEnumerator StartTest()
    {        
        // Freeze ball 
        baseball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        // Start the world
        Time.timeScale = 1;
        // Give the player x seconds to adjust to the motion
        float currCountdownValue = secondsCountdownBeforeStart;
        // Update counter color
        infoBox.color = CustomColors.Orange;
        while (currCountdownValue > 0)
        {
            infoBox.text = currCountdownValue + "...";
            yield return new WaitForSeconds(1.0f);
            currCountdownValue--;
        }
        // Update counter
        infoBox.text = "Trial running";
        // Update status
        testCase.testStatus.text = "In Progress";
        testCase.testStatus.color = CustomColors.Red;
        // Start the data writer
        dataWriter.StartNewTest(testCase.testNumber);
        // Hide the UI
        UI.SetActive(false);
        // Launch the ball
        baseball.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        baseball.transform.eulerAngles = new Vector3(-1 * testCase.launchAngle, 180 + testCase.launchDeviation, 0);
        baseball.GetComponent<Rigidbody>().velocity = baseball.transform.forward * testCase.launchSpeed;
    }

    internal void CompleteTrial(bool caught)
    {
        testCase.CompleteTest(caught);
        // Freeze time
        Time.timeScale = 0;
        // Update infobox
        infoBox.text = "Trial completed";
        infoBox.color = CustomColors.Black;
        // Show UI
        UI.SetActive(true);
        // Cleanup
        Unload();
        dataWriter.CompleteTest(caught);
    }

    internal void Load(TestCase testCase)
    {
        if(testCase != null)
        {
            // Then some other test was previously loaded
            testCase.UnloadTest();
            Unload();
        }
        this.testCase = testCase;
    }

    internal void Unload()
    {
        testCase = null;
    }
}
