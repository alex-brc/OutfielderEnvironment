using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class BallPresetDropdown : MonoBehaviour
{
    public Selectable[] ballFields;

    [Header("Dev's note: make this of size 6")]
    public Material[] ballMaterials;

    public InputField sizeField;
    public InputField massField;
    public InputField frictionField;

    public MeshRenderer ballMesh;

    private Dropdown thisDropdown;
    private BallPreset[] presets;
    
    private void Awake()
    {
        thisDropdown = gameObject.GetComponent<Dropdown>();
        presets = new BallPreset[6];
        presets[1] = BallPreset.Baseball;
        presets[2] = BallPreset.Tennisball;
        presets[3] = BallPreset.Shuttlecock;
        presets[4] = BallPreset.Football;
        presets[5] = BallPreset.Basketball;
    }

    public void SetValue(int value)
    {
        thisDropdown.value = value;
        OnValueChanged();
    }

    public int GetValue()
    {
        return thisDropdown.value;
    }

    public void FillBoxes(float size, float mass, float friction, string format)
    {
        sizeField.text = size.ToString(format);
        massField.text = mass.ToString(format);
        frictionField.text = friction.ToString(format);
    }

    public void OnValueChanged()
    {
        if (thisDropdown.value == 0)
        { // Custom
            foreach (Selectable element in ballFields)
                element.interactable = true;
            ballMesh.material = ballMaterials[thisDropdown.value];
            return;
        }
        foreach (Selectable element in ballFields)
            element.interactable = false;

        // Rewrite box values
        FillBoxes(presets[thisDropdown.value].size, 
            presets[thisDropdown.value].mass, 
            presets[thisDropdown.value].friction,
            "0.##");
        ballMesh.material = ballMaterials[thisDropdown.value];
    }
}

public class BallPreset
{
    public static BallPreset
        Baseball = new BallPreset(0.1f, 0.14f, 0.01f),
        Tennisball = new BallPreset(0.1f, 0.05f, 0.02f),
        Shuttlecock = new BallPreset(0.1f, 0.005f, 0.5f),
        Football = new BallPreset(0.22f, 0.45f, 0.1f),
        Basketball = new BallPreset(0.24f, 0.62f, 0.1f);

    internal float size;
    internal float mass;
    internal float friction;

    private BallPreset(float size, float mass, float friction)
    {
        this.size = size;
        this.mass = mass;
        this.friction = friction;
    }
}
