using Fove.Managed;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class contains frequently used extension methods for the unity Vector3 class
/// </summary>
public static class Vector3Extensions
{
    public static string ToRecordFormat(this Vector3 vector)
    {
        return "" + vector.x + DataTags.SecondarySeparator + vector.y + DataTags.SecondarySeparator + vector.z;
    }

    public static string ToRecordFormat(this SFVR_Vec3 vector)
    {
        return "" + vector.x + DataTags.SecondarySeparator + vector.y + DataTags.SecondarySeparator + vector.z;
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
