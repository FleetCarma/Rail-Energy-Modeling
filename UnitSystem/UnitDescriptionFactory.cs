using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitSystem
{

    /// <summary>
    /// Helper class for getting unit descriptions.
    /// </summary>
    public static class UnitDescriptionFactory
    {
        /// <summary>
        /// Dictionary of Unit types to UnitDescriptions.
        /// </summary>
        private static readonly Dictionary<Unit, UnitDescription> unitDescriptions = new Dictionary<Unit, UnitDescription>
        {
            { Unit.Celsius, new UnitDescription(Unit.Celsius, "&deg;C", 1) },
            { Unit.Fahrenheit, new UnitDescription(Unit.Fahrenheit, "&deg;F", 1) },
            { Unit.Gal, new UnitDescription(Unit.Gal, "gal", 0) },
            { Unit.Kg, new UnitDescription(Unit.Kg, "kg", 0) },
            { Unit.Km, new UnitDescription(Unit.Km, "km", 0) },
            { Unit.KW, new UnitDescription(Unit.KW, "kW", 0) },
            { Unit.W, new UnitDescription(Unit.W, "W", 0) },
            { Unit.KWh, new UnitDescription(Unit.KWh, "kWh", 0) },
            { Unit.Wh, new UnitDescription(Unit.Wh, "Wh", 0) },
            { Unit.L, new UnitDescription(Unit.L, "L", 0) },
            { Unit.UsTon, new UnitDescription(Unit.UsTon, "tons", 2) },
            { Unit.MetricTon, new UnitDescription(Unit.MetricTon, "tons", 2) },
            { Unit.LbPerMi, new UnitDescription(Unit.LbPerMi, "lb/mi", 2) },
            { Unit.Lb, new UnitDescription(Unit.Lb, "lb", 0) },
            { Unit.G, new UnitDescription (Unit.G,"g",0) },
            { Unit.LPer100Km, new UnitDescription(Unit.LPer100Km, "L/100 km", 1) },
            { Unit.Mi, new UnitDescription(Unit.Mi, "mi", 0) },
            { Unit.MiPerGal, new UnitDescription(Unit.MiPerGal, "MPG", 0) },
            { Unit.LPer100KmEquiv, new UnitDescription(Unit.LPer100KmEquiv, "L/100 km<sub>eq</sub>", 1) },
            { Unit.MiPerGalEquiv, new UnitDescription(Unit.MiPerGalEquiv, "MPG<sub>eq</sub>", 0) },
            { Unit.WhPerKm, new UnitDescription(Unit.WhPerKm, "Wh/km", 0) },
            { Unit.WhPerMi, new UnitDescription(Unit.WhPerMi, "Wh/mi", 0) },
            { Unit.GPerKm, new UnitDescription(Unit.GPerKm, "g/km", 0) },
            { Unit.GPerMi, new UnitDescription(Unit.GPerMi, "g/mi", 0) },
            { Unit.KPH, new UnitDescription(Unit.KPH, "km/h", 0) },
            { Unit.MPH, new UnitDescription(Unit.MPH, "MPH", 0) },
            { Unit.GperL, new UnitDescription(Unit.GperL, "g/L", 0) },
            { Unit.LbPerGal, new UnitDescription(Unit.LbPerGal, "lb/gal", 0) },
            { Unit.GPerkWh, new UnitDescription(Unit.GPerkWh, "g/kWh", 0) },
            { Unit.LbPerkWh, new UnitDescription(Unit.LbPerkWh, "lb/kWh", 0) },
            { Unit.hp, new UnitDescription(Unit.hp, "hp", 0) },
            { Unit.btu, new UnitDescription(Unit.btu, "BTU", 0) },
            { Unit.rpm, new UnitDescription(Unit.rpm, "rpm", 0) },
            { Unit.rads, new UnitDescription(Unit.rads, "rad/s", 0) },
            { Unit.Nm, new UnitDescription(Unit.Nm, "N.m", 0) },
            { Unit.lbft, new UnitDescription(Unit.lbft, "lbf-ft", 0) },
            { Unit.sqft, new UnitDescription(Unit.sqft, "ft^2", 0) },
            { Unit.sqm, new UnitDescription(Unit.sqm, "m^2", 0) },
            { Unit.m, new UnitDescription(Unit.m, "m", 0) },
            { Unit.ft, new UnitDescription(Unit.ft, "ft", 0) }
        };

        /// <summary>
        /// Get a unit description for the given unit.
        /// </summary>
        /// <param name="unit">Get the unit description.</param>
        /// <returns>The unit description.</returns>
        public static UnitDescription GetUnitDescription(Unit unit)
        {
            return unitDescriptions[unit];
        }
    }

    /// <summary>
    /// A class describing a unit.
    /// </summary>
    public class UnitDescription
    {
        /// <summary>
        /// The unit type.
        /// </summary>
        public Unit Unit
        {
            get;
            private set;
        }

        /// <summary>
        /// The unit's label.
        /// </summary>
        public string Label
        {
            get;
            private set;
        }

        /// <summary>
        /// The number of decimals to show for the unit
        /// </summary>
        public int Decimals
        {
            get;
            private set;
        }

        /// <summary>
        /// Create a new unit description.
        /// </summary>
        /// <param name="unit">The unit.</param>
        /// <param name="label">The unit's label.</param>
        /// <param name="numDecimals">The number of decimals for the unit</param>
        public UnitDescription(Unit unit, string label, int numDecimals)
        {
            Unit = unit;
            Label = label;
            Decimals = numDecimals;
        }
    }
}
