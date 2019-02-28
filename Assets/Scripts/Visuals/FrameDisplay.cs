using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FrameDisplay : MonoBehaviour
{
    public TrialsManager manager;

    private Text text;
    private void Start()
    {
        text = gameObject.GetComponent<Text>();
    }

    private void Update()
    {
        if (manager.trialStatus == TrialsManager.TrialStatus.TrialInProgress)
            text.text = "F: " + (Time.frameCount - manager.startingFrame);
    }
}
