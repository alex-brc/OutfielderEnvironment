using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ToggleContainer : Container
{
    public Toggle toggle;

    public override string RetrieveContent()
    {
        return toggle.isOn.ToString();
    }

    public override void SetContent(string content)
    {
        toggle.isOn = bool.Parse(content);
    }
}