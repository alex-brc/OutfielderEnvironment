using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IVariable
{
    bool TryParse(string s);
    string Name();
    string ValueString();
    bool Pull();
    bool Check();
    void Push(bool flash);
    string ToString();
}
