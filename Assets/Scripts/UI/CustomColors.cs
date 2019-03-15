using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomColors
{
    
    public static readonly Color 
        Green = new Color(108 / 255.0f, 247 / 255.0f, 86 / 255.0f),
        SoftGreen = new Color(124 / 255.0f, 255 / 255.0f, 112 / 255.0f),
        Orange = new Color(255 / 255.0f, 195 / 255.0f, 30 / 255.0f),
        Red = new Color(244 / 255.0f, 66 / 255.0f, 66 / 255.0f),
        SoftRed = new Color(255 / 255.0f, 76 / 255.0f, 76 / 255.0f),
        Black = new Color(50 / 255.0f, 50 / 255.0f, 50 / 255.0f),
        White = new Color(1, 1, 1);

    public static class Background
    {
        public static readonly Color
        Green = new Color(108 / 255.0f, 247 / 255.0f, 86 / 255.0f, 0.25f),
        Orange = new Color(255 / 255.0f, 195 / 255.0f, 30 / 255.0f, 0.25f),
        Red = new Color(244 / 255.0f, 66 / 255.0f, 66 / 255.0f, 0.25f),
        Black = new Color(50 / 255.0f, 50 / 255.0f, 50 / 255.0f, 0.25f),
        White = new Color(1, 1, 1, 0.25f),
        BrightWhite = new Color(1, 1, 1, 0.65f);
    }
}
