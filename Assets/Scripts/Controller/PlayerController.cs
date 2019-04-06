using UnityEngine;

[RequireComponent(typeof(RigidbodyCollector))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public Configurable<float> maximumSpeed = new Configurable<float>();
    public Vector3 homePosition;
    public PathDisplay path;

    [Header("References")]
    public TrialManager manager;
    public Controller controller;
    public GameObject fove;
    
    private new Rigidbody rigidbody;

    void Start() {
    	rigidbody = GetComponent<Rigidbody>();
    }

    public void Move()
    {
        rigidbody.velocity = controller.latestInput * maximumSpeed.Get();

        // Add current position to path
        path.UpdateLine(transform.position);
    }

    public void SendHome()
    {
        rigidbody.position = homePosition;
        rigidbody.velocity = Vector3.zero;
        rigidbody.rotation = Quaternion.Euler(0,0,0);
    }

    public Rigidbody GetRigidbody()
    {
        return rigidbody;
    }
}