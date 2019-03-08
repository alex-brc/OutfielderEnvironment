using System.Collections;
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
    public TrialsManager manager;
    public GameObject testPrefab;

    internal Image[] testBackgrounds;

    private Rect thisRect;
    private Rect prefabRect;
    private float contentHeight;

    private void Start()
    {
        thisRect = GetComponent<RectTransform>().rect;
        prefabRect = testPrefab.GetComponent<RectTransform>().rect;
    }

    private void OnEnable()
    {
        // Set content panel height
        contentHeight = manager.testCases.Length * prefabRect.height;
        thisRect.Set(thisRect.x, thisRect.y, thisRect.width, contentHeight);

        testBackgrounds = new Image[manager.testCases.Length];
        // Add test prefabs
        for(int i = 0; i < manager.testCases.Length; i++)
        {
            // Instantiate
            GameObject newTest = Instantiate(testPrefab, transform);
            Rect newRect = newTest.GetComponent<RectTransform>().rect;
            // Add image to array
            testBackgrounds[i] = newTest.GetComponent<Image>();
            // Set the position within the content panel
            float newHeight =
                contentHeight / 2 + newRect.height / 2 // First position
                + i * newRect.height;                  // Offset by number of tests
            newRect.Set(newRect.x, newHeight, newRect.x, newRect.height);
            // Initialise it with values
            manager.testCases[i].Initialise(newTest);
            // Done
        }
    }
}
