using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using REMForm.Helpers;
using TrainSimulatorCLI;
using UnitSystem;

namespace REMForm.ViewModels.Options
{
    /// <summary>
    /// Main model for the powertrain data
    /// </summary>
    public class PowerTrainOptions : NotifyPropertyChangedHandler, IInputParameter
    {
        //private variables, stored in canonical units
        public double _enginePower { get; private set; } //Top: Engine Electric/Engine Hybrid
        public double _meanEngineEfficiency { get; private set; }//Top: Engine Electric/Engine Hybrid
        public double _fuelCellPower { get; private set; }//Top: Fuel Cell
        public double _meanFuelCellEfficiency { get; private set; }//Top: Fuel Cell
        public double _generatorEfficiency { get; private set; } //?
        public double _batteryPower { get; private set; } //ESS: Battery
        public double _batteryEnergy { get; private set; } //ESS: Battery
        public double _flywheelPower { get; private set; } //ESS: Flywheel
        public double _flywheelEnergy { get; private set; } //ESS: Flywheel
        public double _ultracapPower { get; private set; } //ESS: Ultracap
        public double _ultracapCapacitance { get; private set; } //ESS: Ultracap
        public double _motorPower { get; private set; } //?
        public double _motorEfficiency { get; private set; } //?
        public double _finalDriveEfficiency { get; private set; } //?

        public double EnginePower
        {
            get
            {
                MechanicalPower val = new MechanicalPower(_enginePower);
                UnitDescription desc = ApplicationModel.staticUnitSystem.MechanicalPower;
                return val.GetValue(desc.Unit);
            }
            set
            {
                //Get the current unit
                UnitDescription desc = ApplicationModel.staticUnitSystem.MechanicalPower;
                //Create the Quantity
                MechanicalPower val = new MechanicalPower(value, desc.Unit);

                //store as canonical
                _enginePower = val.CanonicalValue;

                NotifyPropertyChanged("EnginePower");
            }
        }

        public double MeanEngineEfficiency
        {
            get
            {
                return _meanEngineEfficiency;
            }
            set
            {
                _meanEngineEfficiency = value;
                NotifyPropertyChanged("MeanEngineEfficiency");
            }
        }

        public double FuelCellPower
        {
            get
            {
                return _fuelCellPower;
            }
            set
            {
                _fuelCellPower = value;
                NotifyPropertyChanged("FuelCellPower");
            }
        }

        public double MeanFuelCellEfficiency
        {
            get
            {
                return _meanFuelCellEfficiency;
            }
            set
            {
                _meanFuelCellEfficiency = value;
                NotifyPropertyChanged("MeanFuelCellEfficiency");
            }
        }

        public double GeneratorEfficiency
        {
            get
            {
                return _generatorEfficiency;
            }
            set
            {
                _generatorEfficiency = value;
                NotifyPropertyChanged("GeneratorEfficiency");
            }
        }

        public double BatteryPower
        {
            get
            {
                return _batteryPower;
            }
            set
            {
                _batteryPower = value;
                NotifyPropertyChanged("BatteryPower");
            }
        }

        public double BatteryEnergy
        {
            get
            {
                return _batteryEnergy;
            }
            set
            {
                _batteryEnergy = value;
                NotifyPropertyChanged("BatteryEnergy");
            }
        }

        public double FlywheelPower
        {
            get
            {
                return _flywheelPower;
            }
            set
            {
                _flywheelPower = value;
                NotifyPropertyChanged("FlywheelPower");
            }
        }

        public double FlywheelEnergy
        {
            get
            {
                return _flywheelEnergy;
            }
            set
            {
                _flywheelEnergy = value;
                NotifyPropertyChanged("FlywheelEnergy");
            }
        }

        public double UltracapPower
        {
            get
            {
                return _ultracapPower;
            }
            set
            {
                _ultracapPower = value;
                NotifyPropertyChanged("UltracapPower");
            }
        }

        public double UltracapCapacitance
        {
            get
            {
                return _ultracapCapacitance;
            }
            set
            {
                _ultracapCapacitance = value;
                NotifyPropertyChanged("UltracapCapacitance");
            }
        }

        public double MotorPower
        {
            get
            {
                return _motorPower;
            }
            set
            {
                _motorPower = value;
                NotifyPropertyChanged("MotorPower");
            }
        }

        public double MotorEfficiency
        {
            get
            {
                return _motorEfficiency;
            }
            set
            {
                _motorEfficiency = value;
                NotifyPropertyChanged("MotorEfficiency");
            }
        }

        public double FinalDriveEfficiency
        {
            get
            {
                return _finalDriveEfficiency;
            }
            set
            {
                _finalDriveEfficiency = value;
                NotifyPropertyChanged("FinalDriveEfficiency");
            }
        }

        public double CanonicalPrimePower
        {
            get
            {
                if (ApplicationModel.staticTopology == TrainTopology.Electric)
                {
                    return _motorPower;
                }
                else if (ApplicationModel.staticTopology == TrainTopology.FuelCell)
                {
                    return _fuelCellPower;
                }
                else
                {
                    return _enginePower;
                }
            }
        }

        public double CanonicalESSPower
        {
            get
            {
                if (ApplicationModel.staticTopology == TrainTopology.FuelCell ||
                    ApplicationModel.staticTopology == TrainTopology.EngineHybrid)
                {
                    if (ApplicationModel.staticESS == EnergyStorageSystem.Battery)
                    {
                        return _batteryPower;
                    }
                    else if (ApplicationModel.staticESS == EnergyStorageSystem.Flywheel)
                    {
                        return _flywheelPower;
                    }
                    else if (ApplicationModel.staticESS == EnergyStorageSystem.Ultracapacitive)
                    {
                        return _ultracapPower;
                    }
                    return 0;
                }
                else
                {
                    return 0;
                }
            }
        }

        public override void NotifyAllPropertiesChanged()
        {
            NotifyPropertyChanged(string.Empty);
        }

        /// <summary>
        /// Constructor for powertrain options
        /// These values are considered the defaults, and are the baseline values used when the user wishes to customize the values
        /// </summary>
        public PowerTrainOptions()
        {

            EnginePower             = (new MechanicalPower(3000))
                                        .GetValue(ApplicationModel.staticUnitSystem.MechanicalPower.Unit) ;

            MeanEngineEfficiency    = 0.47;
            FuelCellPower           = 1500;
            MeanFuelCellEfficiency  = 0.60;
            GeneratorEfficiency     = 0.86;
            BatteryPower            = 1000;
            BatteryEnergy           = 200;
            FlywheelPower           = 1000;
            FlywheelEnergy          = 37;
            UltracapPower           = 1000;
            UltracapCapacitance     = 500;
            MotorPower              = 2000;
            MotorEfficiency         = 0.95;
            FinalDriveEfficiency    = 0.9;
        }

        #region Min and Max values

        #region EnginePower

        public double Max_EnginePower
        {
            get
            {
                return 11000;
            }
        }
        public double Min_EnginePower
        {
            get
            {
                if (ApplicationModel.staticTopology == TrainTopology.EngineElectric)
                {
                    return 185;
                }
                else if (ApplicationModel.staticTopology == TrainTopology.EngineHybrid)
                {
                    return 250;
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion

        #region MeanEngineEfficiency

        public double Max_MeanEngineEfficiency
        {
            get
            {
                return 0.55;
            }
        }
        public double Min_MeanEngineEfficiency
        {
            get
            {
                return 0.1;
            }
        }

        #endregion

        #region FuelCellPower

        public double Max_FuelCellPower
        {
            get
            {
                return 11000; //PRIME POWER FUELCELL
            }
        }
        public double Min_FuelCellPower
        {
            get
            {
                return 250;
            }
        }

        #endregion

        #region MeanFuelCellEfficiency

        public double Max_MeanFuelCellEfficiency
        {
            get
            {
                return 0.70;
            }
        }

        public double Min_MeanFuelCellEfficiency
        {
            get
            {
                return 0.1;
            }
        }

        #endregion

        #region GeneratorEfficiency

        public double Max_GeneratorEfficiency
        {
            get
            {
                return 0.97; //gc_eff
            }
        }

        public double Min_GeneratorEfficiency
        {
            get
            {
                return 0.3;
            }
        }

        #endregion

        #region BatteryPower

        public double Max_BatteryPower
        {
            get
            {
                return 15000;
            }
        }

        public double Min_BatteryPower
        {
            get
            {
                return 200;
            }
        }

        #endregion

        #region BatteryEnergy

        public double Max_BatteryEnergy
        {
            get
            {
                return Int32.MaxValue;
            }
        }

        public double Min_BatteryEnergy
        {
            get
            {
                return 35;
            }
        }

        #endregion

        #region FlywheelPower

        public double Max_FlywheelPower
        {
            get
            {
                return 15000;
            }
        }

        public double Min_FlywheelPower
        {
            get
            {
                return 200;
            }
        }

        #endregion

        #region FlywheelEnergy

        public double Max_FlywheelEnergy
        {
            get
            {
                return Int32.MaxValue;
            }
        }

        public double Min_FlywheelEnergy
        {
            get
            {
                return 10;
            }
        }

        #endregion

        #region UltracapPower

        public double Max_UltracapPower
        {
            get
            {
                return 15000;
            }
        }

        public double Min_UltracapPower
        {
            get
            {
                return 200;
            }
        }

        #endregion

        #region UltracapCapacitance

        public double Max_UltracapCapacitance
        {
            get
            {
                return Int32.MaxValue;
            }
        }

        public double Min_UltracapCapacitance
        {
            get
            {
                return 90;
            }
        }

        #endregion

        #region MotorPower

        public double Max_MotorPower
        {
            get
            {
                return 11000; //SOMETHING.
            }
        }

        public double Min_MotorPower
        {
            get
            {
                return 185;
            }
        }

        #endregion

        #region MotorEfficiency

        public double Max_MotorEfficiency
        {
            get
            {
                return 0.97;
            }
        }

        public double Min_MotorEfficiency
        {
            get
            {
                return 0.3;
            }
        }

        #endregion

        #region FinalDriveEfficiency

        public double Max_FinalDriveEfficiency
        {
            get
            {
                return 0.99;
            }
        }

        public double Min_FinalDriveEfficiency
        {
            get
            {
                return 0.8;
            }
        }

        #endregion

        #endregion


        #region IInputParameter method
        /// <summary>
        /// InputParameters method.
        /// This method only returns the parameters for the collection of options,
        /// and not Max and Idle fuel burn rate.
        /// Max and Idle fuel burn rate are required in logic elsewhere
        /// </summary>
        /// <returns>List of parameters to be put into the _params file</returns>
        public IEnumerable<Parameter> InputParameters()
        {
            List<Parameter> paramList = new List<Parameter>();
           
            paramList.Add(new Parameter(mdlParameterNames.eng_eff.ToString(), _meanEngineEfficiency));
            paramList.Add(new Parameter(mdlParameterNames.gc_eff.ToString(), _generatorEfficiency));
            paramList.Add(new Parameter(mdlParameterNames.mc_eff.ToString(), _motorEfficiency));

            double essEnergy = 0;
            double essPower = 0;
            double essCapacitance = 0;
            if(ApplicationModel.staticTopology == TrainTopology.EngineHybrid ||
                ApplicationModel.staticTopology == TrainTopology.FuelCell)
            {
                if(ApplicationModel.staticESS == EnergyStorageSystem.Battery)
                {
                    essEnergy = _batteryEnergy;
                    essPower = _batteryPower;
                }
                else if (ApplicationModel.staticESS == EnergyStorageSystem.Flywheel)
                {
                    essEnergy = _flywheelEnergy;
                    essPower = _flywheelPower;
                }
                else if (ApplicationModel.staticESS == EnergyStorageSystem.Ultracapacitive)
                {
                    essCapacitance = _ultracapCapacitance;
                    essPower = _ultracapPower;
                }
            }

            paramList.Add(new Parameter(mdlParameterNames.PRIMEPOWER.ToString(), CanonicalPrimePower));
            paramList.Add(new Parameter(mdlParameterNames.ess_energy.ToString(), essEnergy));
            paramList.Add(new Parameter(mdlParameterNames.ESSPOWER.ToString(), essPower));
            paramList.Add(new Parameter(mdlParameterNames.ess_capacitance.ToString(), essCapacitance));
            paramList.Add(new Parameter(mdlParameterNames.fd_init_eff.ToString(), _finalDriveEfficiency));

            return paramList;
        }
        #endregion
    }
}
