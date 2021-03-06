﻿using Fove.Managed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class contains frequently used extension methods for the unity Vector3 class
/// </summary>
public static class Vector3Extensions
{

    public static bool TryParse(this Vector3 vector, string input, out Vector3 output)
    {
        string[] tokens = input.Split(',');
        output = new Vector3();
        // Check format
        if (tokens.Length != 3)
            return false;

        float[] vals = new float[3];
        for (int i = 0; i < tokens.Length; i++)
        {
            if (!float.TryParse(tokens[i], out vals[i]))
                return false;
        }

        // Make the vector
        output = new Vector3(vals[0], vals[1], vals[2]);

        return true;
    }

    /// <summary>
    /// Returns a string suitable for inserting in a CSV line.
    /// This outputs just the x,y,z coordinates separated by commas.
    /// </summary>
    public static string ToCSVFormat(this Vector3 vector)
    {
        return "" + vector.x + "," + vector.y + "," + vector.z;
    }

    public static string ToCSVFormat(this Vector2 vector)
    {
        return "" + vector.x + "," + vector.y;
    }

    public static string ToCSVFormat(this SFVR_Vec3 vector)
    {
        return "" + vector.x + "," + vector.y + "," + vector.z;
    }

    /// <summary>
    /// Returns the projection of this vector into the XZ plane
    /// </summary>
    public static Vector3 XZ(this Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }

    /// <summary>
    /// Returns the projection of this vector into the XY plane
    /// </summary>
    public static Vector3 XY(this Vector3 vector)
    {
        return new Vector3(vector.x, vector.y, 0);
    }

    /// <summary>
    /// Returns the projection of this vector into the YZ plane
    /// </summary>
    public static Vector3 YZ(this Vector3 vector)
    {
        return new Vector3(0, vector.y, vector.z);
    }
}
