using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Reference all of the UI elements linked to a variable
    [Header("Configurable references")]
    public Container autoToggle;
    public Container
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

    [Header("Interactions")]
    public InputField smoothingAmountField;
    public Button eyeCalibrateButton;

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

    public void ControllerTypeOnValueChanged(int value)
    {
        if (value == 0) // FOVE
            eyeCalibrateButton.interactable = true;
        else
            eyeCalibrateButton.interactable = false;
    }
}