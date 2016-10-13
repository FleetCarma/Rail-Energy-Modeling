using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using REMForm.Helpers;

namespace REMForm.ViewModels.Simulation
{
    /// <summary>
    /// A single instance of the output from a simulation.
    /// Uniquely identified by the time value, and is used to display graphs of vehicle speed, engine speed, and fuel rate
    /// </summary>
    public class SimResult
    {
        /// <summary>
        /// The time value of this point in the simulation
        /// </summary>
        public double Time { get; set; }

        /// <summary>
        /// The vehicle speed at this point in the simulation
        /// </summary>
        public double Speed { get; set; }

        /// <summary>
        /// The engine speed of this point in the simulation
        /// </summary>
        public double EngineSpeed { get; set; }

        /// <summary>
        /// The fuel rate of this point in the simulation
        /// </summary>
        public double FuelRate { get; set; }

        /// <summary>
        /// The GHG emissions of this point in the simulation
        /// </summary>
        public double GHGEmissions { get; set; }

        /// <summary>
        /// The Engine power at this point in the simulation
        /// </summary>
        public double EnginePower { get; set; }

        /// <summary>
        /// The APU fuel rate of this point in the simulation
        /// </summary>
        public double ApuFuelRate { get; set; }

        /// <summary>
        /// The APU pwoer of this point in the simulation
        /// </summary>
        public double ApuPower { get; set; }

        /// <summary>
        /// The Auxiliary Fuel consumption at this point in the simulation
        /// </summary>
        public double AuxFuelConsumption { get; set; }

        /// <summary>
        /// If AESS is on or off
        /// </summary>
        public double AESSOnOffState { get; set; }

        /// <summary>
        /// ESS Power at this point in the simulation
        /// </summary>
        public double ESSPower { get; set; }

        /// <summary>
        /// Grid Energy at this point in the simulation
        /// </summary>
        public double GridEnergy { get; set; }

        /// <summary>
        /// Traction power at this point in the simulation
        /// </summary>
        public double TractivePower { get; set; }

        /// <summary>
        /// Total fuel used at this point in the simulation
        /// </summary>
        public double TotalFuelUse { get; set; }

        /// <summary>
        /// Total fuel used at this point in the simulation
        /// </summary>
        public double TotalGHGEmissions { get; set; }

        /// <summary>
        /// Total distance travelled at this point in the simulation (estimated)
        /// </summary>
        public double DistanceTravelled { get; set; }

        public double TotalESSPower { get; set; }

    }
}
