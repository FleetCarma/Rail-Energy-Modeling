using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitSystem
{
    /// <summary>
    /// The amount of power consumed
    /// </summary>
    public class MechanicalPower : Quantity
    {
        /// <summary>
        /// Supported units of power
        /// </summary>
        private static Unit[] supportedUnits = { Unit.KW, Unit.W, Unit.hp };


        private static Tuple<Unit, Unit, double>[] conversionFactors =
        {
            //           Source,    Target,     Multiplicative Factor
            Tuple.Create(Unit.KW,  Unit.KW,   1.0),
            Tuple.Create(Unit.KW,  Unit.W,    1000.0),
            Tuple.Create(Unit.KW,   Unit.hp,   1000.0 / Constants.WattsPerHorsepower),
            
            Tuple.Create(Unit.W,   Unit.KW,   0.001),
            Tuple.Create(Unit.W,   Unit.W,    1.0),
            Tuple.Create(Unit.W,   Unit.hp,   1.0 / Constants.WattsPerHorsepower),

            
            Tuple.Create(Unit.hp,  Unit.KW,    Constants.WattsPerHorsepower / 1000.0),
            Tuple.Create(Unit.hp,  Unit.W,    Constants.WattsPerHorsepower),
            Tuple.Create(Unit.hp,  Unit.hp,   1.0)

        };

        /// <summary>
        /// Create a new energy consumption quantity.
        /// </summary>
        /// <param name="value">The numerical value.</param>
        /// <param name="unit">The physical unit.</param>
        public MechanicalPower(double value, Unit unit)
            : base(value, unit)
        {
            if (!supportedUnits.Contains(unit))
            {
                throw new ArgumentException(unit + " is not a valid unit of power");
            }
        }
        public MechanicalPower(double value)
            : this(value, CanonicalUnit)
        {
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
