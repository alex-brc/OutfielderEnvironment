using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface Collector
{
    /// <summary>
    /// Called by the data manager, makes this collector
    /// start collecting and writing data from the 
    /// attached object.
    /// </summary>
    void StartCollecting();

    /// <summary>
    /// Called by the data manager, makes this collector
    /// stop collecting data from the attached object
    /// </summary>
    void StopCollecting();
}
