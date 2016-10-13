using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using REMForm.Helpers;
using UnitSystem;
using REMForm.ViewModels.Options;

namespace REMForm.ViewModels
{
    /// <summary>
    /// Overall model for the entire application
    /// </summary>
    public class ApplicationModel : NotifyPropertyChangedHandler
    {
        //Private variables
        private DriveCycleModel _driveCycleModel;
        private ParameterModel _paramConfigModel;
        private SimulationModel _simModel;
        private IUnitSystem _unitSystem;

        //We need some static variables, as these are needed throughout most of the UImodels
        public static IUnitSystem staticUnitSystem = new MetricSystem();

        public static TrainTopology staticTopology = TrainTopology.EngineElectric;
        public static EnergyStorageSystem staticESS = EnergyStorageSystem.None;
        public static EngineFuelType staticFuelType = EngineFuelType.Diesel;

        /// <summary>
        /// Model for the drive cycle tab
        /// </summary>
        public DriveCycleModel DriveCycleModel
        {
            get { return _driveCycleModel; }
            set
            {
                _driveCycleModel = value;
                NotifyPropertyChanged("DriveCycleModel");
            }
        }

        /// <summary>
        /// Model for the parameters tab
        /// </summary>
        public ParameterModel ParamConfigModel
        {
            get { return _paramConfigModel; }
            set
            {
                _paramConfigModel = value;
                NotifyPropertyChanged("ParamConfigModel");
            }
        }

        /// <summary>
        /// Model for the simulation tab
        /// </summary>
        public SimulationModel SimulationModel
        {
            get { return _simModel; }
            set
            {
                _simModel = value;
                NotifyPropertyChanged("SimulationModel");
            }
        }

        /// <summary>
        /// Model for the simulation tab
        /// </summary>
        public IUnitSystem UnitSystem
        {
            get { return _unitSystem; }
            set
            {
                staticUnitSystem = value;
                _unitSystem = value;
                NotifyAllPropertiesChanged();
            }
        }

        /// <summary>
        /// Model for the simulation tab
        /// </summary>
        public EngineFuelType FuelType
        {
            get { return staticFuelType; }
            set
            {
                staticFuelType = value;
                if (SimulationModel.Results.Count > 0)
                {
                    SimulationModel.NotifyAllPropertiesChanged();
                }
            }
        }

        public override void NotifyAllPropertiesChanged()
        {
            _driveCycleModel.NotifyAllPropertiesChanged();
            _paramConfigModel.NotifyAllPropertiesChanged();
            _simModel.NotifyAllPropertiesChanged();
            NotifyPropertyChanged(string.Empty);
        }

        /// <summary>
        /// The constructor for the application model simply calls the constructors of its child models
        /// </summary>
        public ApplicationModel()
        {
            DriveCycleModel = new DriveCycleModel();
            ParamConfigModel = new ParameterModel();
            SimulationModel = new SimulationModel();
            UnitSystem = new MetricSystem();
        }
    }
}
