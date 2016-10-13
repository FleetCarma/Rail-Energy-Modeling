using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitSystem
{
    /// <summary>
    /// An Mass quantity.
    /// </summary>
    public class Mass : Quantity
    {
        /// <summary>
        /// Supported units of Mass.
        /// </summary>
        private static Unit[] supportedUnits = { Unit.Kg, Unit.UsTon, Unit.Lb, Unit.G, Unit.MetricTon };

        /// <summary>
        /// Conversion factors.
        /// </summary>
        private static Tuple<Unit, Unit, double>[] conversionFactors =
        {
            //           Source,            Target,         Multiplicative Factor
            Tuple.Create(Unit.Kg,           Unit.Kg,        1.0),
            Tuple.Create(Unit.Kg,           Unit.UsTon,     1.0 / Constants.KgPerUsTon),
            Tuple.Create(Unit.UsTon,        Unit.Kg,        Constants.KgPerUsTon),
            Tuple.Create(Unit.UsTon,        Unit.UsTon,     1.0),
            Tuple.Create(Unit.Kg,           Unit.G,         1000.0),
            Tuple.Create(Unit.G,            Unit.Kg,        0.001),
            Tuple.Create(Unit.UsTon,        Unit.G,         Constants.KgPerUsTon*1000),
            Tuple.Create(Unit.G,            Unit.UsTon,     1.0 / (Constants.KgPerUsTon*1000)),
            Tuple.Create(Unit.Lb,           Unit.Lb,        1.0),
            Tuple.Create(Unit.Lb,           Unit.Kg,        1.0/Constants.PoundsPerKilogram),
            Tuple.Create(Unit.Kg,           Unit.Lb,        Constants.PoundsPerKilogram),
            Tuple.Create(Unit.Lb,           Unit.UsTon,     1.0/Constants.PoundsPerUsTon),
            Tuple.Create(Unit.UsTon,        Unit.Lb,        Constants.PoundsPerUsTon),
            Tuple.Create(Unit.Lb,           Unit.G,         1000.0/Constants.PoundsPerKilogram),
            Tuple.Create(Unit.G,            Unit.Lb,        Constants.PoundsPerKilogram/1000.0),
            Tuple.Create(Unit.G,            Unit.G,         1.0),
            Tuple.Create(Unit.Kg,           Unit.MetricTon, 1.0 / Constants.KgPerMetricTon),
            Tuple.Create(Unit.UsTon,        Unit.MetricTon, Constants.KgPerUsTon / Constants.KgPerMetricTon),
            Tuple.Create(Unit.G,            Unit.MetricTon, 1.0/Constants.KgPerMetricTon * 0.001),
            Tuple.Create(Unit.Lb,           Unit.MetricTon, 1.0/Constants.PoundsPerKilogram * 1.0/Constants.KgPerMetricTon),
            Tuple.Create(Unit.MetricTon,    Unit.Kg,        Constants.KgPerMetricTon),
            Tuple.Create(Unit.MetricTon,    Unit.UsTon,     Constants.KgPerMetricTon / Constants.KgPerUsTon),
            Tuple.Create(Unit.MetricTon,    Unit.G,         Constants.KgPerMetricTon * 1000.0),
            Tuple.Create(Unit.MetricTon,    Unit.Lb,        Constants.PoundsPerKilogram * Constants.KgPerMetricTon),
        };

        /// <summary>
        /// Create a new Mass quantity.
        /// </summary>
        /// <param name="value">The numerical value.</param>
        /// <param name="unit">The physical unit.</param>
        public Mass(double value, Unit unit)
            : base(value, unit)
        {
            if (!supportedUnits.Contains(unit))
            {
                throw new ArgumentException(unit + " is not a valid unit of Mass.");
            }
        }

        public Mass(double value)
            : this(value, CanonicalUnit)
        {
        }

        public override UnitDescription GetUnitDescription(IUnitSystem unitSystem)
        {
            return unitSystem.Mass;
        }

        public static Unit CanonicalUnit
        {
            get { return Unit.Kg; }
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
