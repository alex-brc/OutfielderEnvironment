using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Reference all of the UI elements linked to a variable
    public Container
        targetsShapeDropdown,
        ballPresetDropdown,
        autoToggle,
        startingDistanceBox,
        maxSpeedBox,
        numberOfTestsBox,
        practiceRunsBox,
        trialRunsBox,
        maxBallHeightBox,
        radiusBox,
        ballMassBox,
        ballSizeBox,
        ballDragBox,
        pauseBetweenBox;
    
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
}