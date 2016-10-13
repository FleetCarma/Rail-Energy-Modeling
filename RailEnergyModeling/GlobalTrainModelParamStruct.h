#pragma once

#include "TrainModelParamStruct.h"
namespace TrainSimulatorCLI
{
	public value struct GlobalTrainModelParamStruct
	{
		static const int CHECKSUM_SIZE = 4;

		TrainModelParamStruct^ SubParam;
		array<double>^ ModelCheckSum;

		GlobalTrainModelParamStruct(TrainModelParamStruct %param, array<double>^ checksum)
		{
			SubParam = param;
			if(checksum->Length >= CHECKSUM_SIZE){
				ModelCheckSum = gcnew array<double>(CHECKSUM_SIZE);
				ModelCheckSum[0] = checksum[0];
				ModelCheckSum[1] = checksum[1];
				ModelCheckSum[2] = checksum[2];
				ModelCheckSum[3] = checksum[3];
			}
		}
	};
}