using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, ICatcher
{
    
    public float speed = 2;
    public float maxSpeed = 6;
    public float rotationSpeed = 30;

    public Vector3 homePosition;
    public TrialsManager manager;

    private Rigidbody rigidbody;

    void Start() {
    	rigidbody = GetComponent<Rigidbody>();
    }
    
    public void StartButton()
    {
        manager.catcher = this;
        StartCoroutine(manager.StartTrial(TestCase.Type.Trial));
    }

    public void StartPracticeButton()
    {
        manager.catcher = this;
        StartCoroutine(manager.StartTrial(TestCase.Type.Practice));
    }

    public void Move()
    {
        // These are in -1..1
        float moveHor = Input.GetAxis("Horizontal");
        float moveVer = Input.GetAxis("Vertical");

        // Move in global directions
        Vector3 movement = new Vector3(
            moveHor,
            0,
            moveVer);

        // Apply force
        rigidbody.AddForce(movement * speed);
        // Limit top speed
        rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, maxSpeed);
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