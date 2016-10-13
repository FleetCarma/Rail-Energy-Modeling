using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using REMForm.Helpers;
using TrainSimulatorCLI;

namespace REMForm.ViewModels.Options
{
    /// <summary>
    /// Enum for the types of auxiliary engines available
    /// </summary>
    public enum AuxEngineType
    {
        HEP,
        APU,
        None
    }
    /// <summary>
    /// Main model for the Auxiliary engine options
    /// </summary>
    public class AuxEngineOptions : NotifyPropertyChangedHandler, IInputParameter
    {

        //Private variables, stored in canonical units
        private AuxEngineType _type;
        public double _mechanicalAcc { get; private set; }
        public double _electricalAcc { get; private set; }
        public double _load { get; private set; }
        public double _efficiency { get; private set; }
        public bool _aess { get; private set; }

        /// <summary>
        /// The type of auxiliary engine being used
        /// </summary>
        public AuxEngineType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                NotifyPropertyChanged("Type");
            }
        }

        public double MechanicalAccessories
        {
            get
            {
                return _mechanicalAcc;
            }
            set
            {
                _mechanicalAcc = value;
                NotifyPropertyChanged("MechanicalAccessories");
            }
        }

        public double ElectricalAccessories
        {
            get
            {
                return _electricalAcc;
            }
            set
            {
                _electricalAcc = value;
                NotifyPropertyChanged("ElectricalAccessories");
            }
        }

        public double Load
        {
            get
            {

                return _load;
            }
            set
            {
                _load = value;
                NotifyPropertyChanged("Load");
            }
        }

        public double Efficiency
        {
            get
            {
                return _efficiency;
            }
            set
            {
                _efficiency = value;
                NotifyPropertyChanged("Efficiency");
            }
        }

        public bool AESS
        {
            get
            {
                if (Type == AuxEngineType.None)
                {
                    return false;
                }
                return _aess ;
            }
            set
            {
                _aess = value;
                NotifyPropertyChanged("AESS");
            }

        }

        #region Min and Max values

        #region MechanicalAccessories

        public double Max_MechanicalAccessories
        {
            get
            {
                return 11.000;
            }
        }
        public double Min_MechanicalAccessories
        {
            get
            {
                if (ApplicationModel.staticTopology == TrainTopology.EngineElectric)
                {
                    return 0.185;
                }
                else if (ApplicationModel.staticTopology == TrainTopology.EngineHybrid)
                {
                    return 0.250;
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion

        #region ElectricalAccessories

        public double Max_ElectricalAccessories
        {
            get
            {
                return 5;
            }
        }
        public double Min_ElectricalAccessories
        {
            get
            {
                return 0;
            }
        }

        #endregion

        #region Load

        public double Max_Load
        {
            get
            {
                return 4000;
            }
        }
        public double Min_Load
        {
            get
            {
                return 0;
            }
        }

        #endregion

        #region Efficiency

        public double Max_Efficiency
        {
            get
            {
                return 0.67;
            }
        }

        public double Min_Efficiency
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
                return 0.93;
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

        #endregion

        public override void NotifyAllPropertiesChanged()
        {
            NotifyPropertyChanged(string.Empty);
        }

        /// <summary>
        /// Constructor for auxiliary engine model
        /// These act as default values for when the user is customizing the parameters
        /// </summary>
        public AuxEngineOptions()
        {
            Type = AuxEngineType.HEP;
            AESS = false;
            Load = 240;
            Efficiency = 0.35;
            MechanicalAccessories = 0.800;
            ElectricalAccessories = 1.500;
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
            
            paramList.Add(new Parameter(mdlParameterNames.aux_eff.ToString(), Efficiency));
            paramList.Add(new Parameter(mdlParameterNames.AUXPOWER.ToString(), Load));
        
            paramList.Add(new Parameter(mdlParameterNames.accelec_init_pwr.ToString(), ElectricalAccessories*1000));
            paramList.Add(new Parameter(mdlParameterNames.accmech_init_pwr.ToString(), MechanicalAccessories*1000));
            return paramList;
        }
        #endregion

    }
}
