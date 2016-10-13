#pragma once

#include "ParamStruct.h"

namespace TrainSimulatorCLI
{
	using namespace System;
	public value struct TrainModelParamStruct
	{
		String^ DataTypeName;
		double DataTypeId;
		double Complex;
		double dtTransIdx;
		array<double>^ Values;
		array<ParamStruct>^ ParamStructs;
	};
}