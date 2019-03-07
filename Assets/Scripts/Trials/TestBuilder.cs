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
    
    private Vector3[] zeroCenteredTargets;
    
    public void MakeTargets()
    {
        int targetsShape = confManager.targetsShape;
        int numberOfTests = confManager.numberOfTests;
        float radius = confManager.radius;
        
        if (targetsShape == 0 && numberOfTests % 4 != 0)
        {
            numberOfTests -= numberOfTests % 4;
            confManager.statusText.text = "Number of tests has to be a multiple of 4 for a square shape. Truncated to " + numberOfTests;
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
            Debug.Log("K is " + k);
            for (int i = 0; i < k; i++)
            {
                float l = 1.0f * i / k;
                for (int j = 0; j < 4; j++)
                {
                    Debug.Log("l is " + l);
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
    }

    public void UpdateUI()
    {
        foreach(Vector3 target in zeroCenteredTargets)
        {
            Vector3 pos = new Vector3(target.x, target.z, 0);
            float t = fillRatio * 100 / confManager.radius;
            GameObject go = Instantiate(targetPrefab, targetDisplay);
            go.transform.localPosition = pos * t;
        }
    }

    public List<TestCase> GetTests()
    {
        List<TestCase> tests = new List<TestCase>();
        foreach(Vector3 target in zeroCenteredTargets)
            tests.Add(new TestCase(target + manager.catcherStartPosition, confManager.maxBallHeight));
        return tests;
    }
}
