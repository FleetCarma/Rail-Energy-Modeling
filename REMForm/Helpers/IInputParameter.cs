using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace REMForm.Helpers
{
    /// <summary>
    /// Abstract class used to force models to provide a list of parameters representing the data they contain
    /// This data will be fed into the simulation
    /// </summary>
    interface IInputParameter
    {
        IEnumerable<Parameter> InputParameters();
    }
}
