using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitSystem
{
    /// <summary>
    /// Unit types.
    /// </summary>
    public enum Unit
    {
        /// <summary>
        /// Kilometre.
        /// </summary>
        Km,

        /// <summary>
        /// Mile.
        /// </summary>
        Mi,

        /// <summary>
        /// Meter
        /// </summary>
        m,

        /// <summary>
        /// Foot
        /// </summary>
        ft,

        /// <summary>
        /// Meter^2
        /// </summary>
        sqm,

        /// <summary>
        /// Foot^2
        /// </summary>
        sqft,

        /// <summary>
        /// Grams.
        /// </summary>
        G,

        /// <summary>
        /// Kilograms.
        /// </summary>
        Kg,

        /// <summary>
        /// Pounds
        /// </summary>
        Lb,

        /// <summary>
        /// US Ton. AKA short ton
        /// </summary>
        UsTon,

        /// <summary>
        /// Metric Ton
        /// </summary>
        MetricTon,

        /// <summary>
        /// Litres.
        /// </summary>
        L,

        /// <summary>
        /// US gallons.
        /// </summary>
        Gal,

        /// <summary>
        /// Celsius.
        /// </summary>
        Celsius,

        /// <summary>
        /// Fahrenheit.
        /// </summary>
        Fahrenheit,

        /// <summary>
        /// Litres per 100 kilometres.
        /// </summary>
        LPer100Km,

        /// <summary>
        /// Watt-hours per kilometre.
        /// </summary>
        WhPerKm,

        /// <summary>
        /// Watt-hours per mile.
        /// </summary>
        WhPerMi,

        /// <summary>
        /// Miles per gallon equivalent.
        /// </summary>
        MiPerGalEquiv,

        /// <summary>
        /// Litres per 100 km equivalent.
        /// </summary>
        LPer100KmEquiv,

        /// <summary>
        /// Miles per gallon.
        /// </summary>
        MiPerGal,

        /// <summary>
        /// Kilowatt-hours.
        /// </summary>
        KWh,

        /// <summary>
        /// Watt hours
        /// </summary>
        Wh,

        /// <summary>
        /// British Thermal Unit
        /// </summary>
        btu,

        /// <summary>
        /// Kilowatts.
        /// </summary>
        KW,

        /// <summary>
        /// Watts
        /// </summary>
        W,

        /// <summary>
        /// Grams per kilometre.
        /// </summary>
        GPerKm,

        /// <summary>
        /// Grams per mile.
        /// </summary>
        GPerMi,

        /// <summary>
        /// Pounds per mile
        /// </summary>
        LbPerMi,

        /// <summary>
        /// Grams per Litre.
        /// </summary>
        GperL,

        /// <summary>
        /// Grams per Kilwatt-hour.
        /// </summary>
        GPerkWh,

        /// <summary>
        /// Pounds per US Gallon.
        /// </summary>
        LbPerGal,

        /// <summary>
        /// Pounds per Kilwatt-hour.
        /// </summary>
        LbPerkWh,

        /// <summary>
        /// Miles per hour
        /// </summary>
        MPH,

        /// <summary>
        /// Kilometers per hour
        /// </summary>
        KPH,

        /// <summary>
        /// Horsepower
        /// </summary>
        hp,

        /// <summary>
        /// Rotations per minute
        /// </summary>
        rpm,

        /// <summary>
        /// radians per minute
        /// </summary>
        rads,

        /// <summary>
        /// Newton Meters
        /// </summary>
        Nm,

        /// <summary>
        /// Pound-foot
        /// </summary>
        lbft
    }

    public enum UnitSystem
    {
        /// <summary>
        /// The metric (SI) system.
        /// </summary>
        Metric,

        /// <summary>
        /// The US customary unit system.
        /// </summary>
        US
    }
}
