using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrainSimulatorCLI;

namespace REMForm.Helpers
{
    /// <summary>
    /// Helper class for storing parameters to be fed into the simulation
    /// Since values in the _params file are all stored as arrays, this acts as mid-point for the data, making it easier to process
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// The identifer of the parameter
        /// </summary>
        public string Identifier { get; private set; }

        /// <summary>
        /// List of values
        /// </summary>
        public double[] Values { get; private set; }

        /// <summary>
        /// Returns the ParamStruct equivalent of itself
        /// </summary>
        public ParamStruct CLIStruct
        {
            get
            {
                ParamStruct cliStruct = new ParamStruct
                {
                    Identifier = Identifier,
                    Values = Values
                };

                return cliStruct;
            }
        }

        /// <summary>
        /// Base constructor of the parameter object. Can only be called by the other constructors
        /// </summary>
        /// <param name="identifier">The identifier of the parameter</param>
        private Parameter(string identifier)
        {
            Identifier = identifier;
        }

        /// <summary>
        /// Constructor for handling a parameter with a single value
        /// </summary>
        /// <param name="identifier">The identifier of the parameter</param>
        /// <param name="value">The value associated with this parameter</param>
        public Parameter(string identifier, double value)
            : this(identifier)
        {
            Values = new double[1] { value };
        }

        /// <summary>
        /// Constructor for handling a parameter with a 1D array of values
        /// </summary>
        /// <param name="identifier">The identifier of the parameter</param>
        /// <param name="values">The values associated with this parameter</param>
        public Parameter(string identifier, double[] values)
            : this(identifier)
        {
            Values = values;
        }

        /// <summary>
        /// Constructor for handling a parameter with a 2D array of values
        /// Currently unused, but may be potentially useful when this project moves forward
        /// 
        /// The conversion of 2D to 1D mimics the logic used by matlab.
        /// Ex:
        /// Input       Ouput
        /// [1 2 3] --> [1 4 2 5 3 6]
        /// [4 5 6]
        /// </summary>
        /// <param name="identifier">The identifier of the parameter</param>
        /// <param name="values">2D array of values associated with this parameter</param>
        public Parameter(string identifier, double[,] values)
            : this(identifier)
        {
            Values = new double[values.GetLength(0) * values.GetLength(1)];
            for (int c = 0; c < values.GetLength(1); c++)
            {
                for (int r = 0; r < values.GetLength(0); r++)
                {
                    int index = (c * values.GetLength(0)) + r;
                    Values[index] = values[r, c];
                }
            }
        }

        public override string ToString()
        {
            if (Values.Count() == 1)
            {
                return string.Format("Identifier: \"{0}\", Value: {1}", Identifier, Values[0]);
            }
            else
            {
                return string.Format("Identifier: \"{0}\", Value: double[{1}]", Identifier, Values.Count());
            }
        }
    }
}
