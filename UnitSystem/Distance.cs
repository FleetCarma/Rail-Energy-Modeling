using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitSystem
{
    /// <summary>
    /// A distance quantity.
    /// </summary>
    public class Distance : Quantity
    {
        /// <summary>
        /// Supported units of distance.
        /// </summary>
        private static Unit[] supportedUnits = { Unit.Km, Unit.Mi, Unit.m, Unit.ft };

        /// <summary>
        /// Conversion factors.
        /// </summary>
        private static Tuple<Unit, Unit, double>[] conversionFactors =
        {
            //           Source,    Target,     Multiplicative Factor
            Tuple.Create(Unit.Km,   Unit.Km,    1.0),
            Tuple.Create(Unit.Km,   Unit.Mi,    1.0 / Constants.KmPerMi),
            Tuple.Create(Unit.Km,   Unit.m,    1000.0),
            Tuple.Create(Unit.Km,   Unit.ft,    Constants.ftPerMile / Constants.KmPerMi ),

            Tuple.Create(Unit.Mi,   Unit.Km,    Constants.KmPerMi),
            Tuple.Create(Unit.Mi,   Unit.Mi,    1.0),
            Tuple.Create(Unit.Mi,   Unit.m,    Constants.KmPerMi * 1000.0),
            Tuple.Create(Unit.Mi,   Unit.ft,    Constants.ftPerMile),

            Tuple.Create(Unit.m,   Unit.Km,    1.0/1000.0),
            Tuple.Create(Unit.m,   Unit.Mi,    (1.0/1000.0) / Constants.KmPerMi),
            Tuple.Create(Unit.m,   Unit.m,    1.0),
            Tuple.Create(Unit.m,   Unit.ft,    Constants.ftPerMetre),

            Tuple.Create(Unit.ft,   Unit.Km,    1.0 / (Constants.ftPerMetre * 1000.0)),
            Tuple.Create(Unit.ft,   Unit.Mi,    1.0 / Constants.ftPerMile),
            Tuple.Create(Unit.ft,   Unit.m,    1.0 / Constants.ftPerMetre),
            Tuple.Create(Unit.ft,   Unit.ft,    1.0),
        };

        /// <summary>
        /// Create a new distance quantity.
        /// </summary>
        /// <param name="value">The numerical value.</param>
        /// <param name="unit">The physical unit.</param>
        public Distance(double value, Unit unit)
            : base(value, unit)
        {
            if (!supportedUnits.Contains(unit))
            {
                throw new ArgumentException(unit + " is not a valid unit of distance.");
            }
        }

        public Distance(double value)
            : this(value, CanonicalUnit)
        {
        }

        public override UnitDescription GetUnitDescription(IUnitSystem unitSystem)
        {
            return unitSystem.Distance;
        }

        public static Unit CanonicalUnit
        {
            get { return Unit.Km; }
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
