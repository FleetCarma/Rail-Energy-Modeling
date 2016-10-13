using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitSystem
{
    /// <summary>
    /// The interface for a system of units.
    /// </summary>
    public interface IUnitSystem
    {
        /// <summary>
        /// The unit system type (e.g. Metric).
        /// </summary>
        UnitSystemType Type { get; }

        /// <summary>
        /// The distance unit implemented by this unit system.
        /// </summary>
        UnitDescription Distance { get; }

        /// <summary>
        /// The "short" distance unit implemented by this unit system.
        /// </summary>
        UnitDescription ShortDistance { get; }

        /// <summary>
        /// The area unit implemented by this unit system.
        /// </summary>
        UnitDescription Area { get; }

        /// <summary>
        /// The temperature unit implemented by this unit system.
        /// </summary>
        UnitDescription Temperature { get; }

        /// <summary>
        /// The mass unit implemented by this unit system.
        /// </summary>
        UnitDescription Mass { get; }

        /// <summary>
        /// The volume unit implemented by this unit system.
        /// </summary>
        UnitDescription Volume { get; }

        /// <summary>
        /// The fuel consumption unit implemented by this unit system.
        /// </summary>
        UnitDescription FuelConsumption { get; }

        /// <summary>
        /// The energy consumption unit implemented by this unit system.
        /// </summary>
        UnitDescription EnergyConsumption { get; }

        /// <summary>
        /// The units of Wh consumption implememnted by this unit system.
        /// </summary>
        UnitDescription WhConsumption { get; }

        /// <summary>
        /// The units of liquid consumption implemented by this unit system.
        /// </summary>
        UnitDescription LiquidConsumptionEq { get; }

        /// <summary>
        /// The energy unit implemented by this unit system.
        /// </summary>
        UnitDescription Energy { get; }

        /// <summary>
        /// The emissions unit implemented by this unit system.
        /// </summary>
        UnitDescription Emissions { get; }

        /// <summary>
        /// The power unit implemented by this unit system.
        /// </summary>
        UnitDescription Power { get; }

        /// <summary>
        /// The mechanical power unit implemented by this unit system.
        /// </summary>
        UnitDescription MechanicalPower { get; }

        /// <summary>
        /// The speed unit implemented by this unit system
        /// </summary>
        UnitDescription Speed { get; }

        /// <summary>
        /// The upstream fuel emissions unit implemented by this unit system
        /// </summary>
        UnitDescription UpstreamFuelEmissions { get; }

        /// <summary>
        /// The upstream electrical emissions unit implemented by this unit system
        /// </summary>
        UnitDescription UpstreamElectricalEmissions { get; }

        UnitDescription Torque { get; }

        UnitDescription RotationalSpeed { get; }
    }
}
