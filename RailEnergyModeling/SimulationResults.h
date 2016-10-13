#pragma once

namespace TrainSimulatorCLI
{
	public value struct SimulationResults
	{
		//Mechanical accessory torque load
		double accmech_trq;

		//Auxiliary Power Unit (APU) instantaneous fuel burn rate
		double apu_fuel_rate;

		//Bus containing all simulation output vectors, including those not included in this list
		array<double>^ bus;

		//Engine ‘key-on’ signal from the driver controller
		double drv_key_on_dmd;

		//Engine torque demand from the driver controller
		double drv_trq_dmd;

		//Engine command normalized from a scale of 0 to 1
		double eng_cmd;

		//Cumulative fuel use of all engines
		double eng_fuel_cum;

		//Instantaneous fuel burn rate of the prime mover
		double eng_fuel_rate;

		//Engine on/off state
		double eng_on;

		//Engine on/off state, duplicate of driver key on
		double eng_on_dmd_stateflow;

		//Prime mover engine speed
		double eng_spd;

		//Check if engine speed is greater than starter speed
		double eng_stat;

		//Prime mover engine torque
		double eng_trq;

		//Prime mover engine torque demanded by powertrain controller
		double eng_trq_dmd;

		//Prime mover engine torque
		double eng_trq_out;

		//Instantaneous total fuel burn rate
		double engapu_fuel_cum;

		//Generator demand from the controller
		double gc_cmd;

		//Generator output current
		double gc_curr_out;

		//Generator input rotational speed
		double gc_spd_in;

		//Generator and engine speed difference to meet propel demands
		double gc_spd_req_diff;

		//Generator torque demand
		double gc_trq_dmd;

		//Generator torque demand, actual
		double gc_trq_dmd_calc;

		//Generator input torque
		double gc_trq_in;

		double gc_trq_max_regen_calc;

		double gc_trq_max_regen_calc1;

		//Generator torque, transient
		double gc_trq_trs;

		double mc_temp_coeff;

		double num;

		//Engine on signal at the engine controller
		double ptc_eng_on_dmd;

		double ptc_eng_trq_hot_min_cstr;

		double ptc_eng_trq_hot_mx_cstr;

		//Engine torque, transient
		double ptc_eng_trq_trs;

		double ptc_gc_trq_mx_pro_cstr;

		//Simulation time vector
		double time;

		double veh_eng_pwr_max_min1;

		double veh_eng_pwr_max_min2;

		//Power demand at the engine
		double veh_pwr_dmd_at_eng;

		double veh_pwr_min1;

		double veh_pwr_min2;

		//Vehicle speed
		double veh_spd;

		double rt_tout;
	};
}