using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attaches to UI
public class NavigationOperations : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup setupView;
    public CanvasGroup controllerView;
    public CanvasGroup experimentView;
    public LineRenderer[] renderers;
    public ContentLoader loader;

    public void Back()
    {
        UIManager.Show(setupView);
        // Maybe hide controller linerederers
        foreach (LineRenderer lr in renderers)
            lr.enabled = false;
        UIManager.Hide(controllerView);
        UIManager.Hide(experimentView);
    }
    public void GotoControllerView()
    {
        // Maybe show controller linerederers
        foreach (LineRenderer lr in renderers)
            lr.enabled = true;
        UIManager.Show(controllerView);
        UIManager.Hide(setupView);
    }
    public void GotoExperimentView()
    {
        // Load content panel with tests
        loader.LoadTests();

        UIManager.Show(experimentView);
        UIManager.Hide(setupView);
    }
}
