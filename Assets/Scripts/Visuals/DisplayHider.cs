using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Disables the target display (if it has a mask). 
/// For use in the overlay controls.
/// </summary>
public class DisplayHider : MonoBehaviour
{
    public Mask target;
    
    public void OnValueChanged(bool val)
    {
        target.enabled = !val;
        RecursiveLineRendererEnabled(target.transform, val);
    }

    private void RecursiveLineRendererEnabled(Transform trans, bool val)
    {
        foreach (Transform child in trans)
        {
            LineRenderer lr = child.GetComponent<LineRenderer>();
            if (lr != null)
                lr.enabled = val;
            RecursiveLineRendererEnabled(child, val);
        }
    }
}
