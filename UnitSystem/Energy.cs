using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitSystem
{
    /// <summary>
    /// The amount of energy consumed
    /// </summary>
    public class Energy : Quantity
    {
        /// <summary>
        /// Supported units of energy
        /// </summary>
        private static Unit[] supportedUnits = { Unit.KWh, Unit.Wh, Unit.L, Unit.btu };


        /// <summary>
        /// Conversion factors.
        /// </summary>
        private static Tuple<Unit, Unit, double>[] conversionFactors =
        {
            //           Source,    Target,     Multiplicative Factor
            Tuple.Create(Unit.KWh,  Unit.KWh,   1.0),
            Tuple.Create(Unit.KWh,  Unit.Wh,   1000.0),
            Tuple.Create(Unit.KWh,  Unit.L,   1000.0 / Constants.WhPerGal / Constants.GallonsPerLitre),
            Tuple.Create(Unit.KWh,  Unit.btu,   1000.0 / Constants.WattHoursPerBTU),

            Tuple.Create(Unit.Wh,  Unit.KWh,   1.0 / 1000),
            Tuple.Create(Unit.Wh,  Unit.Wh,   1.0),
            Tuple.Create(Unit.Wh,  Unit.L,   Constants.WhPerGal / Constants.GallonsPerLitre),
            Tuple.Create(Unit.Wh,  Unit.btu,   1.0 / Constants.WattHoursPerBTU),

            Tuple.Create(Unit.L, Unit.KWh, Constants.GallonsPerLitre * Constants.WhPerGal / 1000),
            Tuple.Create(Unit.L, Unit.Wh, Constants.GallonsPerLitre * Constants.WhPerGal),
            Tuple.Create(Unit.L, Unit.L, 1.0),
            Tuple.Create(Unit.L, Unit.btu, Constants.GallonsPerLitre * Constants.WhPerGal / Constants.WattHoursPerBTU),

            Tuple.Create(Unit.btu, Unit.KWh, Constants.WattHoursPerBTU / 1000),
            Tuple.Create(Unit.btu, Unit.Wh, Constants.WattHoursPerBTU),
            Tuple.Create(Unit.btu, Unit.L, Constants.WattHoursPerBTU / (Constants.WhPerGal / Constants.GallonsPerLitre)),
            Tuple.Create(Unit.btu, Unit.btu, 1.0),

        };

        /// <summary>
        /// Create a new energy consumption quantity.
        /// </summary>
        /// <param name="value">The numerical value.</param>
        /// <param name="unit">The physical unit.</param>
        public Energy(double value, Unit unit)
            : base(value, unit)
        {
            if (!supportedUnits.Contains(unit))
            {
                throw new ArgumentException(unit + " is not a valid unit of energy");
            }
        }

        public override UnitDescription GetUnitDescription(IUnitSystem unitSystem)
        {
            return unitSystem.Energy;
        }

        public static Unit CanonicalUnit
        {
            get { return Unit.KWh; }
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
