using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IVariable
{
    bool TryParse(string s);
    string Name();
    string ValueString();
    void Push(bool flashDefaults);
    bool Pull();
    bool Check();
    string ToString();
}
