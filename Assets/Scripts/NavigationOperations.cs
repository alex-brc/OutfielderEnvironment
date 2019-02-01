using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attaches to UI
public class NavigationOperations : MonoBehaviour
{
    public GameObject setupMenu;
    public GameObject controlView;

    public void GotoSetupView()
    {
        setupMenu.SetActive(true);
        controlView.SetActive(false);
    }
    public void GotoControlView()
    {
        setupMenu.SetActive(false);
        controlView.SetActive(true);
    }
}
