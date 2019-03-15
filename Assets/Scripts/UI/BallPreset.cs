using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallPreset
{
    // add materials

    public static BallPreset[] presets = {
        null, // Custom
        new BallPreset(0.1f, 0.14f, 0.01f), // Baseball
        new BallPreset(0.1f, 0.05f, 0.02f), // Tennis
        new BallPreset(0.1f, 0.005f, 0.5f), // Shuttlecock
        new BallPreset(0.22f, 0.45f, 0.1f), // Football
        new BallPreset(0.24f, 0.62f, 0.1f)};// Basketball

    internal float size;
    internal float mass;
    internal float drag;
    
    public static void SetPreset(int preset, ref Configurable<float> size, ref Configurable<float> mass, ref Configurable<float> drag)
    {
        // If custom, don't touch them
        if (preset == 0)
            return;

        size.Set(presets[preset].size);
        mass.Set(presets[preset].mass);
        drag.Set(presets[preset].drag);

    }

    private BallPreset(float size, float mass, float drag)
    {
        this.size = size;
        this.mass = mass;
        this.drag = drag;
    }
}
