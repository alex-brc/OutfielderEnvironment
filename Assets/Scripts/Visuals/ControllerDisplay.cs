using UnityEngine;
using UnityEngine.UI;

public class ControllerDisplay : MonoBehaviour
{
    public Image cursorBackground;
    public PlayerController player;

    void Update()
    {
        if (!player.controller.calibrated)
            return;

        // Update the controller viewport with the lean vector
        transform.localPosition = new Vector3() {
            y = - player.controller.latestInput.x * cursorBackground.rectTransform.rect.height / 2,
            x = player.controller.latestInput.z * cursorBackground.rectTransform.rect.width / 2,
            z = 0
        };
    }
}
