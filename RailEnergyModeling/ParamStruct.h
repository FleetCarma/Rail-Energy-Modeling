#pragma once
namespace TrainSimulatorCLI
{
	//struct used to store an identifier of a parameter and a list of values which will be fed into the _params file for simulation
	public value struct ParamStruct
	{
		System::String^ Identifier;
		array<double>^ Values;
		ParamStruct(System::String^ id, array<double>^ vals) : Identifier(id), Values(vals)
		{
		}
	};
}