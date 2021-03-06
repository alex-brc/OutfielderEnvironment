﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Loads the test prefabs into the scrollview content and
/// initialises them with their respective values.
/// Should be attached to the content panel.
/// </summary>
public class ContentLoader : MonoBehaviour
{
    [Header("References")]
    public TrialManager manager;
    public TrialRunner runner;
    public GameObject testPrefab;

    internal Image[] testBackgrounds;

    private List<GameObject> testObjects = new List<GameObject>();
    private RectTransform content;
    private Rect prefabRect;
    private float contentHeight;
    
    public void LoadTests()
    {
        if (!manager.testsChanged)
            return;

        Clear();

        content = GetComponent<RectTransform>();
        prefabRect = testPrefab.GetComponent<RectTransform>().rect;

        // Set content panel height
        contentHeight = manager.TestCases.Length * prefabRect.height;
        content.SetHeight(contentHeight);
        content.SetRight(0);

        testBackgrounds = new Image[manager.TestCases.Length];
        // Add test prefabs
        for(int i = 0; i < manager.TestCases.Length; i++)
        {
            // Instantiate
            GameObject newTest = Instantiate(testPrefab, transform);
            Rect newRect = newTest.GetComponent<RectTransform>().rect;
            // Add image to array
            testBackgrounds[i] = newTest.transform.Find("TextCols").GetComponent<Image>();
            // Set the position within the content panel
            float newHeight = -newRect.height / 2 - i * newRect.height;

            newTest.transform.localPosition = new Vector3(360, newHeight, 0);
            // Initialise it with values
            manager.TestCases[i].Initialise(newTest);
            // Add the object for the builder
            testObjects.Add(newTest);
            // Done
        }

        // Give these to the runner
        runner.testBackgrounds 
            = testBackgrounds;

        manager.testsChanged = false;
    }

    public void Clear()
    {
        foreach(GameObject obj in testObjects)
        {
            Destroy(obj);
        }

        testObjects = new List<GameObject>();
    }
}
