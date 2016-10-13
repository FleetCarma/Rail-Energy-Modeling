using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitSystem
{
    /// <summary>
    /// The Metric (SI) unit system.
    /// </summary>
    public class MetricSystem : IUnitSystem
    {
        #region IUnitSystem Members

        public virtual UnitSystemType Type
        {
            get
            {
                return UnitSystemType.Metric;
            }
        }

        public UnitDescription Distance
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.Km);
            }
        }

        public UnitDescription ShortDistance
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.m);
            }
        }

        public UnitDescription Area
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.sqm);
            }
        }

        public UnitDescription Temperature
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.Celsius);
            }
        }

        public UnitDescription Mass
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.Kg);
            }
        }

        public UnitDescription Volume
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.L);
            }
        }

        public UnitDescription FuelConsumption
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.LPer100Km);
            }
        }

        public virtual UnitDescription EnergyConsumption
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.LPer100KmEquiv);
            }
        }

        public UnitDescription WhConsumption
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.WhPerKm);
            }
        }

        public UnitDescription LiquidConsumptionEq
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.LPer100KmEquiv);
            }
        }

        public UnitDescription Energy
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.KWh);
            }
        }

        public UnitDescription Emissions
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.GPerKm);
            }
        }

        public UnitDescription Power
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.KW);
            }
        }

        public UnitDescription MechanicalPower
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.KW);
            }
        }

        public UnitDescription Speed
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.KPH);
            }
        }

        public UnitDescription UpstreamFuelEmissions
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.GperL);
            }
        }

        public UnitDescription UpstreamElectricalEmissions
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.GPerkWh);
            }
        }

        public UnitDescription Torque
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.Nm);
            }

        }

        public UnitDescription RotationalSpeed
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.rads);
            }
        }

        #endregion
    }
}
