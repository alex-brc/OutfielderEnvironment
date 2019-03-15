using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class VariableContainer : Container
{
    public InputField field;

    public override string RetrieveContent()
    {
        return field.text.ToString();
    }

    public override void SetContent(string content)
    {
        field.text = content;
    }
}