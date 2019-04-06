using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

class FoveInitialiser : MonoBehaviour
{
    public Button startExperimentButton;
    public Button calibrateButton;
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
        startExperimentButton.interactable = false;
        status.text = "Controller not calibrated";
        controlStatus.text = "Connecting FOVE...";

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
            controlStatus.text = "No FOVE found";
            controlStatus.color = CustomColors.Red;
        }
        else
        {
            // Found fove, not calibrated
            controlStatus.text = "FOVE not calibrated";
            controlStatus.color = CustomColors.Black;

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
            controlStatus.text = "Calibrated";
            controlStatus.color = CustomColors.SoftGreen;
            startExperimentButton.interactable = true;
        }
    }
}

