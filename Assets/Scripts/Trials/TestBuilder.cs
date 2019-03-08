using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBuilder : MonoBehaviour
{
    public float fillRatio = 0.8f;
    public ConfigurationManager confManager;
    public TrialsManager manager;
    public Transform targetDisplay;
    public GameObject targetPrefab;

    private List<GameObject> marks; 
    private Vector3[] zeroCenteredTargets;
    
    public void MakeTargets()
    {
        bool ok = true;
        int targetsShape = confManager.targetsShape;
        int numberOfTests = confManager.numberOfTests;
        float radius = confManager.radius;
        
        if (targetsShape == 0 && numberOfTests % 4 != 0)
        {
            numberOfTests -= numberOfTests % 4;
            confManager.statusText.text = "Number of tests has to be a multiple of 4 for a square shape. Truncated to " + numberOfTests;
            confManager.statusText.color = CustomColors.Black;
            ok = false;
        }

        zeroCenteredTargets = new Vector3[numberOfTests];
        if (targetsShape == 0) // Square
        {
            Vector3[] corners =
            {
                new Vector3( - radius, 0, - radius),
                new Vector3( - radius, 0, + radius),
                new Vector3( + radius, 0, + radius),
                new Vector3( + radius, 0, - radius)
            };
            
            int k = numberOfTests / 4;
            for (int i = 0; i < k; i++)
            {
                float l = 1.0f * i / k;
                for (int j = 0; j < 4; j++)
                {
                    Vector3 v = Vector3.Lerp(corners[j], corners[(j + 1) % 4], l);
                    zeroCenteredTargets[i * 4 + j] = v;
                }
            }
        }
        else // Circle
        {
            for (int i = 0; i < numberOfTests; ++i)
            {
                float theta = (2 * Mathf.PI / numberOfTests) * i;
                zeroCenteredTargets[i].x = Mathf.Cos(theta) * radius;
                zeroCenteredTargets[i].z = Mathf.Sin(theta) * radius;
            }
        }
        if(ok)
        {
            confManager.statusText.text = "Tests built succesfully.";
            confManager.statusText.color = CustomColors.Black;
        }
    }

    public void UpdateUI()
    {
        // Clear previous 
        ClearUI();

        foreach(Vector3 target in zeroCenteredTargets)
        {
            Vector3 pos = new Vector3(target.x, target.z, 0);
            float t = fillRatio * 100 / confManager.radius;
            GameObject go = Instantiate(targetPrefab, targetDisplay);
            go.transform.localPosition = pos * t;
            marks.Add(go);
        }
    }

    public void ClearUI()
    {
        if (marks == null)
        {
            marks = new List<GameObject>();
            return;
        }
        foreach (GameObject mark in marks)
            Destroy(mark);
        marks.Clear();
    }

    public List<TestCase> GetTests()
    {
        List<TestCase> tests = new List<TestCase>();
        foreach(Vector3 target in zeroCenteredTargets)
            tests.Add(new TestCase(target + manager.catcherStartPosition, confManager.maxBallHeight, tests.Count));
        return tests;
    }
}
