using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DropdownContainer : Container
{
    public Dropdown dropdown;

    public override string RetrieveContent()
    {
        return dropdown.value.ToString();
    }

    public override void SetContent(string content)
    {
        dropdown.value = int.Parse(content); // guaranteed to be okay
    }
}