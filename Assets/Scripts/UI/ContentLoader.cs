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

    private Rect contentRect;
    private Rect prefabRect;
    private float contentHeight;

    private void Start()
    {
        contentRect = GetComponent<RectTransform>().rect;
        prefabRect = testPrefab.GetComponent<RectTransform>().rect;
    }

    private void OnEnable()
    {
        // Set content panel height
        contentHeight = manager.testCases.Length * prefabRect.height;
        contentRect.Set(contentRect.x, contentRect.y, contentRect.width, contentHeight);

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
            float newHeight = -newRect.height/2 - i * newRect.height;                  
            newRect.Set(newRect.x, newHeight, newRect.x, newRect.height);
            newTest.transform.localPosition = new Vector3(360, newHeight, 0);
            // Initialise it with values
            manager.testCases[i].Initialise(newTest);
            // Done
        }
    }
}
