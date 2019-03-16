using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Reference all of the UI elements linked to a variable
    public Container
        autoToggle,
        startingDistanceBox,
        maxSpeedBox,
        numberOfTestsBox,
        trialRunsBox,
        practiceRunsBox,
        maxBallHeightBox,
        radiusBox,
        targetsShapeDropdown,
        ballPresetDropdown,
        ballMassBox,
        ballSizeBox,
        ballDragBox,
        pauseBetweenBox,
        controllerTypeDropdown,
        inputCurveDropdown,
        inputSmoothingToggle,
        smoothingAmountBox,
        curveParameterBox;

    public InputField smoothingAmountField;
    public InputField ballPresetField;
    public InputField ballSizeField;
    public InputField ballMassField;
    public InputField ballDragField;

    public static void Show(CanvasGroup page)
    {
        page.alpha = 1;
        page.blocksRaycasts = true;
        page.interactable = true;
    }

    public static void Hide(CanvasGroup page)
    {
        page.alpha = 0;
        page.blocksRaycasts = false;
        page.interactable = false;
    }

    public void SmothingOnValueChanged(bool on)
    {
        if (on)
            smoothingAmountField.interactable = true;
        else
            smoothingAmountField.interactable = false;
    }

    public void BallPresetOnValueChanged(int value)
    {
        if(value == 0)
        {
            ballMassField.interactable = true;
            ballSizeField.interactable = true;
            ballDragField.interactable = true;
        }
        else
        {
            ballMassField.interactable = false;
            ballSizeField.interactable = false;
            ballDragField.interactable = false;
        }
    }
}