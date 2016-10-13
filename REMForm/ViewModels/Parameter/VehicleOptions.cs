using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using REMForm.Helpers;
using TrainSimulatorCLI;

namespace REMForm.ViewModels.Options
{
    /// <summary>
    /// main model for the Vehicle options (sub model of the parametermodel)
    /// </summary>
    public class VehicleOptions : NotifyPropertyChangedHandler, IInputParameter
    {
        //Private variables
        private VehicleOptionParameters _parameters;
        private bool _aess;

        /// <summary>
        /// The parameters to use for the simulation
        /// </summary>
        public VehicleOptionParameters Parameters
        {
            get { return _parameters; }
            set
            {
                _parameters = value;
                NotifyPropertyChanged("Parameters");
            }
        }

        public override void NotifyAllPropertiesChanged()
        {
            _parameters.NotifyAllPropertiesChanged();
            NotifyPropertyChanged(string.Empty);
        }

        /// <summary>
        /// Enum for the type of vehicle
        /// </summary>
        public enum VehicleType
        {
            Passenger,
            Freight,
            Switch
        }

        /// <summary>
        /// Constructor for the vehicle options
        /// These values represent the default values of the model
        /// </summary>
        public VehicleOptions()
        {
            Parameters = new VehicleOptionParameters();
        }

        public VehicleOptions(VehicleType type)
        {
            Parameters = new VehicleOptionParameters(type);
        }

        #region IInputParameter method
        /// <summary>
        /// InputParameters method.
        /// Takes the configuration of this object and outputs a list of parameters to be placed into the _params file
        /// </summary>
        /// <returns>List of parameters to be put into the _params file</returns>
        public IEnumerable<Parameter> InputParameters()
        {
            List<Parameter> paramList = new List<Parameter>();
            VehicleOptionParameters optionParams = Parameters;
            paramList.AddRange(optionParams.InputParameters());
            return paramList;
        }
        #endregion
    }
}
