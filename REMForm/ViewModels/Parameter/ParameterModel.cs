using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using REMForm.ViewModels.Options;
using REMForm.Helpers;
using UnitSystem;

namespace REMForm.ViewModels
{
    /// <summary>
    /// Model representing all the configurable parameters that will be put into the simulations _params file
    /// </summary>
    public class ParameterModel : NotifyPropertyChangedHandler, IInputParameter
    {
        //private variables
        private VehicleOptions _vehicleOption { get; set; }
        private AuxEngineOptions _auxEngineOption { get; set; }
        private PowerTrainOptions _powertrainOption { get; set; }
        private TrainOptions _trainOption { get; set; }

        /// <summary>
        /// Vehicle options model
        /// This contains configurations on the vehicle being simulated
        /// </summary>
        public VehicleOptions VehicleOption
        {
            get { return _vehicleOption; }
            set
            {
                _vehicleOption = value;
                NotifyPropertyChanged("VehicleOption");
            }
        }

        /// <summary>
        /// Auxiliary Engine options model
        /// This contains configurations on what type of auxiliary engine is used,
        /// as well as the power and burn rates of the auxiliary engine
        /// </summary>
        public AuxEngineOptions AuxEngineOption
        {
            get { return _auxEngineOption; }
            set
            {
                _auxEngineOption = value;
                NotifyPropertyChanged("AuxEngineOption");
            }
        }

        /// <summary>
        /// Powertrain option model
        /// This contains configurations on the type of engine fuel, the prime mover,
        /// and max and idle fuel burn rates
        /// </summary>
        public PowerTrainOptions PowertrainOption
        {
            get { return _powertrainOption; }
            set
            {
                _powertrainOption = value;
                NotifyPropertyChanged("PowertrainOption");
            }
        }

        /// <summary>
        /// Train option model
        /// This contains configurations on the train topology
        /// </summary>
        public TrainOptions TrainOption
        {
            get { return _trainOption; }
            set
            {
                _trainOption = value;
                NotifyPropertyChanged("TrainOption");
            }
        }

        public override void NotifyAllPropertiesChanged()
        {
            _vehicleOption.NotifyAllPropertiesChanged();
            _auxEngineOption.NotifyAllPropertiesChanged();
            _powertrainOption.NotifyAllPropertiesChanged();
            _trainOption.NotifyAllPropertiesChanged();
            NotifyPropertyChanged(string.Empty);
        }

        /// <summary>
        /// Constructor for the parameter model
        /// Simply calls the constructors of its contained models
        /// </summary>
        public ParameterModel()
        {
            VehicleOption = new VehicleOptions();
            AuxEngineOption = new AuxEngineOptions();
            PowertrainOption = new PowerTrainOptions();
            TrainOption = new TrainOptions();
        }

        #region IInputParameter method
        /// <summary>
        /// InputParameters method.
        /// This simply calls the Inputparameters methods of each submodel (TrainOptions exempt)
        /// and combines them together. The result will be put into the _params file and fed into the simulation
        /// </summary>
        /// <returns>List of every parameter configured by the Vehicle, Auxiliary Engine, and Powertrain models</returns>
        public IEnumerable<Parameter> InputParameters()
        {
            List<Parameter> paramList = new List<Parameter>();
            paramList.AddRange(VehicleOption.InputParameters());
            paramList.AddRange(AuxEngineOption.InputParameters());
            paramList.AddRange(PowertrainOption.InputParameters());

            return paramList;
        }
        #endregion
    }
}
