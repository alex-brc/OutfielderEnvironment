using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

class FoveInitialiser : MonoBehaviour
{
    public Button startExperimentButton;
    public Text status;
    public Text controlStatus;

    public void Start()
    {
        StartCoroutine(Init());
    }

    public void StartCalibrating()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        // Block start button until we have a calibrated fove
        startExperimentButton.interactable = true;
        status.text = "Calibrating FOVE...";

        // If fove is not connected, quit
        bool ok = true;
        try
        {
            if (!FoveInterface.IsHardwareConnected())
            {
                ok = false;
            }
        }
        catch (NullReferenceException)
        {
            ok = false;
        }

        if (!ok)
        {
            // There is no usable fove device
            status.text = "No FOVE found";
            status.color = CustomColors.Red;
        }
        else
        {
            // Wait till it's calibrated
            while (!FoveInterface.IsEyeTrackingCalibrated())
            {
                FoveInterface.EnsureEyeTrackingCalibration();

                while (FoveInterface.IsEyeTrackingCalibrating())
                {
                    yield return new WaitForSeconds(0.5f);
                }
            }

            // Fove is OK
            status.text = "Ready to start";
            startExperimentButton.interactable = true;
        }
    }
}

