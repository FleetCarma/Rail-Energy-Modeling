using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitSystem
{
    /// <summary>
    /// A volume quantity.
    /// </summary>
    public class Volume : Quantity
    {
        /// <summary>
        /// Supported units of volume.
        /// </summary>
        private static Unit[] supportedUnits = { Unit.L, Unit.Gal };

        /// <summary>
        /// Conversion factors.
        /// </summary>
        private static Tuple<Unit, Unit, double>[] conversionFactors =
        {
            //           Source,        Target,         Multiplicative Factor
            Tuple.Create(Unit.L,        Unit.L,         1.0),
            Tuple.Create(Unit.L,        Unit.Gal,       Constants.GallonsPerLitre),

            Tuple.Create(Unit.Gal,      Unit.Gal,       1.0),
            Tuple.Create(Unit.Gal,      Unit.L,         Constants.LitresPerGallon)
            
        };

        /// <summary>
        /// Create a new volume quantity.
        /// </summary>
        /// <param name="value">The numerical value.</param>
        /// <param name="unit">The physical unit.</param>
        public Volume(double value, Unit unit)
            : base(value, unit)
        {
            if (!supportedUnits.Contains(unit))
            {
                throw new ArgumentException(unit + " is not a valid unit of volume.");
            }
        }
        public Volume(double value)
            : this(value, CanonicalUnit)
        {
        }

        public override UnitDescription GetUnitDescription(IUnitSystem unitSystem)
        {
            return unitSystem.Volume;
        }

        public static Unit CanonicalUnit
        {
            get { return Unit.L; }
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
