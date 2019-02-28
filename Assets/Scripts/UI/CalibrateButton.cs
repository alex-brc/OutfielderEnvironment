using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CalibrateButton : MonoBehaviour
{

    public PlayerController playerController;
    public Button startButton, startPracticeButton;

    private Button calibrateButton;
    private ColorBlock notCalibratingColors, calibratingColors;
    
    void Start()
    {
        calibrateButton = transform.gameObject.GetComponent<Button>();

        // Disable start buttons while not calibrated
        startButton.interactable = false;
        startPracticeButton.interactable = false;

        notCalibratingColors = calibrateButton.colors;
        calibratingColors = calibrateButton.colors;
        calibratingColors.normalColor = CustomColors.Red;
        calibratingColors.highlightedColor = CustomColors.Red * 0.85f;
        calibratingColors.pressedColor = CustomColors.Red * 0.6f;
    }

    public void Calibrate()
    {
        if (!playerController.calibrating)
        {
            playerController.calibrating = true;
            startButton.interactable = false;
            startPracticeButton.interactable = false;

            playerController.ClearCalibration();
            playerController.SetZeroPosition();

            calibrateButton.colors = calibratingColors;
        }
        else
        {
            playerController.calibrating = false;
            startButton.interactable = true;
            startPracticeButton.interactable = true;

            calibrateButton.colors = notCalibratingColors;
        }
    }
}
