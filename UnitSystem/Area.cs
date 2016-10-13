using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitSystem
{
    /// <summary>
    /// A distance quantity.
    /// </summary>
    public class Area : Quantity
    {
        /// <summary>
        /// Supported units of distance.
        /// </summary>
        private static Unit[] supportedUnits = { Unit.sqm, Unit.sqft };

        /// <summary>
        /// Conversion factors.
        /// </summary>
        private static Tuple<Unit, Unit, double>[] conversionFactors =
        {
            //           Source,    Target,     Multiplicative Factor
            Tuple.Create(Unit.sqm,   Unit.sqm,    1.0),
            Tuple.Create(Unit.sqm,   Unit.sqft,    Constants.SqFeetPerSqMetre),

            Tuple.Create(Unit.sqft,   Unit.sqm,    1.0 / Constants.SqFeetPerSqMetre),
            Tuple.Create(Unit.sqft,   Unit.sqft,    1.0)
        };

        /// <summary>
        /// Create a new distance quantity.
        /// </summary>
        /// <param name="value">The numerical value.</param>
        /// <param name="unit">The physical unit.</param>
        public Area(double value, Unit unit)
            : base(value, unit)
        {
            if (!supportedUnits.Contains(unit))
            {
                throw new ArgumentException(unit + " is not a valid unit of distance.");
            }
        }

        public Area(double value)
            : this(value, CanonicalUnit)
        {
        }

        public override UnitDescription GetUnitDescription(IUnitSystem unitSystem)
        {
            return unitSystem.Area;
        }

        public static Unit CanonicalUnit
        {
            get { return Unit.sqm; }
        }

        public override double CanonicalValue
        {
            get { return this.GetValue(CanonicalUnit); }
        }

        public override double GetValue(Unit targetUnit)
        {
            return ConvertValue(conversionFactors, targetUnit);
        }
    }
}
