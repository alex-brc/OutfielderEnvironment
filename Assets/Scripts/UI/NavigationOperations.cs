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

    public void Back()
    {
        UIManager.Show(setupView);
        UIManager.Hide(controllerView);
        UIManager.Hide(experimentView);
    }
    public void GotoControllerView()
    {
        UIManager.Show(controllerView);
        UIManager.Hide(setupView);
    }
    public void GotoExperimentView()
    {
        UIManager.Show(experimentView);
        UIManager.Hide(setupView);
    }
}
