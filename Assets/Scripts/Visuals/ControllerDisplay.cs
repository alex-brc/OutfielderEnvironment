using UnityEngine;

public class ControllerDisplay : MonoBehaviour
{
    public PlayerController player;

    void Update()
    {
        // Update the controller viewport with the lean vector
        transform.localPosition = new Vector3() {
            x = - player.velocityVector.x * 50,
            y = - player.velocityVector.z * 50,
            z = 0
        };
    }
}
