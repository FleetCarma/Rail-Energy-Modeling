#pragma once

//This is a collection of enums used by the trainSImulator
namespace TrainSimulatorCLI
{
    //Inputs into the model (drive cycle)
    public enum class mdlInputNames
    {
        TIME,
        FORWARDVELOCITY,
        GRADE,
        KEYON,
        AUXPOWERLOAD
    };

    //Available models.
    //The name of the enum must be reflected in the name of the model executable as well as the associated _params and _consts files
    //Ex:
    //ENGINE_ELECTRIC --> ENGINE_ELECTRIC.exe, ENGINE_ELECTRIC_params, ENGINE_ELECTRIC_consts
    public enum class mdlSimlinkName
    {
        ENGINE_ELECTRIC,
        ELECTRIC,
        ENGINE_HYBRID_BATTERY,
        ENGINE_HYBRID_FLYWHEEL,
        ENGINE_HYBRID_ULTRACAPACITOR,
        FUEL_CELL_HYBRID_BATTERY,
        FUEL_CELL_HYBRID_FLYWHEEL,
        FUEL_CELL_HYBRID_ULTRACAPACITOR
    };

    //The identifiers for the outputs from the model
    public enum class mdlOutputNames
    {
        sim_time_s,//Time [s]
        sim_vehicle_speed_mps, //was:sim_vehicle_speed_mps,//Vehicle Speed [m/s]
        sim_engine_speed_radps,//Engine Speed [rad/s]
        sim_engine_torque_N,//Engine Torque [N]
        sim_engine_power_W,//Engine Mechanical Power [W]
        sim_engine_fuel_rate_kgps,//Engine Fuel Consumption Rate [kg/s]
        sim_engine_fuel_cumulative_kg,//Cumulative Engine Fuel Consumption [kg]
        sim_engine_GHG_emissions_rate_kgps,//Engine GHG Emissions Rate [kg/s]
        sim_engine_on_bool,//Engine On/Off State [Boolean]
        sim_generator_mechanical_power_W,//Generator Mechanical Power [W]
        sim_generator_electrical_power_W,//Generator Electrical Power [W]
        sim_motor_electrical_power_W,//Motor Electrical Power [W]
        sim_motor_mechanical_power_W,//Motor Mechanical Power [W]
        sim_tractive_power_W,//Tractive Power [W]
        sim_mechanical_accessories_power_W,//Mechanical Accessories Power [W]
        sim_electrical_accessories_power_W,//Electrical Accessories Power [W]
        sim_AESS_on_bool,//AESS Functionality [Boolean]
        sim_apu_power_W,//APU Power [W]
        sim_apu_fuel_rate_kgps,//APU Fuel Consumption Rate [kg/s]
        sim_apu_fuel_cumulative_kg,//Cumulative APU Fuel Consumption [kg]
        sim_apu_GHG_emissions_rate_kgps,//APU GHG Emissions Rate [kg/s]
        sim_locomotive_GHG_emissions_rate_kgps,//Locomotive GHG Emissions Rate [kg/s]
        sim_locomotive_GHG_emissions_cumulative_kg,//Cumulative Locomotive GHG Emissions [kg]
        sim_ess_power_W, //ESS Power [W]
        sim_ess_state_of_charge_percent, //ESS State of Charge [%]
        sim_fuelcell_GHG_emissions_rate_kgps, //Fuel Cell GHG Emissions Rate [kg/s]
        sim_fuelcell_h2_cumulative_kg, //Cumulative Fuel Cell H2 Consumption [kg]
        sim_fuelcell_h2_rate_kgps, //Fuel Cell H2 Consumption Rate [kg/s]
        sim_fuelcell_on_bool, //Fuel Cell On\Off State [Boolean]
        sim_fuelcell_power_W, //Fuel Cell Power [W]
        sim_grid_energy_Wh, //Grid Energy [Wh]
        sim_grid_power_W, //Grid Power [W]
    };

    //The identifiers for the parameters of the model
    public enum class mdlParameterNames
    {
        //Tunable params, aka parameters that the user enters
        PRIMEPOWER, //Power rating of engine
        ESSPOWER,//Power of ESS
        AESS, //Enables or disables AESS
        AUXPOWER, //Power of auxiliary engine if present
        veh_init_mass, //Locomotive mass
        veh_init_num_cars, //Number of cars
        veh_init_mass_cars, //Mass of cars
        veh_init_coeff_drag, //coefficient of drag
        veh_init_frontal_area, //frontal area
        accelec_init_pwr, //electric accessory power
        accmech_init_pwr, //mechanical accessory power
        eng_eff, //engine efficiency
        gc_eff, //generator efficiency
        mc_eff, //motor efficieny
        aux_eff, //aux efficiency?
        ess_energy, //ess energy
        ess_capacitance, //ess capacitance
        fd_init_ratio, //final drive gear ratio
        fd_init_eff, //final drive efficiency
        wh_init_coeff_roll1, //rolling resistance 1
        wh_init_coeff_roll2, //rolling resistance 2
        wh_init_radius, //wheel radius
        top_speed, //top speed
        //scaled params (these are functions of the tunable and constant params)
        drv_init_kp,
        dynamic_brake_brake_effort,
        dynamic_brake_mc_spd,
        dynamic_brake_trac_effort,
        eng_calc_pwr_eff_pwr_hot_index,
        eng_init_fuel_hot_map,
        eng_init_fuel_hot_map_diesel,
        eng_init_fuel_hot_map_cng,
        eng_init_fuel_hot_map_lng,
        eng_init_fuel_hot_map_gasoline,
        fuel_to_CO2eq_factor,
        eng_init_trq_max_hot_map,
        ess_calc_pwr_chg,
        ess_calc_pwr_dis,
        ess_init_cap_map,
        ess_init_fly_max_pwr,
        ess_init_max_pwr,
        ess_init_num_cell_series,
        ess_init_num_module_parallel,
        ess_init_cap_map_max,
        ess_init_volt_nom,
        fc_init_h2_hot_map,
        fc_init_pwr_hot_index,
        fc_init_pwr_hot_max,
        gc_init_eff_inverse_trq_map,
        gc_init_eff_trq_map,
        gc_init_pwr_eff_prop_index,
        gc_init_spd_eff_index,
        gc_init_trq_eff_index,
        gc_init_trq_prop_cont_map,
        gc_init_trq_reg_cont_map,
        max_eng_init_trq_fuel_hot_index,
        max_mc_calc_pwr_elec_eff_map,
        max_motor_spd,
        mc_init_eff_inverse_trq_map,
        mc_init_eff_trq_map,
        mc_init_pwr_eff_prop_index,
        mc_init_pwr_eff_reg_index,
        mc_init_spd_eff_index,
        mc_init_spd_prop_cont_index,
        mc_init_trq_eff_index,
        mc_init_trq_prop_cont_map,
        mc_init_trq_reg_cont_map,
        min_eng_init_trq_fuel_hot_index,
        power_corr,
        power_corr2,
        ptc_prop_init_eng_pwr_wh_above_turn_on,
        ptc_prop_init_eng_pwr_wh_below_turn_off,
        ptc_prop_init_veh_spd_wh_trq_max_index,
        reg_corr,
        trq_corr,
        trq_max,
        wh_init_trq_brake_max,
        apu_fuel_rate,
        apu_pwr_index,
        ess_init_pwr_dmd_map,
        ess_init_pwr_dmd_wh_trq_index
    };

    //The identifiers for the cosntants of the model
    //These are used to calculate the scaled parameters
    public enum class mdlConstantNames
    {
        max_apu_pwr_index,
        max_cap_capacitance,
        max_cap_res,
        max_dyn_brk_mc_spd,
        max_eng_power,
        max_eng_trq_fuel_index,
        max_fc_pwr,
        max_fly_energy,
        max_fly_pwr,
        max_mc_pwr,
        max_mc_spd_index,
        max_mc_trq_index,
        max_mc_eff,
        max_ptc_ess_pwr_map,
        max_ptc_wh_trq_index,
        mean_cap_voltage,
        mean_eng_eff,
        mean_fc_eff,
        mean_gc_eff,
        mean_mc_eff,
        min_eng_trq_fuel_index,
        min_mc_trq_index,
        veh_max_accel,
        ess_init_voltage,
        ess_init_cap_nom
    };
}