﻿using System;
using System.ComponentModel;
using System.Globalization;
using UnityEngine;

[TypeConverter(typeof(CVectorConverter))]
public class CVectorConverter : TypeConverter {

    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
        if (sourceType == typeof(string))
        {
            return true;
        }

        return base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {


        return base.ConvertTo(context, culture, value, destinationType);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        if (value is string)
            return new CVector(value as string);

        return base.ConvertFrom(context, culture, value);
    }

}