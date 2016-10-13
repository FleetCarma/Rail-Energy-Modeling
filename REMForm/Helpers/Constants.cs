using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace REMForm.Helpers
{
    public class Constants
    {
        /// <summary>
        /// Constant used to convert fuel rate into GHG emissions
        /// </summary>
       // public const double GHGMultiplier = 3.1882;

        /// <summary>
        /// Number of decimals to show on the main form
        /// </summary>
        public const int NumDecimals = 2;

        /// <summary>
        /// Factor for converting fuel Kg to L
        /// </summary>
        public const double KgToLMultiplier = 0.837;

        /// <summary>
        /// Factor for converting meters per second to kilometers per hour
        /// </summary>
        public const double MpsToKph = 3.6;

        /// <summary>
        /// Threshold (in px) to use for filtering out data points to display on the graphs
        /// </summary>
        public const double FilterThreshold = 1;
    }
}
