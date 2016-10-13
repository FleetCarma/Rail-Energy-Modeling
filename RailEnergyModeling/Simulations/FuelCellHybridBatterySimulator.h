#pragma once
#include "TrainSimulator.h"

namespace TrainSimulatorCLI {

    //Child class of TrainSimulator, representing a train with an EngineElectric topology
    public ref class FuelCellHybridBatterySimulator : public TrainSimulator {
        
    protected:
        //Configure the inputs which will be put into the model,
        //and what outputs this simulator is interested in
        virtual void initInputsOutputsParams() override
        {
            createInputOrder(gcnew array<mdlInputNames>
            {
                mdlInputNames::TIME,
                mdlInputNames::FORWARDVELOCITY,
                mdlInputNames::GRADE,
                mdlInputNames::KEYON,
                mdlInputNames::AUXPOWERLOAD
            });
            createOutputOrder(gcnew array<mdlOutputNames>
            {
                mdlOutputNames::sim_time_s,//Time [s]
                mdlOutputNames::sim_vehicle_speed_mps, //was:sim_vehicle_speed_mps,//Vehicle Speed [m/s]
                mdlOutputNames::sim_engine_speed_radps,//Engine Speed [rad/s]
                mdlOutputNames::sim_engine_torque_N,//Engine Torque [N]
                mdlOutputNames::sim_engine_power_W,//Engine Mechanical Power [W]
                mdlOutputNames::sim_engine_fuel_rate_kgps,//Engine Fuel Consumption Rate [kg/s]
                mdlOutputNames::sim_engine_fuel_cumulative_kg,//Cumulative Engine Fuel Consumption [kg]
                mdlOutputNames::sim_engine_GHG_emissions_rate_kgps,//Engine GHG Emissions Rate [kg/s]
                mdlOutputNames::sim_engine_on_bool,//Engine On/Off State [Boolean]
                mdlOutputNames::sim_generator_mechanical_power_W,//Generator Mechanical Power [W]
                mdlOutputNames::sim_generator_electrical_power_W,//Generator Electrical Power [W]
                mdlOutputNames::sim_motor_electrical_power_W,//Motor Electrical Power [W]
                mdlOutputNames::sim_motor_mechanical_power_W,//Motor Mechanical Power [W]
                mdlOutputNames::sim_tractive_power_W,//Tractive Power [W]
                mdlOutputNames::sim_mechanical_accessories_power_W,//Mechanical Accessories Power [W]
                mdlOutputNames::sim_electrical_accessories_power_W,//Electrical Accessories Power [W]
                mdlOutputNames::sim_AESS_on_bool,//AESS Functionality [Boolean]
                mdlOutputNames::sim_apu_power_W,//APU Power [W]
                mdlOutputNames::sim_apu_fuel_rate_kgps,//APU Fuel Consumption Rate [kg/s]
                mdlOutputNames::sim_apu_fuel_cumulative_kg,//Cumulative APU Fuel Consumption [kg]
                mdlOutputNames::sim_apu_GHG_emissions_rate_kgps,//APU GHG Emissions Rate [kg/s]
                mdlOutputNames::sim_locomotive_GHG_emissions_rate_kgps,//Locomotive GHG Emissions Rate [kg/s]
                mdlOutputNames::sim_locomotive_GHG_emissions_cumulative_kg,//Cumulative Locomotive GHG Emissions [kg]
                mdlOutputNames::sim_ess_power_W, //ESS Power [W]
                mdlOutputNames::sim_ess_state_of_charge_percent, //ESS State of Charge [%]
                mdlOutputNames::sim_fuelcell_GHG_emissions_rate_kgps, //Fuel Cell GHG Emissions Rate [kg/s]
                mdlOutputNames::sim_fuelcell_h2_cumulative_kg, //Cumulative Fuel Cell H2 Consumption [kg]
                mdlOutputNames::sim_fuelcell_h2_rate_kgps, //Fuel Cell H2 Consumption Rate [kg/s]
                mdlOutputNames::sim_fuelcell_on_bool, //Fuel Cell On\Off State [Boolean]
                mdlOutputNames::sim_fuelcell_power_W, //Fuel Cell Power [W]
                mdlOutputNames::sim_grid_energy_Wh, //Grid Energy [Wh]
                mdlOutputNames::sim_grid_power_W, //Grid Power [W]
            });
        }

    public:
        FuelCellHybridBatterySimulator(mdlSimlinkName name, String^ modelDirectory)
            : TrainSimulator(name, modelDirectory)
        {
        }

        //method used for calculating parameters in the _params file which are functions of the inputs of this method and values within the  _consts file
        List<ParamStruct>^ CalculateParameters(double maxFuelBurnRate, double maxEngineSpeed, double maxEngineTorque){
            List<ParamStruct>^ structs = gcnew List<ParamStruct>();
            
            
            return structs;
        }
    };

}