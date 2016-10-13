using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using REMForm.Helpers;

namespace REMForm.ViewModels.Options
{
    /// <summary>
    /// Enum for the train topology
    /// </summary>
    public enum TrainTopology
    {
        EngineElectric,
        EngineHybrid,
        Electric,
        FuelCell
    }

    /// <summary>
    /// Enum for the energy storage system
    /// </summary>
    public enum EnergyStorageSystem
    {
        Battery,
        Flywheel,
        Ultracapacitive,
        None
    }

    /// <summary>
    /// Engine fuel type enum
    /// </summary>
    public enum EngineFuelType
    {
        Diesel,
        Gas,
        CNG,
        LNG
    }

    /// <summary>
    /// main model for the train options (sub model of the parametermodel)
    /// As of right now this doesn't have any impact on the simulation.
    /// Going forward this will determine what model is being used for the simulation
    /// </summary>
    public class TrainOptions : NotifyPropertyChangedHandler
    {
        //Private variables
        private TrainTopology _topology;
        private EnergyStorageSystem _storageSystem;
        private EngineFuelType _type;

        /// <summary>
        /// The type of train topology to use
        /// </summary>
        public TrainTopology Topology {
            get { return _topology; }
            set
            {
                //Funtimes with the UI
                //Electric or EngineElectric cannot have an ESS set.
                if (value == TrainTopology.Electric || value == TrainTopology.EngineElectric)
                {
                    //So we set it to "none"
                    StorageSystem = EnergyStorageSystem.None;
                }
                else
                {
                    //However, we don't want "none" to be a UI option
                    // so if it's a Hybrid-type, then we need to default it to something
                    if (StorageSystem == EnergyStorageSystem.None)
                    {
                        StorageSystem = EnergyStorageSystem.Battery;
                    }
                }
                _topology = value;
                ApplicationModel.staticTopology = _topology;
                NotifyPropertyChanged("Topology");
            }
        }

        /// <summary>
        /// The type of energy storage system to use
        /// </summary>
        public EnergyStorageSystem StorageSystem
        {
            get { return _storageSystem; }
            set
            {
                _storageSystem = value;
                ApplicationModel.staticESS = _storageSystem;
                NotifyPropertyChanged("StorageSystem");

            }
        }

        public override void NotifyAllPropertiesChanged()
        {
            NotifyPropertyChanged(string.Empty);
        }

        /// <summary>
        /// Constructor for train options. This acts as the default values for the train options
        /// </summary>
        public TrainOptions()
        {
            Topology = TrainTopology.EngineElectric;
            StorageSystem = EnergyStorageSystem.None;
        }
    }
}
