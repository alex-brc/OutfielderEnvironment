using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CalibrateButton : MonoBehaviour
{

    public PlayerController player;
    public Button startButton;
    public Text status;

    private Button calibrateButton;
    private ColorBlock notCalibratingColors, calibratingColors;
    
    void Start()
    {
        calibrateButton = transform.gameObject.GetComponent<Button>();
        calibrateButton.interactable = true;

        // Disable start buttons while not calibrated
        startButton.interactable = false;

        notCalibratingColors = calibrateButton.colors;
        calibratingColors = calibrateButton.colors;
        calibratingColors.normalColor = CustomColors.Red;
        calibratingColors.highlightedColor = CustomColors.Red * 0.85f;
        calibratingColors.pressedColor = CustomColors.Red * 0.6f;
    }

    public void Calibrate()
    {
        if (!player.controller.calibrating)
        {
            player.controller.calibrating = true;

            player.controller.ClearCalibration();
            player.controller.SetZeroPosition();

            status.text = "Calibrating";

            calibrateButton.colors = calibratingColors;
        }
        else
        {
            player.controller.calibrating = false;
            startButton.interactable = true;

            status.text = "Calibrated";
            player.controller.calibrated = true;

            calibrateButton.colors = notCalibratingColors;
        }
    }
}
