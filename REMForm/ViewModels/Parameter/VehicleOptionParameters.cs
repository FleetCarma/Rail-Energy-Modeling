using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using REMForm.Helpers;
using TrainSimulatorCLI;
using UnitSystem;

namespace REMForm.ViewModels.Options
{
    /// <summary>
    /// Parameters belonging to th Vehicle options model
    /// </summary>
    public class VehicleOptionParameters : NotifyPropertyChangedHandler, IInputParameter
    {
        // Private variables, stored in canonical units
        public double _locomotiveMass { get; private set;}
        public double _carMass { get; private set; }
        public double _wheelRadius { get; private set; }
        public double _maxSpeed { get; private set; }
        public int _numCars { get; private set; }
        public double _frontalArea { get; private set; }
        public double _dragCoefficient { get; private set; }
        public double _rollingResistance1 { get; private set; }
        public double _rollingResistance2 { get; private set; }
        public double _gearRatio { get; private set; }

        /// <summary>
        /// The locomotive mass of the vehicle
        /// </summary>
        public int LocomotiveMass
        {
            get
            {
                Mass mass = new Mass(_locomotiveMass);
                UnitDescription desc = ApplicationModel.staticUnitSystem.Mass;
                return (int)mass.GetValue(desc.Unit);
            }
            set
            {
                //Get the current unit
                UnitDescription desc = ApplicationModel.staticUnitSystem.Mass;
                //Create the Quantity
                Mass mass = new Mass(value, desc.Unit);

                //store as canonical
                _locomotiveMass = mass.CanonicalValue;
                NotifyPropertyChanged("LocomotiveMass");
            }
        }

        /// <summary>
        /// The mass of each individual car of the vehicle
        /// </summary>
        public int CarMass
        {
            get
            {
                Mass mass = new Mass(_carMass);
                UnitDescription desc = ApplicationModel.staticUnitSystem.Mass;
                return (int)mass.GetValue(desc.Unit);
            }
            set
            {
                //Get the current unit
                UnitDescription desc = ApplicationModel.staticUnitSystem.Mass;
                //Create the Quantity
                Mass mass = new Mass(value, desc.Unit);

                //store as canonical
                _carMass = mass.CanonicalValue;
                NotifyPropertyChanged("CarMass");
            }
        }

        /// <summary>
        /// Radius of each wheel
        /// </summary>
        public double WheelRadius
        {
            get
            {
                Distance dist = new Distance(_wheelRadius);
                UnitDescription desc = ApplicationModel.staticUnitSystem.ShortDistance;
                return dist.GetValue(desc.Unit);
            }
            set
            {
                //Get the current unit
                UnitDescription desc = ApplicationModel.staticUnitSystem.ShortDistance;
                //Create the Quantity
                Distance dist = new Distance(value, desc.Unit);

                //store as canonical
                _wheelRadius = dist.CanonicalValue;
                NotifyPropertyChanged("WheelRadius");
            }
        }

        /// <summary>
        /// Maximum Vehicle Speed
        /// </summary>
        public double MaxSpeed
        {
            get
            {
                Speed val = new Speed(_maxSpeed);
                UnitDescription desc = ApplicationModel.staticUnitSystem.Speed;
                return (int)val.GetValue(desc.Unit);
            }
            set
            {
                //Get the current unit
                UnitDescription desc = ApplicationModel.staticUnitSystem.Speed;
                //Create the Quantity
                Speed val = new Speed(value, desc.Unit);

                //store as canonical
                _maxSpeed = val.CanonicalValue;
                NotifyPropertyChanged("MaxSpeed");
            }
        }

        /// <summary>
        /// The number of cars belonging to the train (does not include locomotive)
        /// </summary>
        public int NumCars
        {
            get { return _numCars; }
            set
            {
                _numCars = value;
                NotifyPropertyChanged("NumCars");
            }
        }

        /// <summary>
        /// The frontal surface area of the vehicle
        /// </summary>
        public double FrontalArea
        {
            get
            {
                Area area = new Area(_frontalArea);
                UnitDescription areaDesc = ApplicationModel.staticUnitSystem.Area;
                return area.GetValue(areaDesc.Unit);
            }
            set
            {
                //Get the current unit
                UnitDescription areaDesc = ApplicationModel.staticUnitSystem.Area;

                //Create the Quantity
                Area area = new Area(value, areaDesc.Unit);

                //store as canonical
                _frontalArea = area.CanonicalValue;

                NotifyPropertyChanged("FrontalArea");
            }
        }

        /// <summary>
        /// The coefficient of drag on the vehicle
        /// </summary>
        public double DragCoefficient
        {
            get { return _dragCoefficient; }
            set
            {
                _dragCoefficient = value;
                NotifyPropertyChanged("DragCoefficient");
            }
        }

        /// <summary>
        /// The rolling resistance of the vehicle.
        /// Currently this is not used in simulation and will have to be added in the future
        /// </summary>
        public double RollingResistance1
        {
            get { return _rollingResistance1; }
            set
            {
                _rollingResistance1 = value;
                NotifyPropertyChanged("RollingResistance1");
            }
        }

        /// <summary>
        /// The rolling resistance of the vehicle.
        /// Currently this is not used in simulation and will have to be added in the future
        /// </summary>
        public double RollingResistance2
        {
            get { return _rollingResistance2; }
            set
            {
                _rollingResistance2 = value;
                NotifyPropertyChanged("RollingResistance2");
            }
        }

        /// <summary>
        /// The final drive gear ratio
        /// </summary>
        public double GearRatio
        {
            get { return _gearRatio; }
            set
            {
                _gearRatio = value;
                NotifyPropertyChanged("GearRatio");
            }
        }

        public override void NotifyAllPropertiesChanged()
        {
            NotifyPropertyChanged(string.Empty);
        }

        #region Min and Max values

        #region Locomotive Mass
        public int Max_LocomotiveMass
        {
            get
            {
                Mass val = new Mass(700000.0);
                UnitDescription desc = ApplicationModel.staticUnitSystem.Mass;
                return (int)val.GetValue(desc.Unit);
            }
        }

        public int Min_LocomotiveMass
        {
            get
            {
                Mass val = new Mass(30000.0);
                UnitDescription desc = ApplicationModel.staticUnitSystem.Mass;
                return (int)val.GetValue(desc.Unit);
            }
        }
        #endregion

        #region Car Mass
        public int Max_CarMass
        {
            get
            {
                Mass val = new Mass(150000.0);
                UnitDescription desc = ApplicationModel.staticUnitSystem.Mass;
                return (int)val.GetValue(desc.Unit);
            }
        }

        public int Min_CarMass
        {
            get
            {
                Mass val = new Mass(20000.0);
                UnitDescription desc = ApplicationModel.staticUnitSystem.Mass;
                return (int)val.GetValue(desc.Unit);
            }
        }
        #endregion

        #region Num Cars
        public int Max_NumCars
        {
            get
            {
                return 450;
            }
        }

        public int Min_NumCars
        {
            get
            {
                return 0;
            }
        }
        #endregion

        #region Max Speed
        public int Max_MaxSpeed
        {
            get
            {
                Speed val = new Speed(360);
                UnitDescription desc = ApplicationModel.staticUnitSystem.Speed;
                return (int)val.GetValue(desc.Unit);
            }
        }

        public int Min_MaxSpeed
        {
            get
            {
                Speed val = new Speed(15);
                UnitDescription desc = ApplicationModel.staticUnitSystem.Speed;
                return (int)val.GetValue(desc.Unit);
            }
        }
        #endregion

        #region Frontal Area
        public int Max_FrontalArea
        {
            get
            {
                Area val = new Area(20);
                UnitDescription desc = ApplicationModel.staticUnitSystem.Area;
                return (int)val.GetValue(desc.Unit);
            }
        }

        public int Min_FrontalArea
        {
            get
            {
                Area val = new Area(5);
                UnitDescription desc = ApplicationModel.staticUnitSystem.Area;
                return (int)val.GetValue(desc.Unit);
            }
        }
        #endregion

        #region WheelRadius
        public double Max_WheelRadius
        {
            get
            {
                Distance val = new Distance(0.6, Unit.m);
                UnitDescription desc = ApplicationModel.staticUnitSystem.ShortDistance;
                return val.GetValue(desc.Unit);
            }
        }

        public double Min_WheelRadius
        {
            get
            {
                Distance val = new Distance(0.4, Unit.m);
                UnitDescription desc = ApplicationModel.staticUnitSystem.ShortDistance;
                return val.GetValue(desc.Unit);
            }
        }
        #endregion

        #region DragCoefficient
        public double Max_DragCoefficient
        {
            get
            {
                return 1.28;
            }
        }

        public double Min_DragCoefficient
        {
            get
            {
                return 0.14;
            }
        }
        #endregion

        #region RollingResistance1
        public double Max_RollingResistance1
        {
            get
            {
                return 0.0034;
            }
        }

        public double Min_RollingResistance1
        {
            get
            {
                return 0.0007;
            }
        }
        #endregion

        #region RollingResistance2
        public double Max_RollingResistance2
        {
            get
            {
                return 0.000025;
            }
        }

        public double Min_RollingResistance2
        {
            get
            {
                return 0;
            }
        }
        #endregion

        #region GearRatio
        public double Max_GearRatio
        {
            get
            {
                return 6;
            }
        }

        public double Min_GearRatio
        {
            get
            {
                return 2;
            }
        }
        #endregion

        #endregion

        /// <summary>
        /// Constructor for the parameters.
        /// These serve as default values for when the user is customizing the parameters
        /// </summary>
        public VehicleOptionParameters()
            :this(VehicleOptions.VehicleType.Passenger)
        {
        }

        public VehicleOptionParameters(VehicleOptions.VehicleType type)
        {
            double tmpLocomotiveMass,
                tmpNumCars,
                tmpCarMass,
                tmpFinalGearRatio,
                tmpMaxSpeed,
                tmpFrontalArea;
                
            if (type == VehicleOptions.VehicleType.Freight)
            {
                tmpLocomotiveMass = 190000;
                tmpNumCars = 50;
                tmpCarMass = 115000;
                tmpFinalGearRatio = 4.13;
                tmpMaxSpeed = 115;
                tmpFrontalArea = 15.9;

            }
            else if (type == VehicleOptions.VehicleType.Passenger)
            {
                tmpLocomotiveMass = 130000;
                tmpNumCars = 12;
                tmpCarMass = 60000;
                tmpFinalGearRatio = 2.85;
                tmpMaxSpeed = 165;
                tmpFrontalArea = 16.1;
            }
            else if (type == VehicleOptions.VehicleType.Switch)
            {
                tmpLocomotiveMass = 120000;
                tmpNumCars = 5;
                tmpCarMass = 27000;
                tmpFinalGearRatio = 4.13;
                tmpMaxSpeed = 115;
                tmpFrontalArea = 15.9;
            }
            else
            {
                tmpLocomotiveMass = 190000;
                tmpNumCars = 50;
                tmpCarMass = 115000;
                tmpFinalGearRatio = 4.13;
                tmpMaxSpeed = 115;
                tmpFrontalArea = 15.4;
            }

            GearRatio = tmpFinalGearRatio;

            MaxSpeed = (double)(new Speed(tmpMaxSpeed))
                                .GetValue(ApplicationModel.staticUnitSystem.Speed.Unit);

            LocomotiveMass = (int)(new Mass(tmpLocomotiveMass))
                                .GetValue(ApplicationModel.staticUnitSystem.Mass.Unit);

            CarMass = (int)(new Mass(tmpCarMass))
                                .GetValue(ApplicationModel.staticUnitSystem.Mass.Unit);

            NumCars = (int)tmpNumCars;

            FrontalArea = (int)(new Area(tmpFrontalArea))
                                .GetValue(ApplicationModel.staticUnitSystem.Area.Unit);

            WheelRadius = (double)(new Distance(0.5080, Unit.m))
                                .GetValue(ApplicationModel.staticUnitSystem.ShortDistance.Unit);

            DragCoefficient = 0.9388;

            RollingResistance1 = 0.0019;
            RollingResistance2 = 0.000015;
        }

        /// <summary>
        /// InputParameters method.
        /// Takes the configuration of this object and outputs a list of parameters to be placed into the _params file
        /// </summary>
        /// <returns>List of parameters to be put into the _params file</returns>
        public IEnumerable<Parameter> InputParameters()
        {
            List<Parameter> paramList = new List<Parameter>();

            paramList.Add(new Parameter(mdlParameterNames.veh_init_mass.ToString(), _locomotiveMass));
            paramList.Add(new Parameter(mdlParameterNames.veh_init_mass_cars.ToString(), _carMass));
            paramList.Add(new Parameter(mdlParameterNames.veh_init_num_cars.ToString(), _numCars));
            paramList.Add(new Parameter(mdlParameterNames.veh_init_frontal_area.ToString(), _frontalArea));
            paramList.Add(new Parameter(mdlParameterNames.veh_init_coeff_drag.ToString(), _dragCoefficient));
            paramList.Add(new Parameter(mdlParameterNames.wh_init_coeff_roll1.ToString(), _rollingResistance1));
            paramList.Add(new Parameter(mdlParameterNames.wh_init_coeff_roll2.ToString(), _rollingResistance2));
            paramList.Add(new Parameter(mdlParameterNames.wh_init_radius.ToString(), (new Distance(_wheelRadius, Unit.Km)).GetValue(Unit.m)));
            paramList.Add(new Parameter(mdlParameterNames.top_speed.ToString(), _maxSpeed));
            paramList.Add(new Parameter(mdlParameterNames.fd_init_ratio.ToString(), _gearRatio));
            
            return paramList;
        }
    }
}
