using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitSystem
{
    /// <summary>
    /// The Imperial Unit System
    /// </summary>
    public class USImperialSystem : IUnitSystem
    {
        #region IUnitSystem Members

        public virtual UnitSystemType Type
        {
            get
            {
                return UnitSystemType.US;
            }
        }

        public UnitDescription Distance
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.Mi);
            }
        }

        public UnitDescription ShortDistance
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.ft);
            }
        }

        public UnitDescription Area
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.sqft);
            }
        }

        public UnitDescription Temperature
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.Fahrenheit);
            }
        }

        public UnitDescription Mass
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.Lb);
            }
        }

        public UnitDescription Volume
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.Gal);
            }
        }

        public UnitDescription FuelConsumption
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.MiPerGal);
            }
        }

        public virtual UnitDescription EnergyConsumption
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.MiPerGalEquiv);
            }
        }

        public UnitDescription WhConsumption
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.WhPerMi);
            }
        }


        public UnitDescription LiquidConsumptionEq
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.MiPerGalEquiv);
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
                return UnitDescriptionFactory.GetUnitDescription(Unit.LbPerMi);
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
                return UnitDescriptionFactory.GetUnitDescription(Unit.hp);
            }
        }

        public UnitDescription Speed
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.MPH);
            }
        }

        public UnitDescription UpstreamFuelEmissions
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.LbPerGal);
            }
        }

        public UnitDescription UpstreamElectricalEmissions
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.LbPerkWh);
            }
        }

        public UnitDescription Torque
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.lbft);
            }

        }

        public UnitDescription RotationalSpeed
        {
            get
            {
                return UnitDescriptionFactory.GetUnitDescription(Unit.rpm);
            }
        }

        #endregion
    }
}
