using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using REMForm.ViewModels.Options;
using TrainSimulatorCLI;
using System.IO;
using CsvHelper;
using REMForm.ViewModels;

namespace REMForm.Helpers
{
    //This isn't exactly like the factory design pattern, but close enough
    public class SimWrapper
    {
        /// <summary>
        /// The train simulator that we're going to use
        /// </summary>
        public TrainSimulator Simulator { get; private set; }

        /// <summary>
        /// Parameters to pass to the simulator
        /// </summary>
        public List<Parameter> SimulationParameters { get; private set; }


        /// <summary>
        /// Outputs resulting from running the simulator
        /// </summary>
        private Dictionary<mdlOutputNames, double[]> Outputs { get; set; }

        #region Properties representing obfuscated simulation outputs

        /// <summary>
        /// Vehicle Speed
        /// </summary>
        public double[] Speed
        {
            get
            {
                return GetOutput(mdlOutputNames.sim_vehicle_speed_mps);
            }
        }

        /// <summary>
        /// Time points
        /// </summary>
        public double[] Time
        {
            get
            {
                return GetOutput(mdlOutputNames.sim_time_s);
            }
        }

        /// <summary>
        /// Engine Speed
        /// </summary>
        public double[] EngineSpeed
        {
            get
            {
                return GetOutput(mdlOutputNames.sim_engine_speed_radps);
            }
        }

        /// <summary>
        /// Fuel Rate
        /// </summary>
        public double[] FuelRate
        {
            get
            {
                return GetOutput(mdlOutputNames.sim_engine_fuel_rate_kgps);
            }
        }

        /// <summary>
        /// Fuel Rate for Fuel Cell
        /// </summary>
        public double[] FuelH2Rate
        {
            get
            {
                return GetOutput(mdlOutputNames.sim_fuelcell_h2_rate_kgps);
            }
        }

        /// <summary>
        /// Total fuel Use
        /// </summary>
        public double[] TotalFuelUse
        {
            get
            {
                return GetOutput(mdlOutputNames.sim_engine_fuel_cumulative_kg);
            }
        }

        /// <summary>
        /// Total fuel Use for Fuel Cell
        /// </summary>
        public double[] TotalH2FuelUse
        {
            get
            {
                return GetOutput(mdlOutputNames.sim_fuelcell_h2_cumulative_kg);
            }
        }

        /// <summary>
        /// Total APU fuel use
        /// </summary>
        public double[] APUTotalFuelUse
        {
            get
            {
                return GetOutput(mdlOutputNames.sim_apu_fuel_cumulative_kg);
            }
        }

        /// <summary>
        /// APU fuel rate
        /// </summary>
        public double[] APUFuelRate
        {
            get
            {
                return GetOutput(mdlOutputNames.sim_apu_fuel_rate_kgps);
            }
        }

        /// <summary>
        /// APU fuel rate
        /// </summary>
        public double[] APUPower
        {
            get
            {
                return GetOutput(mdlOutputNames.sim_apu_power_W);
            }
        }

        /// <summary>
        /// Prime Engine Speed
        /// </summary>
        public double[] PrimeEngineSpeed
        {
            get
            {
                return GetOutput(mdlOutputNames.sim_engine_speed_radps);
            }
        }

        /// <summary>
        /// Prime Engine Torque
        /// </summary>
        public double[] PrimeTorque
        {
            get
            {
                return GetOutput(mdlOutputNames.sim_engine_torque_N);
            }
        }

        /// <summary>
        /// Total GHG Emissions
        /// </summary>
        public double[] TotalGHGEmissions
        {
            get
            {
                return GetOutput(mdlOutputNames.sim_locomotive_GHG_emissions_cumulative_kg);
            }
        }

        /// <summary>
        /// AESS On/Off State
        /// </summary>
        public double[] AESSOnOffState
        {
            get
            {
                return GetOutput(mdlOutputNames.sim_AESS_on_bool);
            }
        }

        /// <summary>
        /// Engine Power
        /// </summary>
        public double[] EnginePower
        {
            get
            {
                return GetOutput(mdlOutputNames.sim_engine_power_W);
            }
        }

        /// <summary>
        /// FuelCell Power
        /// </summary>
        public double[] FuelCellPower
        {
            get
            {
                return GetOutput(mdlOutputNames.sim_fuelcell_power_W);
            }
        }

        /// <summary>
        /// ESS Power
        /// </summary>
        public double[] ESSPower
        {
            get
            {
                return GetOutput(mdlOutputNames.sim_ess_power_W);
            }
        }

        /// <summary>
        /// GridEnergy
        /// </summary>
        public double[] GridEnergy
        {
            get
            {
                return GetOutput(mdlOutputNames.sim_grid_energy_Wh);
            }
        }

        /// <summary>
        /// Tractive Power
        /// </summary>
        public double[] TractivePower
        {
            get
            {
                return GetOutput(mdlOutputNames.sim_tractive_power_W);
            }
        }

        /// <summary>
        /// Calculates an array representing dumb-integration between the time and value arrays
        /// </summary>
        /// <param name="time">Time vector</param>
        /// <param name="val">some value vector</param>
        /// <returns></returns>
        /// <remarks>Assumes that time and val are the same size</remarks>
        public IEnumerable<double> DumbIntegrate(double[] time, double[] val)
        {
            double total = 0;
            yield return total;
            for (int i = 1; i < time.Count(); i++)
            {
                double timeDelta = time[i] - time[i - 1];

                double scaledVal = val[i] * timeDelta;
                total += scaledVal;
                yield return total;
            }
        }
        public IEnumerable<double> CumulativeDistance()
        {
            return DumbIntegrate(Time, Speed);
        }

        public IEnumerable<double> CumulativeESSPower()
        {
            return DumbIntegrate(Time, ESSPower);
        }
        #endregion

        #region Properties representing obfuscated constant values

        /// <summary>
        /// max_apu_pwr_index
        /// </summary>
        private double? _maxAPUPower;
        public double MaxAPUPower
        {
            get
            {
                if (!_maxAPUPower.HasValue)
                {
                    _maxAPUPower = Simulator.GetConst(mdlConstantNames.max_apu_pwr_index);
                }

                return _maxAPUPower.Value;
            }
        }

        /// <summary>
        /// max_cap_capacitance
        /// </summary>
        private double? _maxCapCapacitance;
        public double MaxCapCapacitance
        {
            get
            {
                if (!_maxCapCapacitance.HasValue)
                {
                    _maxCapCapacitance = Simulator.GetConst(mdlConstantNames.max_cap_capacitance);
                }
                return _maxCapCapacitance.Value;
            }
        }

        /// <summary>
        /// max_cap_res
        /// </summary>
        private double? _maxCapRes;
        public double MaxCapRes
        {
            get
            {
                if (!_maxCapRes.HasValue)
                {
                    _maxCapRes = Simulator.GetConst(mdlConstantNames.max_cap_res);
                }
                return _maxCapRes.Value;
            }
        }

        /// <summary>
        /// max_dyn_brk_mc_spd
        /// </summary>
        private double? _maxDynamicBreakMCSPD;
        public double MaxDynamicBreakMCSPD
        {
            get
            {
                if (!_maxDynamicBreakMCSPD.HasValue)
                {
                    _maxDynamicBreakMCSPD = Simulator.GetConst(mdlConstantNames.max_dyn_brk_mc_spd);
                }
                return _maxDynamicBreakMCSPD.Value;
            }
        }

        /// <summary>
        /// max_eng_power
        /// </summary>
        private double? _maxEngPower;
        public double MaxEngPower
        {
            get
            {
                if (!_maxEngPower.HasValue)
                {
                    _maxEngPower = Simulator.GetConst(mdlConstantNames.max_eng_power);
                }
                return _maxEngPower.Value;
            }
        }

        /// <summary>
        /// max_eng_trq_fuel_index
        /// </summary>
        private double? _maxEngTorqueFuelIndex;
        public double MaxEngTorqueFuelIndex
        {
            get
            {
                if (!_maxEngTorqueFuelIndex.HasValue)
                {
                    _maxEngTorqueFuelIndex = Simulator.GetConst(mdlConstantNames.max_eng_trq_fuel_index);
                }
                return _maxEngTorqueFuelIndex.Value;
            }
        }

        /// <summary>
        /// max_fc_pwr
        /// </summary>
        private double? _maxFCPower;
        public double MaxFCPower
        {
            get
            {
                if (!_maxFCPower.HasValue)
                {
                    _maxFCPower = Simulator.GetConst(mdlConstantNames.max_fc_pwr);
                }
                return _maxFCPower.Value;
            }
        }

        /// <summary>
        /// max_fly_energy
        /// </summary>
        private double? _maxFlywheelEnergy;
        public double MaxFlywheelEnergy
        {
            get
            {
                if (!_maxFlywheelEnergy.HasValue)
                {
                    _maxFlywheelEnergy = Simulator.GetConst(mdlConstantNames.max_fly_energy);
                }
                return _maxFlywheelEnergy.Value;
            }
        }

        /// <summary>
        /// max_fly_pwr
        /// </summary>
        private double? _maxFlywheelPower;
        public double MaxFlywheelPower
        {
            get
            {
                if (!_maxFlywheelPower.HasValue)
                {
                    _maxFlywheelPower = Simulator.GetConst(mdlConstantNames.max_fly_pwr);
                }
                return _maxFlywheelPower.Value;
            }
        }

        /// <summary>
        /// max_mc_pwr
        /// </summary>
        private double? _maxMCPower;
        public double MaxMCPower
        {
            get
            {
                if (!_maxMCPower.HasValue)
                {
                    _maxMCPower = Simulator.GetConst(mdlConstantNames.max_mc_pwr);
                }
                return _maxMCPower.Value;
            }
        }

        /// <summary>
        /// max_mc_spd_index
        /// </summary>
        private double? _maxMCSpeedIndex;
        public double MaxMCSpeedIndex
        {
            get
            {
                if (!_maxMCSpeedIndex.HasValue)
                {
                    _maxMCSpeedIndex = Simulator.GetConst(mdlConstantNames.max_mc_spd_index);
                }
                return _maxMCSpeedIndex.Value;
            }
        }

        /// <summary>
        /// max_mc_eff
        /// </summary>
        private double? _maxMCEfficiency;
        public double MaxMCEfficiency
        {
            get
            {
                if (!_maxMCEfficiency.HasValue)
                {
                    _maxMCEfficiency = Simulator.GetConst(mdlConstantNames.max_mc_eff);
                }
                return _maxMCEfficiency.Value;
            }
        }

        /// <summary>
        /// max_mc_trq_index
        /// </summary>
        private double? _maxMCTorqueIndex;
        public double MaxMCTorqueIndex
        {
            get
            {
                if (!_maxMCTorqueIndex.HasValue)
                {
                    _maxMCTorqueIndex = Simulator.GetConst(mdlConstantNames.max_mc_trq_index);
                }
                return _maxMCTorqueIndex.Value;
            }
        }

        /// <summary>
        /// max_ptc_ess_pwr_map
        /// </summary>
        private double? _maxPTCESSPowerMap;
        public double MaxPTCESSPowerMap
        {
            get
            {
                if (!_maxPTCESSPowerMap.HasValue)
                {
                    _maxPTCESSPowerMap = Simulator.GetConst(mdlConstantNames.max_ptc_ess_pwr_map);
                }
                return _maxPTCESSPowerMap.Value;
            }
        }

        /// <summary>
        /// max_ptc_wh_trq_index
        /// </summary>
        private double? _maxPTCWheelTorqueIndex;
        public double MaxPTCWheelTorqueIndex
        {
            get
            {
                if (!_maxPTCWheelTorqueIndex.HasValue)
                {
                    _maxPTCWheelTorqueIndex = Simulator.GetConst(mdlConstantNames.max_ptc_wh_trq_index);
                }
                return _maxPTCWheelTorqueIndex.Value;
            }
        }

        /// <summary>
        /// mean_cap_voltage
        /// </summary>
        private double? _meanCapVoltage;
        public double MeanCapVoltage
        {
            get
            {
                if (!_meanCapVoltage.HasValue)
                {
                    _meanCapVoltage = Simulator.GetConst(mdlConstantNames.mean_cap_voltage);
                }
                return _meanCapVoltage.Value;
            }
        }

        /// <summary>
        /// mean_eng_eff
        /// </summary>
        private double? _meanEngineEfficiency;
        public double MeanEngineEfficiency
        {
            get
            {
                if (!_meanEngineEfficiency.HasValue)
                {
                    _meanEngineEfficiency = Simulator.GetConst(mdlConstantNames.mean_eng_eff);
                }
                return _meanEngineEfficiency.Value;
            }
        }

        /// <summary>
        /// mean_fc_eff
        /// </summary>
        private double? _meanFCEfficiency;
        public double MeanFCEfficiency
        {
            get
            {
                if (!_meanFCEfficiency.HasValue)
                {
                    _meanFCEfficiency = Simulator.GetConst(mdlConstantNames.mean_fc_eff);
                }
                return _meanFCEfficiency.Value;
            }
        }

        /// <summary>
        /// mean_gc_eff
        /// </summary>
        private double? _meanGCEfficiency;
        public double MeanGCEfficiency
        {
            get
            {
                if (!_meanGCEfficiency.HasValue)
                {
                    _meanGCEfficiency = Simulator.GetConst(mdlConstantNames.mean_gc_eff);
                }
                return _meanGCEfficiency.Value;
            }
        }

        /// <summary>
        /// mean_mc_eff
        /// </summary>
        private double? _meanMCEfficiency;
        public double MeanMCEfficiency
        {
            get
            {
                if (!_meanMCEfficiency.HasValue)
                {
                    _meanMCEfficiency = Simulator.GetConst(mdlConstantNames.mean_mc_eff);
                }
                return _meanMCEfficiency.Value;
            }
        }

        /// <summary>
        /// min_eng_trq_fuel_index
        /// </summary>
        private double? _minEngineTorqueFuelIndex;
        public double MinEngineTorqueFuelIndex
        {
            get
            {
                if (!_minEngineTorqueFuelIndex.HasValue)
                {
                    _minEngineTorqueFuelIndex = Simulator.GetConst(mdlConstantNames.min_eng_trq_fuel_index);
                }
                return _minEngineTorqueFuelIndex.Value;
            }
        }

        /// <summary>
        /// min_mc_trq_index
        /// </summary>
        private double? _minMCTorqueIndex;
        public double MinMCTorqueIndex
        {
            get
            {
                if (!_minMCTorqueIndex.HasValue)
                {
                    _minMCTorqueIndex = Simulator.GetConst(mdlConstantNames.min_mc_trq_index);
                }
                return _minMCTorqueIndex.Value;
            }
        }

        /// <summary>
        /// veh_init_max_accel
        /// </summary>
        private double? _maxAccel;
        public double MaxAccel
        {
            get
            {
                if (!_maxAccel.HasValue)
                {
                    _maxAccel = Simulator.GetConst(mdlConstantNames.veh_max_accel);
                }
                return _maxAccel.Value;
            }
        }

        private double? _essInitCapNom;
        public double ESSInitCapNom
        {
            get
            {
                if (!_essInitCapNom.HasValue)
                {
                    _essInitCapNom = Simulator.GetConst(mdlConstantNames.ess_init_cap_nom);
                }
                return _essInitCapNom.Value;
            }
        }

        private double? _essInitVoltage;
        public double ESSInitVoltage
        {
            get
            {
                if (!_essInitVoltage.HasValue)
                {
                    _essInitVoltage = Simulator.GetConst(mdlConstantNames.ess_init_voltage);
                }
                return _essInitVoltage.Value;
            }
        }

        #endregion

        /// <summary>
        /// Constructor for the simwrapper
        /// Initializes the train simulator. Chooses a different one based on the inputs
        /// </summary>
        /// <param name="topology">The type of train topology</param>
        /// <param name="storageSystem">The type of energy storage system</param>
        public SimWrapper(TrainTopology topology, EnergyStorageSystem storageSystem)
        {
            Outputs = new Dictionary<mdlOutputNames, double[]>();
            string executionPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            mdlSimlinkName simlinkName = GetSimlinkName(topology, storageSystem);
            switch (simlinkName)
            {
                case mdlSimlinkName.ELECTRIC:
                    Simulator = new ElectricSimulator(simlinkName, executionPath);
                    break;
                case mdlSimlinkName.ENGINE_HYBRID_BATTERY:
                    Simulator = new EngineHybridBatterySimulator(simlinkName, executionPath);
                    break;
                case mdlSimlinkName.ENGINE_HYBRID_FLYWHEEL:
                    Simulator = new EngineHybridFlywheelSimulator(simlinkName, executionPath);
                    break;
                case mdlSimlinkName.ENGINE_HYBRID_ULTRACAPACITOR:
                    Simulator = new EngineHybridUltracapacitorSimulator(simlinkName, executionPath);
                    break;
                case mdlSimlinkName.FUEL_CELL_HYBRID_BATTERY:
                    Simulator = new FuelCellHybridBatterySimulator(simlinkName, executionPath);
                    break;
                case mdlSimlinkName.FUEL_CELL_HYBRID_FLYWHEEL:
                    Simulator = new FuelCellHybridFlywheelSimulator(simlinkName, executionPath);
                    break;
                case mdlSimlinkName.FUEL_CELL_HYBRID_ULTRACAPACITOR:
                    Simulator = new FuelCellHybridUltracapacitorSimulator(simlinkName, executionPath);
                    break;
                case mdlSimlinkName.ENGINE_ELECTRIC:
                default:
                    Simulator = new EngineElectricSimulator(simlinkName, executionPath);
                    break;
            }
        }

        private mdlSimlinkName GetSimlinkName(TrainTopology topology, EnergyStorageSystem storageSystem)
        {
            switch (topology)
            {
                case TrainTopology.Electric:
                    return mdlSimlinkName.ELECTRIC;
                case TrainTopology.EngineHybrid:
                    switch (storageSystem)
                    {
                        case EnergyStorageSystem.Battery:
                            return mdlSimlinkName.ENGINE_HYBRID_BATTERY;
                        case EnergyStorageSystem.Flywheel:
                            return mdlSimlinkName.ENGINE_HYBRID_FLYWHEEL;
                        case EnergyStorageSystem.Ultracapacitive:
                            return mdlSimlinkName.ENGINE_HYBRID_ULTRACAPACITOR;
                    }
                    break;
                case TrainTopology.FuelCell:
                    switch (storageSystem)
                    {
                        case EnergyStorageSystem.Battery:
                            return mdlSimlinkName.FUEL_CELL_HYBRID_BATTERY;
                        case EnergyStorageSystem.Flywheel:
                            return mdlSimlinkName.FUEL_CELL_HYBRID_FLYWHEEL;
                        case EnergyStorageSystem.Ultracapacitive:
                            return mdlSimlinkName.FUEL_CELL_HYBRID_ULTRACAPACITOR;
                    }
                    break;
                case TrainTopology.EngineElectric:
                default:
                    return mdlSimlinkName.ENGINE_ELECTRIC;
            }
            return mdlSimlinkName.ENGINE_ELECTRIC;
        }

        /// <summary>
        /// Destructor for this wrapper.
        /// Makes sure we dispose of the Simulator
        /// </summary>
        ~SimWrapper()
        {
            if (Simulator != null)
            {
                Simulator.Dispose();
            }
        }

        /// <summary>
        /// Adds the provided inputs to the simulator, in preparation for simulation
        /// </summary>
        /// <param name="inputVals">List of input names and their associated values</param>
        public void AddInputs(Dictionary<mdlInputNames, double[]> inputVals)
        {
            foreach (KeyValuePair<mdlInputNames, double[]> input in inputVals)
            {
                if (Simulator.Inputs.Contains(input.Key))
                {
                    Simulator.SetInput(input.Key, input.Value);
                }
            }

            ExportInputsToCSV(inputVals);
        }

        /// <summary>
        /// Makes the train simulator begin simulation.
        /// After simulation, collects all the outputs (so we don't have to constantly query the simulator afterwards)
        /// The simulator creates a .mat file after being run, but we would also like a .csv file to be created.
        /// </summary>
        public void Simulate()
        {
            ExportParamsToCSV();
            Simulator.Simulate(SimulationParameters.Select(param => param.CLIStruct).ToArray());
            Outputs = new Dictionary<mdlOutputNames, double[]>();

            foreach (mdlOutputNames outputName in Simulator.Outputs)
            {
                double[] vals = Simulator.GetOutput(outputName);
                Outputs.Add(outputName, vals);
            }

            ExportOutputsToCSV();
        }

        private void ExportInputsToCSV(Dictionary<mdlInputNames, double[]> inputVals)
        {
            string filePath = Simulator.GetOutputFileLoc();
            filePath = System.IO.Path.ChangeExtension(filePath, ".inputs.csv");
            using (StreamWriter writer = new StreamWriter(filePath, false))
            using (CsvWriter csvWriter = new CsvWriter(writer))
            {
                foreach (mdlInputNames inputName in inputVals.Select(o => o.Key))
                {
                    csvWriter.WriteField(inputName.ToString());
                }
                csvWriter.NextRecord();

                int numEntries = inputVals.First().Value.Count();

                for (int i = 0; i < numEntries; i++)
                {
                    foreach (mdlInputNames inputName in inputVals.Select(o => o.Key))
                    {
                        csvWriter.WriteField(inputVals[inputName][i]);
                    }
                    csvWriter.NextRecord();
                }
            }
        }

        /// <summary>
        /// Exports the simulation outputs into a .csv filen in the same directory as the generated .mat file
        /// </summary>
        private void ExportOutputsToCSV()
        {
            string filePath = Simulator.GetOutputFileLoc();
            filePath = System.IO.Path.ChangeExtension(filePath, ".csv");
            using (StreamWriter writer = new StreamWriter(filePath, false))
            using (CsvWriter csvWriter = new CsvWriter(writer))
            {
                foreach (mdlOutputNames outputName in Outputs.Select(o => o.Key))
                {
                    csvWriter.WriteField(outputName.ToString());
                }
                csvWriter.NextRecord();

                int numEntries = Outputs.First().Value.Count();

                for (int i = 0; i < numEntries; i++)
                {
                    foreach (double[] outputName in Outputs.Select(o => o.Value))
                    {
                        csvWriter.WriteField(outputName[i]);
                    }
                    csvWriter.NextRecord();
                }
            }
        }

        private void ExportParamsToCSV()
        {
            string filePath = Simulator.GetOutputFileLoc();
            filePath = System.IO.Path.ChangeExtension(filePath, "_params.csv");
            using (StreamWriter writer = new StreamWriter(filePath, false))
            using (CsvWriter csvWriter = new CsvWriter(writer))
            {
                foreach (var param in SimulationParameters)
                {
                    csvWriter.WriteField(param.Identifier);
                    foreach (var val in param.Values)
                    {
                        csvWriter.WriteField(val);
                    }
                    csvWriter.NextRecord();
                }
            }
        }

        /// <summary>
        /// Grabs the values associated to the provided mdlOutputName
        /// </summary>
        /// <param name="outputName">name of the output we want to get</param>
        /// <returns>Double array of values associated to the outputName</returns>
        private double[] GetOutput(mdlOutputNames outputName)
        {
            if (Outputs.ContainsKey(outputName))
            {
                return Outputs[outputName];
            }
            return new double[] { };
        }

        /// <summary>
        /// Helper function for generating the entire list of parameters which need to be modified in the _params.mat file
        /// </summary>
        /// <param name="paramModel">A model which contains parameters customized by the user</param>
        public void UpdateParameters(ParameterModel paramModel)
        {
            List<Parameter> paramList = new List<Parameter>();

            paramList.AddRange(paramModel.InputParameters());

            paramList.AddRange(CalculateSimulationParameters(paramModel)
                .Select(param => new Parameter(param.Identifier, param.Values)));

            SimulationParameters = paramList;
        }


        /// <summary>
        /// Calculate the scaled parameters for the simulation based on what typology and ESS are chosen
        /// </summary>
        /// <remarks>
        /// Originally this was done within the simulator
        /// However there are FAR too many variables that would need to be passed in as the CLI stuff can't know about the ParameterModel structure</remarks>
        private List<ParamStruct> CalculateSimulationParameters(ParameterModel model)
        {
            List<ParamStruct> structs = new List<ParamStruct>();

            #region Standard Sim Parameters
            //max_motor_spd 	top_speed/3.6/wh.init.radius*fd.init.ratio;	
            double max_motor_spd = model.VehicleOption.Parameters._maxSpeed / 3.6 / (model.VehicleOption.Parameters._wheelRadius * 1000) * model.VehicleOption.Parameters._gearRatio;
            ParamStruct temp = new ParamStruct(
                mdlParameterNames.max_motor_spd.ToString(),
                new double[] { max_motor_spd }
            );
            structs.Add(temp);

            //mc.init.spd_eff_index = mc.init.spd_eff_index*max_motor_spd/max(mc.init.spd_eff_index);	const: max_mc_spd_index
            double[] spd_eff_index = Simulator.GetParam(mdlParameterNames.mc_init_spd_eff_index);
            spd_eff_index = spd_eff_index.Select(v => v * max_motor_spd / MaxMCSpeedIndex).ToArray();
            temp = new ParamStruct(
                 mdlParameterNames.mc_init_spd_eff_index.ToString(),
                 spd_eff_index
             );
            structs.Add(temp);


            //mc.init.spd_prop_cont_index = mc.init.spd_prop_cont_index*max_motor_spd/max(mc.init.spd_prop_cont_index);	const: max_mc_spd_index
            double[] mc_spd_prop_cont_index = Simulator.GetParam(mdlParameterNames.mc_init_spd_prop_cont_index);
            mc_spd_prop_cont_index = mc_spd_prop_cont_index.Select(v => v * max_motor_spd / MaxMCSpeedIndex).ToArray();
            temp = new ParamStruct(
                 mdlParameterNames.mc_init_spd_prop_cont_index.ToString(),
                 mc_spd_prop_cont_index
             );
            structs.Add(temp);


            //double maxMCPower = spd_prop_cont_index.Select((v, i) => v * mc_trq_prop_cont_map.ElementAt(i)).Max();

            //dynamic_brake.trac_effort = dynamic_brake.trac_effort .* (wh.init.radius / fd.init.ratio);	
            double[] dynamic_brake_trac_effort = Simulator.GetParam(mdlParameterNames.dynamic_brake_trac_effort);
            dynamic_brake_trac_effort = dynamic_brake_trac_effort.Select(v => v * ((model.VehicleOption.Parameters._wheelRadius * 1000) / model.VehicleOption.Parameters._gearRatio)).ToArray();
            temp = new ParamStruct(
                 mdlParameterNames.dynamic_brake_trac_effort.ToString(),
                 dynamic_brake_trac_effort
             );
            structs.Add(temp);

            //dynamic_brake.brake_effort = dynamic_brake.brake_effort .* (wh.init.radius / fd.init.ratio);	
            double[] dynamic_brake_brake_effort = Simulator.GetParam(mdlParameterNames.dynamic_brake_brake_effort);
            dynamic_brake_brake_effort = dynamic_brake_brake_effort.Select(v => v * ((model.VehicleOption.Parameters._wheelRadius * 1000) / model.VehicleOption.Parameters._gearRatio)).ToArray();
            temp = new ParamStruct(
                 mdlParameterNames.dynamic_brake_brake_effort.ToString(),
                 dynamic_brake_brake_effort
             );
            structs.Add(temp);

            //drv.init.kp = 3/5 * (veh.init.mass + veh.init.mass_cars * veh.init.num_cars);	
            double drv_init_kp = 3.0 / 5.0 * (model.VehicleOption.Parameters._locomotiveMass +
                                            (model.VehicleOption.Parameters._carMass * model.VehicleOption.Parameters._numCars));
            temp = new ParamStruct(
                mdlParameterNames.drv_init_kp.ToString(),
                new double[] { drv_init_kp }
            );
            structs.Add(temp);

            //dynamic_brake.mc_spd = dynamic_brake.mc_spd ./ (wh.init.radius * fd.init.ratio);
            //dynamic_brake.mc_spd = dynamic_brake.mc_spd ./ max(dynamic_brake.mc_spd); const: max_dyn_brk_mc_spd
            //dynamic_brake.mc_spd = dynamic_brake.mc_spd .* max_motor_spd;	
            double[] dynamic_brake_mc_spd = Simulator.GetParam(mdlParameterNames.dynamic_brake_mc_spd);
            dynamic_brake_mc_spd = dynamic_brake_mc_spd.Select(v => v / ((model.VehicleOption.Parameters._wheelRadius * 1000) / model.VehicleOption.Parameters._gearRatio)).ToArray();

            double maxDynBrakeSpd = dynamic_brake_mc_spd.Max();
            dynamic_brake_mc_spd = dynamic_brake_mc_spd.Select(v => (v / maxDynBrakeSpd) * max_motor_spd).ToArray();

            temp = new ParamStruct(
                mdlParameterNames.dynamic_brake_mc_spd.ToString(),
                dynamic_brake_mc_spd
            );
            structs.Add(temp);

            //reg_corr = dynamic_brake.brake_effort ./ dynamic_brake.trac_effort;	
            double[] reg_corr = dynamic_brake_brake_effort.Select((v, i) => v / dynamic_brake_trac_effort[i]).ToArray();
            structs.Add(new ParamStruct("CalculateStandardSimParameters_reg_corr", reg_corr));

            if (model.TrainOption.Topology == TrainTopology.EngineHybrid ||
                model.TrainOption.Topology == TrainTopology.FuelCell)
            {
                structs.AddRange(CalculateHybridSimParameters(model));
            }

            //mc.init.eff_inverse_trq_map = mc.init.eff_inverse_trq_map*mean(mean(mc.init.eff_trq_map))/mc_eff;	const: mean_mc_eff
            double[] eff_inverse_trq_map = Simulator.GetParam(mdlParameterNames.mc_init_eff_inverse_trq_map);

            eff_inverse_trq_map = eff_inverse_trq_map.Select(v => v * MeanMCEfficiency / model.PowertrainOption._motorEfficiency).ToArray();

            temp = new ParamStruct(
                mdlParameterNames.mc_init_eff_inverse_trq_map.ToString(),
                eff_inverse_trq_map
            );
            structs.Add(temp);

            //mc.init.eff_trq_map = mc.init.eff_trq_map*mc_eff/mean(mean(mc.init.eff_trq_map));	const: mean_mc_eff
            double[] eff_trq_map = Simulator.GetParam(mdlParameterNames.mc_init_eff_trq_map);
            eff_trq_map = eff_trq_map.Select(v => v * model.PowertrainOption._motorEfficiency / MeanMCEfficiency).ToArray();
            temp = new ParamStruct(
                 mdlParameterNames.mc_init_eff_trq_map.ToString(),
                 eff_trq_map
             );
            structs.Add(temp);

            double primePower = model.PowertrainOption.CanonicalPrimePower;
            double essPower = model.PowertrainOption.CanonicalESSPower;

            double[] mc_trq_prop_cont_map = Simulator.GetParam(mdlParameterNames.mc_init_trq_prop_cont_map);

            //temp_pwr = max(mc.init.spd_prop_cont_index.*mc.init.trq_prop_cont_map); %NEW		
            double temp_pwr = mc_spd_prop_cont_index.Select((v, i) => v * mc_trq_prop_cont_map[i]).Max();

            //mc.init.trq_prop_cont_map = mc.init.trq_prop_cont_map.*(PRIMEPOWER*1000/temp_pwr); %NEW
            mc_trq_prop_cont_map = mc_trq_prop_cont_map.Select(v => v * ((primePower + essPower) * 1000.0 / temp_pwr)).ToArray();
            temp = new ParamStruct(
                 mdlParameterNames.mc_init_trq_prop_cont_map.ToString(),
                 mc_trq_prop_cont_map
             );
            structs.Add(temp);

            //mc.init.trq_eff_index = mc.init.trq_eff_index.*(PRIMEPOWER*1000/temp_pwr); %NEW
            double[] trq_eff_index = Simulator.GetParam(mdlParameterNames.mc_init_trq_eff_index);
            trq_eff_index = trq_eff_index.Select(v => v * ((primePower + essPower) * 1000.0 / temp_pwr)).ToArray();
            temp = new ParamStruct(
                 mdlParameterNames.mc_init_trq_eff_index.ToString(),
                 trq_eff_index
             );
            structs.Add(temp);

            //mc.init.trq_reg_cont_map = -mc.init.trq_prop_cont_map.*reg_corr;	
            double[] mc_trq_reg_cont_map = reg_corr.Select((v, i) => -1 * mc_trq_prop_cont_map[i] * v).ToArray();
            temp = new ParamStruct(
                 mdlParameterNames.mc_init_trq_reg_cont_map.ToString(),
                 mc_trq_reg_cont_map
             );
            structs.Add(temp);

            //wh.init.trq_brake_max = max(mc.init.trq_prop_cont_map)*fd.init.eff*fd.init.ratio;	const: max_mc_trq_index
            double trq_brake_max = mc_trq_prop_cont_map.Max() * model.PowertrainOption.FinalDriveEfficiency * model.VehicleOption.Parameters._gearRatio;
            temp = new ParamStruct(
                mdlParameterNames.wh_init_trq_brake_max.ToString(),
                new double[] { trq_brake_max }
            );
            structs.Add(temp);
            #endregion
            structs.Add(new ParamStruct("maxMCPower", new double[] { MaxMCPower }));

            if (model.TrainOption.Topology == TrainTopology.EngineElectric)
            {
                structs.AddRange(CalculateEngineParameters(model));
                //power_corr = (PRIMEPOWER*1000)/max(eng.init.spd_max_hot_index.*eng.init.trq_max_hot_map); const: max_eng_power
                double power_corr = (model.PowertrainOption.CanonicalPrimePower * 1000) / MaxEngPower;
                structs.Add(new ParamStruct("CalculateSimulationParameters_power_corr", new double[] { power_corr }));

                //mc.init.pwr_eff_prop_index = mc.init.pwr_eff_prop_index*power_corr;
                double[] pwr_eff_prop_index = Simulator.GetParam(mdlParameterNames.mc_init_pwr_eff_prop_index);
                pwr_eff_prop_index = pwr_eff_prop_index.Select(v => v * power_corr).ToArray();
                structs.Add(new ParamStruct(mdlParameterNames.mc_init_pwr_eff_prop_index.ToString(), pwr_eff_prop_index));

            }
            else if (model.TrainOption.Topology == TrainTopology.Electric)
            {
                //power_corr 	(PRIMEPOWER*1000)/max(mc.init.spd_prop_cont_index.*mc.init.trq_prop_cont_map);	max_mc_pwr
                double power_corr = (model.PowertrainOption.CanonicalPrimePower * 1000) / MaxMCPower;
                structs.Add(new ParamStruct("CalculateSimulationParameters_power_corr", new double[] { power_corr }));

                //max_mc_calc_pwr_elec_eff_map = PRIMEPOWER*1000/max(max(mc.init.eff_trq_map));	
                double max_pwr_elec_eff_map = (model.PowertrainOption.CanonicalPrimePower * 1000) / MaxMCEfficiency;
                structs.Add(new ParamStruct(mdlParameterNames.max_mc_calc_pwr_elec_eff_map.ToString(), new double[] { max_pwr_elec_eff_map }));

                //mc.init.pwr_eff_prop_index = mc.init.pwr_eff_prop_index*power_corr;
                double[] pwr_eff_prop_index = Simulator.GetParam(mdlParameterNames.mc_init_pwr_eff_prop_index);
                pwr_eff_prop_index = pwr_eff_prop_index.Select(v => v * power_corr).ToArray();
                structs.Add(new ParamStruct(mdlParameterNames.mc_init_pwr_eff_prop_index.ToString(), pwr_eff_prop_index));

            }
            else if (model.TrainOption.Topology == TrainTopology.FuelCell)
            {
                //power_corr = (PRIMEPOWER*1000)/max(fc.init.pwr_hot_index); const: max_fc_pwr
                //power_corr2 = (POWER*1000)/max(mc.init.spd_prop_cont_index.*mc.init.trq_prop_cont_map); const: max_mc_pwr
                double power_corr = (model.PowertrainOption.CanonicalPrimePower * 1000) / MaxFCPower;
                double power_corr2 = ((model.PowertrainOption.CanonicalPrimePower + model.PowertrainOption.CanonicalESSPower) * 1000) / MaxMCPower;

                structs.Add(new ParamStruct("CalculateSimulationParameters_power_corr", new double[] { power_corr }));
                structs.Add(new ParamStruct("CalculateSimulationParameters_power_corr2", new double[] { power_corr2 }));

                //fc.init.h2_hot_map = fc.init.h2_hot_map*power_corr;
                //fc.init.h2_hot_map = fc.init.h2_hot_map./(fc_eff/nanmean(nanmean(fc.calc.eff_pwr_hot_map))); const: mean_fc_eff
                double[] h2_hot_map = Simulator.GetParam(mdlParameterNames.fc_init_h2_hot_map);
                h2_hot_map = h2_hot_map.Select(v => v * power_corr).ToArray();

                h2_hot_map = h2_hot_map.Select(v => v / (model.PowertrainOption.MeanFuelCellEfficiency / MeanFCEfficiency)).ToArray();
                structs.Add(new ParamStruct(mdlParameterNames.fc_init_h2_hot_map.ToString(), h2_hot_map));

                //fc.init.pwr_hot_index = fc.init.pwr_hot_index*power_corr;
                double[] pwr_hot_index = Simulator.GetParam(mdlParameterNames.fc_init_pwr_hot_index);
                pwr_hot_index = pwr_hot_index.Select(v => v * power_corr).ToArray();
                structs.Add(new ParamStruct(mdlParameterNames.fc_init_pwr_hot_index.ToString(), pwr_hot_index));

                //fc.init.pwr_hot_max = fc.init.pwr_hot_max*power_corr;
                double[] pwr_hot_max = Simulator.GetParam(mdlParameterNames.fc_init_pwr_hot_max);
                pwr_hot_max = pwr_hot_max.Select(v => v * power_corr).ToArray();
                structs.Add(new ParamStruct(mdlParameterNames.fc_init_pwr_hot_max.ToString(), pwr_hot_max));

                //mc.init.pwr_eff_prop_index = mc.init.pwr_eff_prop_index*power_corr2;
                double[] pwr_eff_prop_index = Simulator.GetParam(mdlParameterNames.mc_init_pwr_eff_prop_index);
                pwr_eff_prop_index = pwr_eff_prop_index.Select(v => v * power_corr2).ToArray();
                structs.Add(new ParamStruct(mdlParameterNames.mc_init_pwr_eff_prop_index.ToString(), pwr_eff_prop_index));

                //mc.init.pwr_eff_reg_index = mc.init.pwr_eff_reg_index*power_corr2;
                double[] pwr_eff_reg_index = Simulator.GetParam(mdlParameterNames.mc_init_pwr_eff_reg_index);
                pwr_eff_reg_index = pwr_eff_reg_index.Select(v => v * power_corr2).ToArray();
                structs.Add(new ParamStruct(mdlParameterNames.mc_init_pwr_eff_reg_index.ToString(), pwr_eff_reg_index));
            }
            else if (model.TrainOption.Topology == TrainTopology.EngineHybrid)
            {
                structs.AddRange(CalculateEngineParameters(model));
                //power_corr = (PRIMEPOWER*1000)/max(eng.init.spd_max_hot_index.*eng.init.trq_max_hot_map); const: max_eng_power
                //power_corr2 = (POWER*1000)/max(mc.init.spd_prop_cont_index.*mc.init.trq_prop_cont_map); const: max_mc_pwr
                double power_corr = (model.PowertrainOption.CanonicalPrimePower * 1000) / MaxEngPower;
                double power_corr2 = ((model.PowertrainOption.CanonicalPrimePower + model.PowertrainOption.CanonicalESSPower) * 1000) / MaxMCPower;
                structs.Add(new ParamStruct("CalculateSimulationParameters_power_corr", new double[] { power_corr }));
                structs.Add(new ParamStruct("CalculateSimulationParameters_power_corr2", new double[] { power_corr2 }));

                //mc.init.pwr_eff_prop_index = mc.init.pwr_eff_prop_index*power_corr2;
                double[] pwr_eff_prop_index = Simulator.GetParam(mdlParameterNames.mc_init_pwr_eff_prop_index);
                pwr_eff_prop_index = pwr_eff_prop_index.Select(v => v * power_corr2).ToArray();
                structs.Add(new ParamStruct(mdlParameterNames.mc_init_pwr_eff_prop_index.ToString(), pwr_eff_prop_index));

                //ptc.prop.init.eng_pwr_wh_above_turn_on = 0.2*(ESSPOWER*1000);
                double eng_pwr_wh_above_turn_on = 0.2 * model.PowertrainOption.CanonicalESSPower * 1000;
                structs.Add(new ParamStruct(mdlParameterNames.ptc_prop_init_eng_pwr_wh_above_turn_on.ToString(), new double[] { eng_pwr_wh_above_turn_on }));

                //ptc.prop.init.eng_pwr_wh_below_turn_off = 0.1*(ESSPOWER*1000);
                double eng_pwr_wh_below_turn_off = 0.1 * model.PowertrainOption.CanonicalESSPower * 1000;
                structs.Add(new ParamStruct(mdlParameterNames.ptc_prop_init_eng_pwr_wh_below_turn_off.ToString(), new double[] { eng_pwr_wh_below_turn_off }));

            }

            if (ApplicationModel.staticTopology != TrainTopology.Electric)
            {
                double[] apu_pwr_index = Simulator.GetParam(mdlParameterNames.apu_pwr_index);
                double maxAPUPower = apu_pwr_index.Max();
                // apu.fuel_rate = apu.fuel_rate .* (AUXPOWER/max(apu.pwr_index)) ./ (aux_eff/0.5367);
                double[] apu_fuel_rate = Simulator.GetParam(mdlParameterNames.apu_fuel_rate);
                apu_fuel_rate = apu_fuel_rate.Select(v => (v * (model.AuxEngineOption._load / maxAPUPower)) / (model.AuxEngineOption._efficiency / 0.5367)).ToArray();

                structs.Add(new ParamStruct(mdlParameterNames.apu_fuel_rate.ToString(), apu_fuel_rate));

                // apu.pwr_index = apu.pwr_index .* (AUXPOWER/max(apu.pwr_index));
                apu_pwr_index = apu_pwr_index.Select(v => v * (model.AuxEngineOption._load / maxAPUPower)).ToArray();
                structs.Add(new ParamStruct(mdlParameterNames.apu_pwr_index.ToString(), apu_pwr_index));

            }

            return structs;

        }

        private List<ParamStruct> CalculateHybridSimParameters(ParameterModel model)
        {
            List<ParamStruct> structs = new List<ParamStruct>();

            //ess.init.pwr_dmd_map=ess.init.pwr_dmd_map.*ESSPOWER*1000
            double[] ess_pwr_dmd_map = Simulator.GetParam(mdlParameterNames.ess_init_pwr_dmd_map);
            ess_pwr_dmd_map = ess_pwr_dmd_map.Select(v => v * (model.PowertrainOption.CanonicalESSPower) * 1000).ToArray();
            structs.Add(new ParamStruct(mdlParameterNames.ess_init_pwr_dmd_map.ToString(), ess_pwr_dmd_map));

            if (model.TrainOption.StorageSystem == EnergyStorageSystem.Battery)
            {

                //ess.calc.pwr_chg = -ones(size(ess.calc.pwr_chg))*(ESSPOWER*1000);
                double[] pwr_chg = Enumerable.Repeat(
                        -1 * model.PowertrainOption.BatteryPower * 1000,
                        Simulator.GetParam(mdlParameterNames.ess_calc_pwr_chg).Count()
                    ).ToArray();
                //ess.calc.pwr_chg(end) = 0;
                pwr_chg[pwr_chg.Count() - 1] = 0;
                pwr_chg[pwr_chg.Count() - 2] = 0;
                pwr_chg[pwr_chg.Count() - 3] = 0;
                pwr_chg[pwr_chg.Count() - 4] = 0;

                structs.Add(new ParamStruct(mdlParameterNames.ess_calc_pwr_chg.ToString(), pwr_chg));

                //ess.calc.pwr_dis = ones(size(ess.calc.pwr_dis))*(ESSPOWER*1000);
                double[] pwr_dis = Enumerable.Repeat(
                        model.PowertrainOption.BatteryPower * 1000,
                        Simulator.GetParam(mdlParameterNames.ess_calc_pwr_dis).Count()
                    ).ToArray();
                //ess.calc.pwr_dis(1) = 0;
                pwr_dis[0] = 0;
                pwr_dis[1] = 0;
                pwr_dis[2] = 0;

                structs.Add(new ParamStruct(mdlParameterNames.ess_calc_pwr_dis.ToString(), pwr_dis));

                //ess.init.num_module_parallel = ess_energy/ess.init.voltage/ess.init.cap_nom;
                double num_module_parallel = model.PowertrainOption.BatteryEnergy * 1000 / ESSInitVoltage / ESSInitCapNom;
                structs.Add(new ParamStruct(mdlParameterNames.ess_init_num_module_parallel.ToString(), new double[] { num_module_parallel }));

            }
            else if (model.TrainOption.StorageSystem == EnergyStorageSystem.Flywheel)
            {
                //ess.init.fly.max_pwr = ESSPOWER*1000;
                double fly_max_power = model.PowertrainOption.FlywheelPower * 1000;
                structs.Add(new ParamStruct(mdlParameterNames.ess_init_fly_max_pwr.ToString(), new double[] { fly_max_power }));

                //ess.init.num_cell_series = (ESSPOWER*1000)/max(ess.init.fly.power); const: max_fly_pwr
                double num_cell_series = model.PowertrainOption.FlywheelPower * 1000 / MaxFlywheelPower;
                structs.Add(new ParamStruct(mdlParameterNames.ess_init_num_cell_series.ToString(), new double[] { num_cell_series }));

                //ess.init.num_module_parallel = ess_energy*1e6/max(ess.init.fly.kinetic_energy); const: max_fly_energy
                double num_module_parallel = model.PowertrainOption.FlywheelEnergy * 1000000 / MaxFlywheelEnergy;
                structs.Add(new ParamStruct(mdlParameterNames.ess_init_num_module_parallel.ToString(), new double[] { num_module_parallel }));


            }
            else if (model.TrainOption.StorageSystem == EnergyStorageSystem.Ultracapacitive)
            {
                //ess.init.num_cell_series = ess.init.voltage/mean([ess.init.volt_max ess.init.volt_min]); const: mean_cap_voltage
                double num_cell_series = ESSInitVoltage / MeanCapVoltage;
                structs.Add(new ParamStruct(mdlParameterNames.ess_init_num_cell_series.ToString(), new double[] { num_cell_series }));

                //ess.init.num_module_parallel = ess.init.max_pwr/((mean_cap_voltage^2/(4*max_cap_res))*ess.init.num_cell_series); const: max_cap_res
                double num_module_parallel = Simulator.GetParam(mdlParameterNames.ess_init_max_pwr)[0] /
                                                (((MeanCapVoltage * MeanCapVoltage) / (4 * MaxCapRes)) *
                                                num_cell_series);
                structs.Add(new ParamStruct(mdlParameterNames.ess_init_num_module_parallel.ToString(), new double[] { num_module_parallel }));


                //ess.init.cap_map = ones(size(ess.init.cap_map))*(ess.init.num_cell_series/ess.init.num_module_parallel*ess_capacitance);
                double[] cap_map = Enumerable.Repeat(
                    num_cell_series / num_module_parallel * model.PowertrainOption.UltracapCapacitance,
                    Simulator.GetParam(mdlParameterNames.ess_init_cap_map).Count()
                    ).ToArray();
                structs.Add(new ParamStruct(mdlParameterNames.ess_init_cap_map.ToString(), cap_map));
                structs.Add(new ParamStruct(mdlParameterNames.ess_init_cap_map_max.ToString(), new double[] { cap_map.Max() }));
                //ess.init.max_pwr = ESSPOWER*1000;
                double max_pwr = model.PowertrainOption.UltracapPower * 1000;
                structs.Add(new ParamStruct(mdlParameterNames.ess_init_max_pwr.ToString(), new double[] { max_pwr }));

            }

            //ess.init.pwr_dmd_wh_trq_index = ess.init.pwr_dmd_wh_trq_index.*max_mc_trq_index;
            double[] ess_pwr_dmd_wh_trq_index = Simulator.GetParam(mdlParameterNames.ess_init_pwr_dmd_wh_trq_index);
            ess_pwr_dmd_wh_trq_index = ess_pwr_dmd_wh_trq_index.Select(v => v * MaxMCTorqueIndex).ToArray();
            structs.Add(new ParamStruct(mdlParameterNames.ess_init_pwr_dmd_wh_trq_index.ToString(), ess_pwr_dmd_wh_trq_index));
            return structs;
        }

        /// <summary>
        /// Calculate the common scaled parameters between EngineElectric and Engine Hybrid
        /// </summary>
        private List<ParamStruct> CalculateEngineParameters(ParameterModel model)
        {
            List<ParamStruct> structs = new List<ParamStruct>();
            //power_corr = (PRIMEPOWER*1000)/max(eng.init.spd_max_hot_index.*eng.init.trq_max_hot_map); const: max_eng_power
            double power_corr = (model.PowertrainOption.CanonicalPrimePower * 1000) / MaxEngPower;
            structs.Add(new ParamStruct("CalculateEngineParameters_power_corr", new double[] { power_corr }));

            //eng.calc.pwr_eff_pwr_hot_index = eng.calc.pwr_eff_pwr_hot_index*power_corr;
            double[] pwr_eff_pwr_hot_index = Simulator.GetParam(mdlParameterNames.eng_calc_pwr_eff_pwr_hot_index);
            pwr_eff_pwr_hot_index = pwr_eff_pwr_hot_index.Select(v => v * power_corr).ToArray();
            structs.Add(new ParamStruct(
                    mdlParameterNames.eng_calc_pwr_eff_pwr_hot_index.ToString(),
                    pwr_eff_pwr_hot_index
                ));

            //eng.init.fuel_hot_map = eng.init.fuel_hot_map*power_corr;
            //eng.init.fuel_hot_map = eng.init.fuel_hot_map./(eng_eff/nanmean(nanmean(eng.calc.eff_hot_map))); const: mean_eng_eff
            double[] fuel_hot_map = new double[] { };
            double fuelFactor = 1;
            if (ApplicationModel.staticFuelType == EngineFuelType.CNG)
            {
                fuel_hot_map = Simulator.GetParam(mdlParameterNames.eng_init_fuel_hot_map_cng);
                fuelFactor = 0.818456;
            }
            else if (ApplicationModel.staticFuelType == EngineFuelType.Gas)
            {
                fuel_hot_map = Simulator.GetParam(mdlParameterNames.eng_init_fuel_hot_map_gasoline);
                fuelFactor = 0.980428;
            }
            else if (ApplicationModel.staticFuelType == EngineFuelType.LNG)
            {
                fuel_hot_map = Simulator.GetParam(mdlParameterNames.eng_init_fuel_hot_map_lng);
                fuelFactor = 0.818456;
            }
            else
            { //diesel
                fuel_hot_map = Simulator.GetParam(mdlParameterNames.eng_init_fuel_hot_map_diesel);
            }

            double[] fuel_to_CO2eq_factor = Simulator.GetParam(mdlParameterNames.fuel_to_CO2eq_factor).Select(v => v * fuelFactor).ToArray();
            structs.Add(new ParamStruct(
                    mdlParameterNames.fuel_to_CO2eq_factor.ToString(),
                    fuel_to_CO2eq_factor
                ));


            fuel_hot_map = fuel_hot_map.Select(v => v * power_corr).ToArray();
            fuel_hot_map = fuel_hot_map.Select(v => v / (model.PowertrainOption.MeanEngineEfficiency / MeanEngineEfficiency)).ToArray();
            structs.Add(new ParamStruct(
                    mdlParameterNames.eng_init_fuel_hot_map.ToString(),
                    fuel_hot_map
                ));


            //eng.init.trq_max_hot_map = eng.init.trq_max_hot_map*power_corr;
            double[] trq_max_hot_map = Simulator.GetParam(mdlParameterNames.eng_init_trq_max_hot_map);
            trq_max_hot_map = trq_max_hot_map.Select(v => v * power_corr).ToArray();
            structs.Add(new ParamStruct(
                    mdlParameterNames.eng_init_trq_max_hot_map.ToString(),
                    trq_max_hot_map
                ));

            //gc.init.eff_inverse_trq_map = gc.init.eff_inverse_trq_map*mean(mean(gc.init.eff_trq_map))/gc_eff;const: mean_gc_eff
            double[] eff_inverse_trq_map = Simulator.GetParam(mdlParameterNames.gc_init_eff_inverse_trq_map);
            eff_inverse_trq_map = eff_inverse_trq_map.Select(v => v * MeanGCEfficiency / model.PowertrainOption.GeneratorEfficiency).ToArray();
            structs.Add(new ParamStruct(
                    mdlParameterNames.gc_init_eff_inverse_trq_map.ToString(),
                    eff_inverse_trq_map
                ));

            //gc.init.eff_trq_map = gc.init.eff_trq_map*gc_eff/mean(mean(gc.init.eff_trq_map));const: mean_gc_eff
            double[] eff_trq_map = Simulator.GetParam(mdlParameterNames.gc_init_eff_trq_map);
            eff_trq_map = eff_trq_map.Select(v => v * model.PowertrainOption.GeneratorEfficiency / MeanGCEfficiency).ToArray();
            structs.Add(new ParamStruct(
                    mdlParameterNames.gc_init_eff_trq_map.ToString(),
                    eff_trq_map
                ));


            //gc.init.pwr_eff_prop_index = gc.init.pwr_eff_prop_index*power_corr;
            double[] pwr_eff_prop_index = Simulator.GetParam(mdlParameterNames.gc_init_pwr_eff_prop_index);
            pwr_eff_prop_index = pwr_eff_prop_index.Select(v => v * power_corr).ToArray();
            structs.Add(new ParamStruct(
                    mdlParameterNames.gc_init_pwr_eff_prop_index.ToString(),
                    pwr_eff_prop_index
                ));

            //gc.init.trq_eff_index = gc.init.trq_eff_index*power_corr;
            double[] trq_eff_index = Simulator.GetParam(mdlParameterNames.gc_init_trq_eff_index);
            trq_eff_index = trq_eff_index.Select(v => v * power_corr).ToArray();
            structs.Add(new ParamStruct(
                    mdlParameterNames.gc_init_trq_eff_index.ToString(),
                    trq_eff_index
                ));

            //gc.init.trq_prop_cont_map = gc.init.trq_prop_cont_map*power_corr;
            double[] trq_prop_cont_map = Simulator.GetParam(mdlParameterNames.gc_init_trq_prop_cont_map);
            trq_prop_cont_map = trq_prop_cont_map.Select(v => v * power_corr).ToArray();
            structs.Add(new ParamStruct(
                    mdlParameterNames.gc_init_trq_prop_cont_map.ToString(),
                    trq_prop_cont_map
                ));

            //gc.init.trq_reg_cont_map = gc.init.trq_reg_cont_map*power_corr;
            double[] trq_reg_cont_map = Simulator.GetParam(mdlParameterNames.gc_init_trq_reg_cont_map);
            trq_reg_cont_map = trq_reg_cont_map.Select(v => v * power_corr).ToArray();
            structs.Add(new ParamStruct(
                    mdlParameterNames.gc_init_trq_reg_cont_map.ToString(),
                    trq_reg_cont_map
                ));


            return structs;
        }

    }
}
