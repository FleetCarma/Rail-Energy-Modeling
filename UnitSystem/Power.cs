using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitSystem
{
    /// <summary>
    /// The amount of power consumed
    /// </summary>
    public class Power : Quantity
    {
        /// <summary>
        /// Supported units of power
        /// </summary>
        private static Unit[] supportedUnits = { Unit.KW, Unit.W };


        private static Tuple<Unit, Unit, double>[] conversionFactors =
        {
            //           Source,    Target,     Multiplicative Factor
            Tuple.Create(Unit.KW,  Unit.KW,   1.0),
            Tuple.Create(Unit.KW,  Unit.W,    1000.0),
            
            Tuple.Create(Unit.W,   Unit.KW,   0.001),
            Tuple.Create(Unit.W,   Unit.W,    1.0)

        };

        /// <summary>
        /// Create a new energy consumption quantity.
        /// </summary>
        /// <param name="value">The numerical value.</param>
        /// <param name="unit">The physical unit.</param>
        public Power(double value, Unit unit)
            : base(value, unit)
        {
            if (!supportedUnits.Contains(unit))
            {
                throw new ArgumentException(unit + " is not a valid unit of power");
            }
        }

        public override UnitDescription GetUnitDescription(IUnitSystem unitSystem)
        {
            return unitSystem.Power;
        }

        public static Unit CanonicalUnit
        {
            get { return Unit.KW; }
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
