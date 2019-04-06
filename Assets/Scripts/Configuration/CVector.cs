using System;
using System.ComponentModel;
using UnityEngine;

[TypeConverter(typeof(CVectorConverter))]
public class CVector : IEquatable<Vector3>, IEquatable<CVector>, IComparable<Vector3>, IComparable<CVector>{

    private Vector3 value;

    public CVector(Vector3 value)
    {
        this.value = value;
    }

    public CVector(string s)
    {
        if (!value.TryParse(s, out value))
            throw new Exception("Tried to parse \"" + s + "\" as a vector.");
    }

    public Vector3 Get()
    {
        return value;
    }

    public void Set(Vector3 value)
    {
        this.value = value;
    }
    
    public static implicit operator string(CVector v)
    {
        return v.value.ToString();
    }

    public static implicit operator CVector(string s)
    {
        Vector3 result = new Vector3();
        if (!result.TryParse(s, out result))
            return null;

        return new CVector(result);
    }

    public static implicit operator Vector3(CVector v)
    {
        return v.Get();
    }

    public static implicit operator CVector(Vector3 v)
    {
        return new CVector(v);
    }

    public int CompareTo(Vector3 other)
    {
        if (value.Equals(other))
            return 0;
        else if (value.x < other.x && value.y < other.y && value.z < other.z)
            return -1;
        else
            return 1;
    }

    public int CompareTo(CVector other)
    {
        if (value.Equals(other.value))
            return 0;
        else if (value.x <= other.value.x && value.y <= other.value.y && value.z <= other.value.z)
            return -1;
        else
            return 1;
    }

    public bool Equals(Vector3 other)
    {
        return value.Equals(other);
    }

    public bool Equals(CVector other)
    {
        return value.Equals(other.value);
    }

    public override string ToString()
    {
        return value.ToString();
    }
}