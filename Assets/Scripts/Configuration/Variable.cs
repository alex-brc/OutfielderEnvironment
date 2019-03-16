using System;
using System.ComponentModel;
using UnityEngine.Events;

public class Variable<T> : IVariable where T : struct, IComparable<T>, IEquatable<T>
{
    /// <summary>
    /// A Default variable has the value set as default from the configuration manager.
    /// A Set variable is a formerly Default variable that has been configured
    /// by the user and had its value set manually (or via a saved auto configuration).
    /// </summary>
    private enum Type { Default, Unchecked, Checked}
    private Type type;

    public readonly string name;
    public Container container;
    /// <summary>
    /// This should be a wrapper of a primitive
    /// (i.e. Integer, Float, etc.)
    /// </summary>
    private Configurable<T> managerVariable;

    private T value, defaultValue;
    private T rangeFrom, rangeTo;
    private ConfigurationManager.RangeType rangeType;

    private TypeConverter converter;

    private UnityAction onValueChanged;
    
    public Variable(string name, ref Configurable<T> managerVariable, T defaultValue, ref Container uiContainer,
        T rangeFrom, T rangeTo, ConfigurationManager.RangeType rangeType, UnityAction onValueChanged = null)
    {
        // Sanity check
        if(!CheckRange(defaultValue,rangeFrom,rangeTo,rangeType))
            throw new Exception("The default value given is outside the range specified. (" + name + ")");
        if(uiContainer == null)
            throw new Exception("Null container reference. (" + name + ")");
        
        // Try to get a type converter
        converter = TypeDescriptor.GetConverter(typeof(T));

        // Check if it can parse from string
        if (converter == null || !converter.CanConvertFrom(typeof(string)))
            throw new Exception("Type " + typeof(T).ToString() + " does not support parsing from a string.");
        
        this.defaultValue = defaultValue;
        this.name = name;
        this.managerVariable = managerVariable;
        type = Type.Default;

        this.rangeFrom = rangeFrom;
        this.rangeTo = rangeTo;
        this.rangeType = rangeType;

        this.container = uiContainer;

        this.onValueChanged = onValueChanged;
    }
    
    public void ForceSet(T value)
    {
        this.value = value;
        type = Type.Unchecked;

        if (onValueChanged != null)
            onValueChanged.Invoke();
    }

    /// <summary>
    /// Attempts to parse the input string into this variable.
    /// </summary>
    /// <returns>true if the value was set, false otherwise</returns>
    public bool TryParse(string input)
    {
        T t;
        try
        {
            // Attempt to convert
            t = (T)converter.ConvertFromString(input);
        }
        catch (NotSupportedException)
        {
            // Failed to convert
            return false;
        }

        // Conversion succeded, set the value
        value = t;

        // Change type
        type = Type.Unchecked;

        return true;
    }

    /// <summary>
    /// Updates the variable in the manager with the value stored,
    /// as well as the UI.
    /// </summary>
    public void Push(bool flashDefaults = false)
    {
        T temp = default(T);
        // If default, push default value
        switch (type)
        {
            case Type.Default:
                temp = defaultValue;
                // Flash red, this isn't recommended
                if(flashDefaults)
                    container.Flash(CustomColors.SoftRed);
                break;
            case Type.Checked:
                temp = value;
                break;
            case Type.Unchecked:
                throw new Exception("Attempted to assign unchecked varaible to manager. (" + name + ")");
        }

        managerVariable.Set(temp);
        container.SetContent(temp.ToString());
        
        if(onValueChanged != null)
            onValueChanged.Invoke();
    }

    /// <summary>
    /// Updates this object with the value from the given
    /// ui container.
    /// </summary>
    public bool Pull()
    {
        return TryParse(container.RetrieveContent());
    }

    /// <summary>
    /// Checks whether the set value is in the given range.
    /// </summary>
    /// <returns>true if in the range, false otherwise</returns>
    public bool Check()
    {
        // If default, already checked
        if (type == Type.Default)
            return true;

        bool c = CheckRange(value, rangeFrom, rangeTo, rangeType);

        // If it's ok, mark this as checked
        if (c)
            type = Type.Checked;

        return c;
    }

    public override string ToString()
    {
        if(type == Type.Default)
            return name + "=" + defaultValue.ToString();
        else
            return name + "=" + value.ToString();
    }

    public string ValueString()
    {
        return value.ToString();
    }


    /// <summary>
    /// Check whether x is in the interval {a,b}, closed or open as
    /// specified by the range type
    /// </summary>
    /// <returns>true if x is in the interval, false otherwise</returns>
    private bool CheckRange(T x, T a, T b, ConfigurationManager.RangeType t)
    {
        int cmpAX = a.CompareTo(x);
        int cmpXB = x.CompareTo(b);
        switch (t)
        {
            case ConfigurationManager.RangeType.Closed:
                return cmpAX <= 0 && cmpXB <= 0;
            case ConfigurationManager.RangeType.Open:
                return cmpAX <  0 && cmpXB <  0;
            case ConfigurationManager.RangeType.LeftOpen:
                return cmpAX <  0 && cmpXB <= 0;
            case ConfigurationManager.RangeType.RightOpen:
                return cmpAX <= 0 && cmpXB <  0;
        }
        return false;
    }

    public string Name()
    {
        return name;
    }
}
