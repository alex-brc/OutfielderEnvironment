using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public float speed = 2;
    public float maxSpeed = 6;
    public float rotationSpeed = 30;


    private Rigidbody rb;

    void Start() {
    	rb = GetComponent<Rigidbody>();
    }

    // Physics goes here
    void FixedUpdate() {
    	// These are in -1..1
    	float moveHor = Input.GetAxis("Horizontal"); 
    	float moveVer = Input.GetAxis("Vertical");
        Debug.Log("Hor:" + moveHor + " ver:" + moveVer);

        // Move in global directions
        Vector3 movement = new Vector3(
    		moveHor,
    		0,
            moveVer);

        // Apply force
        rb.AddForce(movement * speed);
        // Limit top speed
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);


    }
 }