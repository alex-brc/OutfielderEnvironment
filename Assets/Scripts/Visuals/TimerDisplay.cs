using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TimerDisplay : MonoBehaviour
{
    public TrialManager manager;

    private Text text;
    private void Start()
    {
        text = gameObject.GetComponent<Text>();
    }

    private void Update()
    {
        if (manager.trialStatus == TrialManager.TrialStatus.TrialInProgress)
            text.text = "T: " + (Time.time - manager.startingTime).ToString("0.##");
    }
}
