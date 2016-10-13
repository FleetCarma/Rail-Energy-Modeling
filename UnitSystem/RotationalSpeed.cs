using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitSystem
{
    public class RotationalSpeed : Quantity
    {
        /// <summary>
        /// Supported units of Rotational Speed.
        /// </summary>
        private static Unit[] supportedUnits = { Unit.rads, Unit.rpm };

        /// <summary>
        /// Conversion factors.
        /// </summary>
        private static Tuple<Unit, Unit, double>[] conversionFactors =
        {
            //           Source,        Target,     Multiplicative Factor
            Tuple.Create(Unit.rads,      Unit.rads,   1.0),
            Tuple.Create(Unit.rads,      Unit.rpm,   Constants.RPMPerRads),
            Tuple.Create(Unit.rpm,      Unit.rpm,   1.0),
            Tuple.Create(Unit.rpm,      Unit.rads,   1.0 / Constants.RPMPerRads)
        };

        /// <summary>
        /// Create a new Rotational Speed quantity.
        /// </summary>
        /// <param name="value">The numerical value.</param>
        /// <param name="unit">The physical unit.</param>
        public RotationalSpeed(double value, Unit unit)
            : base(value, unit)
        {
            if (!supportedUnits.Contains(unit))
            {
                throw new ArgumentException(unit + " is not a valid unit of Rotational Speed.");
            }
        }

        public override UnitDescription GetUnitDescription(IUnitSystem unitSystem)
        {
            return unitSystem.RotationalSpeed;
        }

        public static Unit CanonicalUnit
        {
            get { return Unit.rads; }
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
