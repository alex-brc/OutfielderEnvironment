﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestBuilder : MonoBehaviour
{
    public float fillRatio = 0.8f;

    public Configurable<int> targetsShape = new Configurable<int>();
    public Configurable<int> numberOfTests = new Configurable<int>();
    public Configurable<float> maxBallHeight = new Configurable<float>();
    public Configurable<float> radius = new Configurable<float>();

    public Text xRange;
    public Text zRange;
    public Text cPos;
    public Text bPos;
    public Transform cTransform;
    public Transform bTransform;
    public RectTransform targetDisplay;
    public GameObject targetPrefab;
    public ConfigurationManager confManager;
    public DataManager dataManager;
    public SubjectOperations subjectOp;
    public TrialManager manager;
    public Button experimentViewButton;
    public Button controllerViewButton;

    private List<GameObject> marks; 
    private Vector3[] relativeTargets;
    
    public void ResetTests()
    {
        confManager.tests = new List<TestCase>();
        manager.TestCases = new TestCase[0];
        ClearUI();
        experimentViewButton.interactable = false;
        controllerViewButton.interactable = false;
    }

    public void BuildTests()
    {
        confManager.Refresh();

        if (confManager.auto.Get())
        {
            if (!MakeTargets())
                return;
            confManager.tests = GetTests();
        }

        manager.TestCases = confManager.tests.ToArray();
        UpdateUI();
        dataManager.WriteTestsFile(manager.TestCases);
        if (subjectOp.hasSubject)
        {
            experimentViewButton.interactable = true;
            controllerViewButton.interactable = true;
        }
    }

    public bool MakeTargets()
    {
        if (numberOfTests == 0)
        {
            confManager.statusText.text = "Number of tests cannot be 0.";
            confManager.statusText.color = CustomColors.Black;
            return false;
        }
        if (targetsShape == 0 && numberOfTests.Get() % 4 != 0)
        {
            confManager.statusText.text = "Number of tests has to be a multiple of 4 for a square shape.";
            confManager.statusText.color = CustomColors.Black;
            return false;
        }

        relativeTargets = new Vector3[numberOfTests.Get()];
        if (targetsShape == 0) // Square
        {
            Vector3[] corners =
            {
                new Vector3( - radius.Get(), 0, - radius.Get()),
                new Vector3( - radius.Get(), 0, + radius.Get()),
                new Vector3( + radius.Get(), 0, + radius.Get()),
                new Vector3( + radius.Get(), 0, - radius.Get())
            };
            
            int k = numberOfTests.Get() / 4;
            for (int i = 0; i < k; i++)
            {
                float l = 1.0f * i / k;
                for (int j = 0; j < 4; j++)
                {
                    Vector3 v = Vector3.Lerp(corners[j], corners[(j + 1) % 4], l);
                    relativeTargets[i * 4 + j] = v;
                }
            }
        }
        else // Circle
        {
            for (int i = 0; i < numberOfTests.Get(); ++i)
            {
                float theta = (2 * Mathf.PI / numberOfTests.Get()) * i;
                relativeTargets[i].x = Mathf.Cos(theta) * radius.Get();
                relativeTargets[i].z = Mathf.Sin(theta) * radius.Get();
            }
        }

        confManager.statusText.text = "Tests built succesfully.";
        confManager.statusText.color = CustomColors.Black;
        return true;
    }

    public void UpdateUI()
    {
        // NOTE: x = -x, z = -y

        // Clear previous 
        ClearUI();

        // Scaling factor to fit all tests in the box
        float scalingFactor = 0;

        // If manual, we must make the targets
        if (!confManager.auto.Get())
        {
            List<Vector3> relativeTargets = new List<Vector3>();

            // Update UI using tests from manager
            Vector2 min = Vector2.positiveInfinity;
            Vector2 max = Vector2.negativeInfinity;
            foreach (TestCase test in manager.TestCases)
            {
                // Find all targets relative to catcher pos
                Vector3 relativeTarget = test.target - manager.playerStartPosition; 
                relativeTargets.Add(relativeTarget);
                // Find the range of the targets
                Vector3 pos = new Vector3(-relativeTarget.x, -relativeTarget.z);
                if (pos.x < min.x)
                    min.x = pos.x;
                if (pos.x > max.x)
                    max.x = pos.x;
                if (pos.y < min.y)
                    min.y = pos.y;
                if (pos.y > max.y)
                    max.y = pos.y;
            }
            // Find scaling factor
            float uiY = targetDisplay.rect.height;
            // The range we have around the catcher for the tests to fit in is
            float yTestRange = targetDisplay.rect.height / 2;
            float xTestRange = (targetDisplay.rect.width + cTransform.localPosition.x); // since this position is going to be negative
            // Scale for the maximum of the range values
            float biggest = Mathf.Max(Mathf.Abs(max.x), Mathf.Abs(min.x),
                           Mathf.Abs(max.y), Mathf.Abs(min.y));
            if (biggest == Mathf.Abs(max.x) || biggest == Mathf.Abs(min.x)) // then it's wider than tall
                scalingFactor = fillRatio * (xTestRange / biggest);
            else // it's taller than wide
                scalingFactor = fillRatio * (yTestRange / biggest);
            
            this.relativeTargets = relativeTargets.ToArray();
        }
        else
        {
            // Tests should've been built previously
            scalingFactor = fillRatio * (targetDisplay.rect.height / 2) / radius.Get();
        }

        foreach (Vector3 target in relativeTargets)
        {
            Vector3 pos = new Vector3(-target.x, -target.z, 0);
            GameObject go = Instantiate(targetPrefab, targetDisplay);
            go.transform.localPosition = cTransform.localPosition + pos * scalingFactor;
            marks.Add(go);
        }

        // Position markers
        cPos.text = "(" + manager.playerStartPosition.x.ToString("0.##") + ",0)";
        bPos.text = "(0,0)";
        // Range markers
        float temp =  manager.playerStartPosition.x + targetDisplay.rect.width / scalingFactor / 2;
        xRange.text = "range(x) = (" + temp.ToString("0.##") + ",0)";
        temp = targetDisplay.rect.height / scalingFactor / 2;
        zRange.text = "range(z) = (" + (-temp).ToString("0.##") + "," + temp.ToString("0.##") + ")";
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
        foreach(Vector3 target in relativeTargets)
            tests.Add(new TestCase(target + manager.playerStartPosition, maxBallHeight.Get(), tests.Count));
        return tests;
    }
}
