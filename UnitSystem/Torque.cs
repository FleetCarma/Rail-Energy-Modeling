using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitSystem
{
    /// <summary>
    /// A Torque quantity.
    /// </summary>
    public class Torque : Quantity
    {
        /// <summary>
        /// Supported units of torque.
        /// </summary>
        private static Unit[] supportedUnits = { Unit.Nm, Unit.lbft };

        /// <summary>
        /// Conversion factors.
        /// </summary>
        private static Tuple<Unit, Unit, double>[] conversionFactors =
        {
            //           Source,        Target,         Multiplicative Factor
            Tuple.Create(Unit.Nm,        Unit.Nm,         1.0),
            Tuple.Create(Unit.Nm,        Unit.lbft,       Constants.lbftPerNewtonMetre),

            Tuple.Create(Unit.lbft,      Unit.lbft,       1.0),
            Tuple.Create(Unit.lbft,      Unit.Nm,         1.0 / Constants.lbftPerNewtonMetre)
            
        };

        /// <summary>
        /// Create a new volume quantity.
        /// </summary>
        /// <param name="value">The numerical value.</param>
        /// <param name="unit">The physical unit.</param>
        public Torque(double value, Unit unit)
            : base(value, unit)
        {
            if (!supportedUnits.Contains(unit))
            {
                throw new ArgumentException(unit + " is not a valid unit of volume.");
            }
        }

        public override UnitDescription GetUnitDescription(IUnitSystem unitSystem)
        {
            return unitSystem.Volume;
        }

        public static Unit CanonicalUnit
        {
            get { return Unit.Nm; }
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
