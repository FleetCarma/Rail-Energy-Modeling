using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitSystem
{
    public abstract class Quantity
    {
        /// <summary>
        /// Get the numerical value of this quantity.
        /// </summary>
        protected double Value
        {
            get;
            private set;
        }

        /// <summary>
        /// Get the unit associated with this quantity.
        /// </summary>
        public Unit Unit
        {
            get;
            private set;
        }

        /// <summary>
        /// Create a new Quantity.
        /// </summary>
        /// <param name="value">The numerical value of the quantity.</param>
        /// <param name="unit">The physical unit of the quantity.</param>
        protected Quantity(double value, Unit unit)
        {
            Value = value;
            Unit = unit;
        }

        /// <summary>
        /// Convert our value.
        /// </summary>
        /// <param name="conversionFactors">Array of possible valid conversion factors.</param>
        /// <param name="targetUnit">The unit we are converting to.</param>
        /// <returns>The converted value.</returns>
        protected double ConvertValue(Tuple<Unit, Unit, double>[] conversionFactors, Unit targetUnit)
        {
            Tuple<Unit, Unit, double> factor = conversionFactors.FirstOrDefault(tuple => tuple.Item1 == Unit && tuple.Item2 == targetUnit);
            if (factor == null)
            {
                throw new ArgumentException(String.Format("No conversion factor exists for converting {0} to {1}", Unit, targetUnit));
            }
            return Value * factor.Item3;
        }

        /// <summary>
        /// Get this Quantity's unit description in the given unit system.
        /// </summary>
        /// <param name="unitSystem">The unit system.</param>
        /// <returns>The Quantity's unit description.</returns>
        public abstract UnitDescription GetUnitDescription(IUnitSystem unitSystem);

        public abstract double CanonicalValue { get; }

        /// <summary>
        /// Get the value of this quantity in the given unit.
        /// </summary>
        /// <param name="targetUnit">The unit to get the value as.</param>
        /// <returns>The value of this quantity in the given unit.</returns>
        public abstract double GetValue(Unit targetUnit);
    }
}
