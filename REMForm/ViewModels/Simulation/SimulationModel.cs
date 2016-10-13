using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using REMForm.Helpers;
using REMForm.ViewModels.Simulation;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UnitSystem;

namespace REMForm.ViewModels
{
    /// <summary>
    /// Model representing what is displayed on the simulations tab
    /// </summary>
    public class SimulationModel : NotifyPropertyChangedHandler
    {
        //Chart image file name constants
        private const string VEHICLESPEED_FILENAME = "_vehiclespeed.png";
        private const string FUELBURNRATE_FILENAME = "_fuelburnrate.png";
        private const string ENGINESPEED_FILENAME = "_enginespeed.png";
        private const string GHGEMISSIONS_FILENAME = "_ghgemissions.png";
        private const string LOCOMOTIVEPOWER_FILENAME = "_locomotivepower.png";
        private const string AUXFUELCONSUMPTION_FILENAME = "_auxfuelconsumption.png";
        private const string AESSSTATE_FILENAME = "_aessstate.png";


        //Private variables
        #region Simulation Result variables
        private List<SimResult> _results;
        private double? _totalFuelUse;
        private double? _totalApuFuelUse;
        private double? _totalGridEnergyUse;
        private double? _totalGHGEmissions;
        private double? _distanceTravelled;
        private double? _totalESSPower;
        #endregion

        private double _co2Emissions;//Is this used anymore?

        private double _totalWeight;
        private double _loadMovedPerKgFuel;
        private SimChartType _chartType;
        private string _chartImageLoc;
        private string _simOutputLoc;
        private bool _graphNeedsUpdate;


        #region Simulation Result variables
        /// <summary>
        /// List of the simulation results
        /// </summary>
        public List<SimResult> Results
        {
            get { return _results; }
            set
            {
                _results = value;
                ClearAllSummaries();
                NotifyPropertyChanged("Results");
                NotifyPropertyChanged("TotalFuelUseMass");
                NotifyPropertyChanged("TotalESSPower");
                NotifyPropertyChanged("TotalEnergyConsumption");
                NotifyPropertyChanged("DistanceTravelled");
                NotifyPropertyChanged("CalculatedFuelRate");
                NotifyPropertyChanged("GHGEmissions");
                NotifyPropertyChanged("TotalAPUFuelUse");

                NotifyPropertyChanged("ActualFuelRate");
                NotifyPropertyChanged("ESSPowerRate");
                NotifyPropertyChanged("EquivalentFuelRate");
            }
        }

        /// <summary>
        /// The total mass of fuel used, reported by the simulation
        /// </summary>
        /// <remarks>_totalFuelUse is in kg</remarks>
        public double TotalFuelUseMass
        {
            get
            {
                if (!_totalFuelUse.HasValue)
                {
                    if (Results == null || Results.Count == 0)
                    {
                        return 0;
                    }
                    _totalFuelUse = Math.Round(
                        Results.Select(r => r.TotalFuelUse).Max(),
                        Constants.NumDecimals
                    );
                }

                return (new Mass(_totalFuelUse.Value)).GetValue(ApplicationModel.staticUnitSystem.Mass.Unit);
            }
        }


        /// <summary>
        /// The total distance travelled at the end of the simulation
        /// This is calculated through the simulation speed and timestamps, so it is not necessarily accurate
        /// But it's accurate enough.
        /// </summary>
        public double DistanceTravelled
        {
            get
            {
                if (!_distanceTravelled.HasValue)
                {
                    if (Results == null || Results.Count == 0)
                    {
                        return 0;
                    }
                    _distanceTravelled = Math.Round(
                        Results.Select(r => r.DistanceTravelled).Max(),
                        Constants.NumDecimals
                    );
                }

                return Math.Round((new Distance(_distanceTravelled.Value, Unit.m)).GetValue(ApplicationModel.staticUnitSystem.Distance.Unit),2);
            }
        }

        public double TotalESSPower
        {
            get
            {
                if (!_totalESSPower.HasValue)
                {
                    if (Results == null || Results.Count == 0)
                    {
                        return 0;
                    }
                    _totalESSPower = Math.Round(
                        Results.Select(r => r.TotalESSPower).Last(),
                        Constants.NumDecimals
                    );
                }
                return _totalESSPower.Value;
            }
        }

        public double TotalAPUFuelUse
        {
            get
            {
                if (!_totalApuFuelUse.HasValue)
                {
                    if (Results == null || Results.Count == 0)
                    {
                        return 0;
                    }
                    _totalApuFuelUse = Math.Round(
                        Results.Select(r => r.AuxFuelConsumption).Max(),
                        Constants.NumDecimals
                    );
                }
                NotifyPropertyChanged("TotalAPUFuelUseVolume");
                return (new Mass(_totalApuFuelUse.Value)).GetValue(ApplicationModel.staticUnitSystem.Mass.Unit);
            }
        }

        public double TotalAPUFuelUseVolume
        {
            get
            {
                double litres = _totalApuFuelUse.Value / 0.832;
                return Math.Round((new Volume(litres, Unit.L)).GetValue(ApplicationModel.staticUnitSystem.Volume.Unit),2);
            }
        }

        public double TotalEnergyConsumption
        {
            get
            {
                if (!_totalGridEnergyUse.HasValue)
                {
                    if (Results == null || Results.Count == 0)
                    {
                        return 0;
                    }
                    _totalGridEnergyUse = Math.Round(
                        Results.Select(r => r.GridEnergy).Last(),
                        Constants.NumDecimals
                    );
                }

                return Math.Round((_totalGridEnergyUse.Value/1000) + (TotalESSPower / 3600000),2);
            }
        }

        /// <summary>
        /// Actual Fuel rate
        /// </summary>
        public double ActualFuelRate
        {
            get
            {
                bool imperial = ApplicationModel.staticUnitSystem.FuelConsumption.Unit == Unit.MiPerGal;
                if (ApplicationModel.staticTopology == Options.TrainTopology.FuelCell ||
                    ApplicationModel.staticFuelType == Options.EngineFuelType.CNG)
                {
                    //the value we'll have is in m^3
                    _actualFuelLabel = imperial ? "ft^3" : "m^3";
                }
                else
                {
                    //the value we'll have is in L
                    _actualFuelLabel = imperial ? "gal" : "L";
                }
                NotifyPropertyChanged("ActualFuelLabel");

                if (DistanceTravelled == 0)
                {
                    return 0;
                }
                if (_totalFuelUse == null)
                {
                    double var = TotalFuelUseMass; //I hate doing this.
                }
                double val = CalculateActualEngineFuelUse(ApplicationModel.staticFuelType,
                    _totalFuelUse.Value);

                //Gotta avoid div0
                if (val == 0)
                {
                    return 0;
                }

                if (ApplicationModel.staticTopology == Options.TrainTopology.FuelCell ||
                    ApplicationModel.staticFuelType == Options.EngineFuelType.CNG)
                {
                    //the value we have is in m^3
                    val = imperial ? 35.3147 * val : val;
                }
                else
                {
                    //the value we have is in L
                    val = imperial ? val / 3.78541 : val;
                }


                return Math.Round(val,2);
            }
        }


        private string _actualFuelLabel;
        /// <summary>
        /// Label for the actual fuel units
        /// </summary>
        public string ActualFuelLabel
        {
            get
            {
                return _actualFuelLabel;
            }
        }

        /// <summary>
        /// Equivalent Fuel rate in L/km
        /// </summary>
        public double EquivalentFuelRate
        {
            get
            {
                bool imperial = ApplicationModel.staticUnitSystem.FuelConsumption.Unit == Unit.MiPerGal;
                _equivRateLabel = imperial ? "MPG eq" : "L/km eq";
                NotifyPropertyChanged("EquivFuelRateLabel");

                if (DistanceTravelled == 0)
                {
                    return 0;
                }
                double lpkm = CalculateEqEngineFuelRate(ApplicationModel.staticFuelType,
                    (new Distance(DistanceTravelled, ApplicationModel.staticUnitSystem.Distance.Unit)).GetValue(Unit.Km),
                    (new Mass(TotalFuelUseMass, ApplicationModel.staticUnitSystem.Mass.Unit)).GetValue(Unit.Kg),
                    TotalESSPower,
                    (new Mass(TotalAPUFuelUse, ApplicationModel.staticUnitSystem.Mass.Unit)).GetValue(Unit.Kg));

                //Gotta avoid div0
                if (lpkm == 0)
                {
                    return 0;
                }
                return Math.Round(imperial ? 2.35214 / lpkm : lpkm, 3);
            }
        }

        private string _equivRateLabel;
        public string EquivFuelRateLabel
        {
            get
            {
                return _equivRateLabel;
            }
        }

        /// <summary>
        /// The total GHG Emissions, reported by the simulation
        /// </summary>
        public double GHGEmissions
        {
            get
            {
                if (!_totalGHGEmissions.HasValue)
                {
                    if (Results == null || Results.Count == 0)
                    {
                        return 0;
                    }
                    _totalGHGEmissions = Math.Round(
                        Results.Select(r => r.TotalGHGEmissions).Max(), //What units does this get turned into?
                        Constants.NumDecimals
                    );
                }
                return Math.Round(_totalGHGEmissions.Value,2);
            }
        }
        #endregion

        /// <summary>
        /// Total weight of the vehicle, calculated by the values entered in the parameter config tab
        /// </summary>
        public double TotalWeight
        {
            get
            {
                return Math.Round((new Mass(_totalWeight)).GetValue(ApplicationModel.staticUnitSystem.Mass.Unit),2);
            }
            set
            {
                //Get the current unit
                UnitDescription desc = ApplicationModel.staticUnitSystem.Mass;
                //Create the Quantity
                Mass mass = new Mass(value, desc.Unit);

                //store as canonical
                _totalWeight = mass.CanonicalValue;
                NotifyPropertyChanged("TotalWeight");
            }
        }

        /// <summary>
        /// The total CO2 Emissions, reported by the simulation
        /// </summary>
        public double CO2Emissions
        {
            get
            {
                return _co2Emissions;
            }
            set
            {
                _co2Emissions = value;
                NotifyPropertyChanged("CO2Emissions");
            }
        }

        /// <summary>
        /// The total load moved per kg fuel used
        /// Simple calculation of total weight / total fuel used
        /// </summary>
        public double LoadMovedPerKgFuel
        {
            get { return Math.Round(_loadMovedPerKgFuel,2); }
            set
            {
                _loadMovedPerKgFuel = value;
                NotifyPropertyChanged("LoadMovedPerKgFuel");
            }
        }

        /// <summary>
        /// The type of chart to display
        /// </summary>
        public SimChartType ChartType
        {
            get { return _chartType; }
            set
            {
                _chartType = value;
                NotifyPropertyChanged("ChartType");
            }
        }

        private bool _useDistance;
        /// <summary>
        /// 
        /// </summary>
        public bool UseDistance
        {
            get
            {
                return _useDistance;
            }
            set
            {
                _useDistance = value;
                NotifyPropertyChanged("UseDistance");
                GraphNeedsUpdate = true;
            }
        }

        /// <summary>
        /// The location on the user's computer where the image of the chart to display is stored
        /// </summary>
        public string ChartImageLoc
        {
            get { return _chartImageLoc; }
            set
            {
                _chartImageLoc = value;
                NotifyPropertyChanged("ChartImageLoc");
            }
        }

        /// <summary>
        /// The location on the user's computer where the simulation results .mat file is stored
        /// </summary>
        public string SimOutputLoc
        {
            get { return _simOutputLoc; }
            set
            {
                _simOutputLoc = value;
                NotifyPropertyChanged("SimOutputLoc");
            }
        }

        /// <summary>
        /// If the simulation Graph needs to be updated, and thus a message displayed to the user
        /// Cannot be set if there are no simulation results loaded
        /// </summary>
        public bool GraphNeedsUpdate
        {
            get
            {
                return _graphNeedsUpdate;
            }
            set
            {
                if (Results.Any())
                {
                    _graphNeedsUpdate = value;
                    NotifyPropertyChanged("GraphNeedsUpdate");
                }
            }
        }

        public override void NotifyAllPropertiesChanged()
        {
            NotifyPropertyChanged(string.Empty);
        }

        /// <summary>
        /// The "base" file location.
        /// Points to the user's temp folder, and includes a random file name
        /// </summary>
        public string BaseFileLoc { get; private set; }

        /// <summary>
        /// Helper function used to get the associated file location + name of the image of the specified chart
        /// Used in both finding and storing the chart image
        /// </summary>
        /// <param name="type">Type of chart</param>
        /// <returns>File location of the desired chart</returns>
        public string ImageLoc(SimChartType type)
        {
            switch (type)
            {
                case SimChartType.EngineSpeed:
                    return BaseFileLoc + ENGINESPEED_FILENAME;
                case SimChartType.FuelBurnRate:
                    return BaseFileLoc + FUELBURNRATE_FILENAME;
                case SimChartType.GHGEmissions:
                    return BaseFileLoc + GHGEMISSIONS_FILENAME;
                case SimChartType.LocomotivePower:
                    return BaseFileLoc + LOCOMOTIVEPOWER_FILENAME;
                case SimChartType.AuxFuelConsumption:
                    return BaseFileLoc + AUXFUELCONSUMPTION_FILENAME;
                case SimChartType.AESSState:
                    return BaseFileLoc + AESSSTATE_FILENAME;
                case SimChartType.VehicleSpeed:
                default:
                    return BaseFileLoc + VEHICLESPEED_FILENAME;
            }
        }

        /// <summary>
        /// Constructor for a simulation model
        /// </summary>
        public SimulationModel()
        {
            ChartType = SimChartType.VehicleSpeed;
            BaseFileLoc = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Results = new List<SimResult>();

            ChartImageLoc = ImageLoc(ChartType);
        }

        /// <summary>
        /// Destructor for the simulation Model
        /// </summary>
        ~SimulationModel()
        {
            foreach (SimChartType type in Enum.GetValues(typeof(SimChartType)))
            {
                if (File.Exists(ImageLoc(type)))
                {
                    File.Delete(ImageLoc(type));
                }
            }
            if (File.Exists(SimOutputLoc))
            {
                File.Delete(SimOutputLoc);
            }
            if (File.Exists(System.IO.Path.ChangeExtension(SimOutputLoc, ".csv")))
            {
                File.Delete(System.IO.Path.ChangeExtension(SimOutputLoc, ".csv"));
            }
            if (File.Exists(System.IO.Path.ChangeExtension(SimOutputLoc, "_params.csv")))
            {
                File.Delete(System.IO.Path.ChangeExtension(SimOutputLoc, "_params.csv"));
            }
            if (File.Exists(System.IO.Path.ChangeExtension(SimOutputLoc, ".inputs.csv")))
            {
                File.Delete(System.IO.Path.ChangeExtension(SimOutputLoc, ".inputs.csv"));
            }
        }

        /// <summary>
        /// Enum for the type of chart to use/display
        /// </summary>
        public enum SimChartType
        {
            VehicleSpeed,
            FuelBurnRate,
            EngineSpeed,
            GHGEmissions,
            LocomotivePower,
            AuxFuelConsumption,
            AESSState
        }

        /// <summary>
        /// Calculates the engine fuel rate based on the given parameters. Returns all values in L/km equivalent
        /// </summary>
        /// <param name="type">What type of topology the system is</param>
        /// <param name="totalDistance">The total distance travelled in Km</param>
        /// <param name="totalFuelUse">The total fuel used in Kg</param>
        /// <param name="totalESSPower">The total ESS power used</param>
        /// <returns></returns>
        private double CalculateEqEngineFuelRate(Options.EngineFuelType type, double totalDistance, double totalFuelUse, double totalESSPower, double totalApuFuelUse)
        {
            double totalMiles = (new Distance(totalDistance)).GetValue(Unit.Mi);
            double powerPerHour = totalESSPower / 3600;
            double mpge = 0;
            double apuFuel = totalApuFuelUse / 0.832 / 3.7854118 * 37950;
            if (ApplicationModel.staticTopology == Options.TrainTopology.FuelCell)
            {
                //mpge = 33705/(((0.88*max(sim_fuelcell_h2_cumulative_kg)*2.204/2.198*33705)+(max(sim_apu_fuel_cumulative_kg)/0.832/3.7854118*37950)+(trapz(sim_time_s,sim_ess_power_W)/3600))/((trapz(sim_time_s,sim_vehicle_speed_mps)/1000)*0.62137119));
                mpge = 33705 / (((0.88 * totalFuelUse * 2.204 / 2.198 * 33705) + powerPerHour + apuFuel) / totalMiles);
            }
            else if (ApplicationModel.staticTopology == Options.TrainTopology.Electric)
            {
                //mpge = 37950/((sim_grid_energy_Wh(end))/((trapz(sim_time_s,sim_vehicle_speed_mps)/1000)*0.62137119));
                double finalGridEnergy = _results.Last().GridEnergy;
                mpge = 37950 / ((finalGridEnergy) / totalMiles);
            }
            else
            {
                switch (type)
                {
                    case Options.EngineFuelType.Diesel:
                        //mpge = 37950/(((max(sim_engine_fuel_cumulative_kg)/0.832/3.7854118*37950)+(max(sim_apu_fuel_cumulative_kg)/0.832/3.7854118*37950)+(trapz(sim_time_s,sim_ess_power_W)/3600))/((trapz(sim_time_s,sim_vehicle_speed_mps)/1000)*0.62137119));
                        mpge = 37950 / (((totalFuelUse / 0.832 / 3.7854118 * 37950) + powerPerHour + apuFuel) / totalMiles);

                        break;
                    case Options.EngineFuelType.Gas:
                        //mpge = 33705/(((0.88*max(sim_engine_fuel_cumulative_kg)/0.745/3.7854118*33705)+(max(sim_apu_fuel_cumulative_kg)/0.832/3.7854118*37950)+(trapz(sim_time_s,sim_ess_power_W)/3600))/((trapz(sim_time_s,sim_vehicle_speed_mps)/1000)*0.62137119));                          
                        mpge = 33705 / (((0.88 * totalFuelUse / 0.745 / 3.7854118 * 33705) + powerPerHour + apuFuel) / totalMiles);

                        break;
                    case Options.EngineFuelType.CNG:
                        //mpge = 33705/(((0.88*max(sim_engine_fuel_cumulative_kg)*2.204/5.66*33705)+(max(sim_apu_fuel_cumulative_kg)/0.832/3.7854118*37950)+(trapz(sim_time_s,sim_ess_power_W)/3600))/((trapz(sim_time_s,sim_vehicle_speed_mps)/1000)*0.62137119));
                        mpge = 33705 / (((0.88 * totalFuelUse * 2.204 / 5.66 * 33705) + powerPerHour + apuFuel) / totalMiles);

                        break;
                    case Options.EngineFuelType.LNG:
                    default:
                        //mpge = 33705/(((0.88*max(sim_fuelcell_h2_cumulative_kg)*2.204/2.198*33705)+(max(sim_apu_fuel_cumulative_kg)/0.832/3.7854118*37950)+(trapz(sim_time_s,sim_ess_power_W)/3600))/((trapz(sim_time_s,sim_vehicle_speed_mps)/1000)*0.62137119));
                        mpge = 33705 / (((0.88 * totalFuelUse * 2.204 / 5.38 * 33705) + powerPerHour + apuFuel) / totalMiles);

                        break;
                }
            }
            double lpkme = 2.35214 / mpge;
            return lpkme;
        }

        /// <summary>
        /// Calculates the engine fuel rate based on the given parameters.
        /// Returns values either in L or m^3 (if fuel cell or CNG)
        /// </summary>
        /// <param name="type">What type of topology the system is</param>
        /// <param name="totalDistance">The total distance travelled in Km</param>
        /// <param name="totalFuelUse">The total fuel used in Kg</param>
        /// <returns></returns>
        private double CalculateActualEngineFuelUse(Options.EngineFuelType type, double totalFuelUse)
        {
            bool imperial = ApplicationModel.staticUnitSystem.FuelConsumption.Unit == Unit.MiPerGal;

            double lpkm = 0;

            if (ApplicationModel.staticTopology == Options.TrainTopology.FuelCell)
            {
                //m3Pkm = (max(sim_fuelcell_h2_cumulative_kg)/0.0899)/(trapz(sim_time_s,sim_vehicle_speed_mps)/1000);
                return (totalFuelUse / 0.0899);
            }
            //nothing for electric!
            else
            {
                switch (type)
                {
                    case Options.EngineFuelType.Diesel:
                        //LPkm = (max(sim_engine_fuel_cumulative_kg)/0.832)/(trapz(sim_time_s,sim_vehicle_speed_mps)/1000);
                        lpkm = (totalFuelUse / 0.832);

                        break;
                    case Options.EngineFuelType.Gas:
                        //LPkm = (max(sim_engine_fuel_cumulative_kg)/0.745)/(trapz(sim_time_s,sim_vehicle_speed_mps)/1000);
                        lpkm = (totalFuelUse / 0.745);

                        break;
                    case Options.EngineFuelType.CNG:
                        //m3Pkm = (max(sim_engine_fuel_cumulative_kg)/0.679)/(trapz(sim_time_s,sim_vehicle_speed_mps)/1000);
                        lpkm = (totalFuelUse / 0.679);

                        break;
                    case Options.EngineFuelType.LNG:
                    default:
                        //LPkm = (max(sim_engine_fuel_cumulative_kg)/0.45)/(trapz(sim_time_s,sim_vehicle_speed_mps)/1000);
                        lpkm = (totalFuelUse / 0.45);

                        break;
                }
            }
            return lpkm;
        }

        private void ClearAllSummaries()
        {
            _totalFuelUse = null;
            _totalApuFuelUse = null;
            _totalGridEnergyUse = null;
            _totalGHGEmissions = null;
            _distanceTravelled = null;
            _totalESSPower = null;
        }
    }
}
