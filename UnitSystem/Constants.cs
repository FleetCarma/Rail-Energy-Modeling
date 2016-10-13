using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitSystem
{
    internal static class Constants
    {
        /// <summary>
        /// Number of kilometres in a mile.
        /// </summary>
        public const double KmPerMi = 1.609344;

        /// <summary>
        /// Number of feet per metre
        /// </summary>
        public const double ftPerMetre = 3.28084;

        /// <summary>
        /// Number of feet per mile
        /// </summary>
        public const double ftPerMile = 5280;

        /// <summary>
        /// Number of square feet per square metre
        /// </summary>
        public const double SqFeetPerSqMetre = 10.7639;

        /// <summary>
        /// Number of kilograms in a US ton.
        /// </summary>
        public const double KgPerUsTon = 907.1847;

        /// <summary>
        /// The number of pounds in a kilogram
        /// </summary>
        public const double PoundsPerKilogram = 2.20462;

        /// <summary>
        /// Number of Kilograms in a Metric Ton
        /// </summary>
        public const double KgPerMetricTon = 1000;

        /// <summary>
        /// The number of pounds in a US Ton
        /// </summary>
        public const double PoundsPerUsTon = 2000;

        /// <summary>
        /// This is the number of Litres in a Imperial Gallon
        /// </summary>
        public const double LitresPerImpGallon = 4.54609;

        /// <summary>
        /// This is the number of Imperial Gallon in a Gallon
        /// </summary>
        public const double ImpGallonPerGallon = 0.832674188148;

        /// <summary>
        /// This is the number of Litres in a Gallon
        /// </summary>
        public const double LitresPerGallon = 3.785411784;

        /// <summary>
        /// Number of US gallons in a litre.
        /// </summary>
        public const double GallonsPerLitre = 1.0 / LitresPerGallon;

        /// <summary>
        /// Number of imperial gallons in a litre.
        /// </summary>
        public const double ImpGallonsPerLitre = 1.0 / LitresPerImpGallon;

        /// <summary>
        /// Number of Gallon in an Imperial Gallon
        /// </summary>
        public const double GallonsPerImpGallon = 1.0 / ImpGallonPerGallon;

        /// <summary>
        /// Number of Newton Metre per Pound-feet
        /// </summary>
        public const double NewtonMetrePerlbft = 1.355818;

        /// <summary>
        /// Number of Pound-feet per Newton Metre
        /// </summary>
        public const double lbftPerNewtonMetre = 1.0 / NewtonMetrePerlbft;

        public const double WattsPerHorsepower = 745.69987;

        public const double RPMPerRads = 9.5493;



        /// <summary>
        /// Watt-hours of energy in a US gallon of regular gasoline.
        /// </summary>
        public const double WhPerGal = 33705;

        public const double WattHoursPerBTU = 0.293071;
    }
}
