using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    [Header("References")]
    public TrialsManager manager;
    public InputField sizeField;
    public InputField massField;
    public InputField dragField;


    internal Configurable<int> preset = new Configurable<int>();
    internal Configurable<float> size = new Configurable<float>();
    internal Configurable<float> mass = new Configurable<float>();
    internal Configurable<float> drag = new Configurable<float>();
    private Rigidbody rb;

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Refresh()
    {
        if (preset != 0)
            BallPreset.SetPreset(preset.Get(), ref size, ref mass, ref drag);
           
        rb.mass = mass.Get();
        rb.drag = drag.Get();
        transform.localScale = new Vector3(size.Get(), size.Get(), size.Get());

        // Update UI boxes
        sizeField.text = size.ToString();
        massField.text = mass.ToString();
        dragField.text = drag.ToString();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if we hit the player or not
        bool result = false;
        if (collision.collider.gameObject.tag == "Catcher")
            result = true;
        // Finish the trial and clean the physics sampler
        manager.CompleteTrial(result);
    }   
}
