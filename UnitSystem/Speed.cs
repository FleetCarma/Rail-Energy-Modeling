using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitSystem
{
    public class Speed : Quantity
    {
        /// <summary>
        /// Supported units of Speed.
        /// </summary>
        private static Unit[] supportedUnits = { Unit.KPH, Unit.MPH };

        /// <summary>
        /// Conversion factors.
        /// </summary>
        private static Tuple<Unit, Unit, double>[] conversionFactors =
        {
            //           Source,        Target,     Multiplicative Factor
            Tuple.Create(Unit.KPH,      Unit.KPH,   1.0),
            Tuple.Create(Unit.KPH,      Unit.MPH,   1.0 / Constants.KmPerMi),
            Tuple.Create(Unit.MPH,      Unit.MPH,   1.0),
            Tuple.Create(Unit.MPH,      Unit.KPH,   Constants.KmPerMi)
        };

        /// <summary>
        /// Create a new Speed quantity.
        /// </summary>
        /// <param name="value">The numerical value.</param>
        /// <param name="unit">The physical unit.</param>
        public Speed(double value, Unit unit)
            : base(value, unit)
        {
            if (!supportedUnits.Contains(unit))
            {
                throw new ArgumentException(unit + " is not a valid unit of Speed.");
            }
        }

        public Speed(double value)
            : this(value, CanonicalUnit)
        {
        }

        public override UnitDescription GetUnitDescription(IUnitSystem unitSystem)
        {
            return unitSystem.Speed;
        }

        public static Unit CanonicalUnit
        {
            get { return Unit.KPH; }
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
