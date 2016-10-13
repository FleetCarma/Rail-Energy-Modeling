#include <msclr\marshal.h>
#include "TrainSimulator.h"
#include "Simulations\EngineElectricSimulator.h"
#include "Simulations\ElectricSimulator.h"
#include "Simulations\EngineHybridBatterySimulator.h"
#include "Simulations\EngineHybridFlywheelSimulator.h"
#include "Simulations\EngineHybridUltracapacitorSimulator.h"
#include "Simulations\FuelCellHybridBatterySimulator.h"
#include "Simulations\FuelCellHybridFlywheelSimulator.h"
#include "Simulations\FuelCellHybridUltracapacitorSimulator.h"
#include <cstring>
//For some reason this has to be done in order to avoid conflict with the preprocessor definition in the windows header file?
#undef GetTempPath

using namespace System;
using namespace System::IO;
using namespace System::Collections::Generic;
using namespace System::Globalization;
using namespace msclr::interop;

namespace TrainSimulatorCLI
{
	TrainSimulator::TrainSimulator(mdlSimlinkName simlinkName, String^ modelDirectory)
	{
		mdlName = simlinkName;
		this->modelDirectory = modelDirectory;

		// Initialize the input dictionaries.
		this->mdlInputs = gcnew Dictionary< mdlInputNames, array<double>^ >;
		this->mdlOutputs = gcnew Dictionary< mdlOutputNames, array<double>^ >;

		// Let the concrete class set up the inputOrder, outputOrder and parameters.
		initInputsOutputsParams();

		// Set up the input and output dictionaries. 
		for each ( mdlInputNames s in this->inputsOrder){
			mdlInputs[s] = nullptr;
		}

		for each ( mdlOutputNames s in this->outputsOrder){
			mdlOutputs[s] = nullptr;
		}

		cleanupOutput = true;

		// Create temporary input and output files.
		String^ tmpFile = Path::Combine(Path::GetTempPath(), Path::GetRandomFileName());

		this->inFile = tmpFile + ".mat";
		this->inFileC = (char*)System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(this->inFile).ToPointer();

		this->outFile = tmpFile + "_out.mat";
		this->outFileC = (char*)System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(this->outFile).ToPointer();

		this->paramFile = tmpFile + "_params.mat";
		this->paramFileC = (char*)System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(this->paramFile).ToPointer();
	}

	TrainSimulator::~TrainSimulator(){
		// Delete temp files.
		//Note: it is the caller's responsibility to delete the output file
		try
		{
			if (File::Exists(this->inFile))
			{
				File::Delete(this->inFile);
			}
			if (File::Exists(this->paramFile))
			{
				File::Delete(this->paramFile);
			}
		}
		catch(Exception^)
		{
		}

		// Free C style strings.
		Runtime::InteropServices::Marshal::FreeHGlobal(IntPtr(this->inFileC));
		Runtime::InteropServices::Marshal::FreeHGlobal(IntPtr(this->outFileC));
		Runtime::InteropServices::Marshal::FreeHGlobal(IntPtr(this->paramFileC));
	}

	// Note: Caller must hold the matlabLock.
	array<double>^ TrainSimulator::getRTPparam(mxArray * rtpParamStruct,char * paramName){

		mxArray* parameters = mxGetField(rtpParamStruct,0,"parameters");
		mxArray* paramMap = mxGetField(parameters,0,"map");

		char strBuf[256];
		int i,numElem = (int)mxGetNumberOfElements(paramMap);
		for( i = 0; i < numElem; i++){
			mxArray* mfParamName = mxGetField(paramMap,i,"Identifier");
			mxGetString(mfParamName,strBuf,256);
			if( !strcmp(strBuf,paramName) )    break;
		}
		
		if( i == numElem){
			throw gcnew Exception(String::Format("Couldn't find variable \"{0}\" in the parameter file", gcnew String(paramName)));
		}

		//get the location of the values in the values array
		mxArray* mfParamIndices = mxGetField(paramMap,i,"ValueIndices");
		mwSize index[] = {0,0};
		int valueStart = mxGetPr(mfParamIndices)[mxCalcSingleSubscript(mfParamIndices, 2, index)];
		index[1] = 1;
		int valueEnd = mxGetPr(mfParamIndices)[mxCalcSingleSubscript(mfParamIndices, 2, index)];
		int length = (valueEnd - valueStart) + 1;

		if( i == numElem){
			throw gcnew Exception(String::Format("Couldn't find variable \"{0}\" in the parameter file", gcnew String(paramName)));
		} else {
			mxArray* values = mxGetField(parameters,0,"values");
			double *pval = mxGetPr(values);
			array<double>^ vals = gcnew array<double>(length);
			for(int j = 0; j < length; j++){
				int offset = j + valueStart - 1;
				vals[j] = *(pval + offset);
			}
			return vals;
		}
		mxDestroyArray(parameters);
		mxDestroyArray(paramMap);
		mxDestroyArray(mfParamIndices);
		return nullptr;
	}

	// Note: Caller must hold the matlabLock.
	int TrainSimulator::setRTPparam(mxArray * rtpParamStruct,char * paramName,array<double>^ paramValues){
		
		mxArray* parameters = mxGetField(rtpParamStruct,0,"parameters");
		mxArray* paramMap = mxGetField(parameters,0,"map");

		char strBuf[256];
		int i,numElem = (int)mxGetNumberOfElements(paramMap);
		for( i = 0; i < numElem;i++){
			mxArray* mfParamName = mxGetField(paramMap,i,"Identifier");
			mxGetString(mfParamName,strBuf,256);
			if( !strcmp(strBuf,paramName) )    break;
		}
		
		if( i >= numElem){
			printf("SETRTParam: Couldn't find the variable name in the parameter file:");
			printf(paramName);
			printf("\n\r");
			return 1;
		}

		//validate that the number of parameter values we're adding in equal the size of the ValueIndices entry of the map
		mxArray* mfParamIndices = mxGetField(paramMap,i,"ValueIndices");
		mwSize index[] = {0,0};
		int valueStart = mxGetPr(mfParamIndices)[mxCalcSingleSubscript(mfParamIndices, 2, index)];
		index[1] = 1;
		int valueEnd = mxGetPr(mfParamIndices)[mxCalcSingleSubscript(mfParamIndices, 2, index)];
		if( (valueEnd - (valueStart-1)) != paramValues->Length )
		{
			printf("Parameter variable lengths do not match");
			return 1;
		}

		for(int j = valueStart; j <= valueEnd; j++)
		{
			mxArray * values = mxGetField(parameters,0,"values");
			double *pval = mxGetPr(values);
			*(pval + j - 1) = paramValues[j - valueStart];
		}

		return 0;
	}

	
	void TrainSimulator::Simulate(array<ParamStruct>^ paramStructs)
	{
		MATFile * pmat = NULL;

		// Locate the parameters file.
		String^ mdlHome = Path::Combine(modelDirectory, this->mdlName.ToString());
		String^ paramFileLoc = Path::Combine(mdlHome, String::Concat(this->mdlName,"_params.mat"));
		char * paramFileLocC = (char *)System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(paramFileLoc).ToPointer();

		Threading::Monitor::Enter(matlabLock);
		try
		{
			pmat = matOpen(this->inFileC, "w");
			if (NULL == pmat)
			{
				throw gcnew Exception(String::Format("Unable to open {0} for writing. Unable to create temp file for simulation inputs.", this->inFile));
			}

			// Create an mxArray to be written to the file large enough to hold the inputs. Must free.
			mxArray * inputData = mxCreateDoubleMatrix(mdlInputs[mdlInputNames::TIME]->Length, inputsOrder->Length, mxREAL);

			// Write the input to the array.
			double * matArr = mxGetPr(inputData);
			int matIndex = 0;
			for each (mdlInputNames paramName in this->inputsOrder){
				array<double>^ values = mdlInputs[paramName];
				for each (double val in values){
					*(matArr + matIndex) = val;
					matIndex++;
				}
			}

			// Put the array into the file. 
			matPutVariable(pmat, "inputs", inputData);

			// Cleanup array and close input file.
			mxDestroyArray(inputData);
			matClose(pmat);
			pmat = NULL;

			// Open the parameters file for reading.
			pmat = matOpen(paramFileLocC, "r");
			if (NULL == pmat)
			{
				throw gcnew Exception(String::Format("Unable to open {0} for reading. Cannot update tuneable parameters.", paramFileLoc));
			}

			mxArray * parametersStruct = matGetVariable(pmat,"params");
			if (NULL == parametersStruct)
			{
				matClose(pmat);
				throw gcnew Exception("Cannot find 'params' variable in parameter file.");
			}

			// Close the parameters file.
			matClose(pmat);
			pmat = NULL;

			for each(ParamStruct p in paramStructs)
			{
				char * sC = (char *)Runtime::InteropServices::Marshal::StringToHGlobalAnsi(p.Identifier).ToPointer();
				this->setRTPparam(parametersStruct, sC, p.Values);
				Runtime::InteropServices::Marshal::FreeHGlobal(IntPtr(sC));
			}

			// Open the temporary params file.
			pmat = matOpen(paramFileC, "w");
			if (NULL == pmat)
			{
				throw gcnew Exception(String::Format("Unable to open {0} for writing. Cannot update tuneable parameters.", paramFile));
			}

			// Write the struct to the temporary params file.
			matPutVariable(pmat, "params", parametersStruct);
			mxDestroyArray(parametersStruct);

			// Close the parameters file.
			matClose(pmat);
			pmat = NULL;
		}
		finally
		{
			Threading::Monitor::Exit(matlabLock);
		}

		double maxTime = 0;
		if( this->mdlInputs[mdlInputNames::TIME]->Length > 0 ){
			maxTime = this->mdlInputs[ mdlInputNames::TIME][ this->mdlInputs[ mdlInputNames::TIME]->Length - 1 ];
		}

		String^ modelArgs =  String::Concat(" -tf ", maxTime.ToString()," -i ", this->inFile, " -o ", this->outFile, " -p ", paramFile);  

		//Run the program
		Diagnostics::Process ^ prog = gcnew Diagnostics::Process;
		prog->StartInfo->UseShellExecute = false;
		prog->StartInfo->RedirectStandardOutput = true;
		prog->StartInfo->RedirectStandardError = true;
		prog->StartInfo->FileName = Path::Combine( modelDirectory,String::Concat(this->mdlName,".exe") );
		prog->StartInfo->CreateNoWindow = true;
		prog->StartInfo->WorkingDirectory = Path::Combine(modelDirectory, "..");
		prog->StartInfo->Arguments = modelArgs;
		prog->Start();
		String^ outputlog = prog->StandardOutput->ReadToEnd();
		String^ errorLog = prog->StandardError->ReadToEnd();
		prog->WaitForExit();

		Threading::Monitor::Enter(matlabLock);
		try
		{
			// Get the simulation output file.
			pmat = matOpen(this->outFileC, "r");
			if (NULL == pmat)
			{
				throw gcnew Exception(String::Format(
					"Unable to open {0} for reading. Cannot read simulation output file.\nSTDERR:\n{1}",
					this->outFile,
					errorLog));
			}

			for each (mdlOutputNames s in this->outputsOrder)
			{
				char * sC = (char *)Runtime::InteropServices::Marshal::StringToHGlobalAnsi(s.ToString()).ToPointer();
				mxArray* p = matGetVariable(pmat, sC);
				if (NULL == p)
				{
					matClose(pmat);
					throw gcnew Exception(String::Format("Unable to find output variable: {0}", s.ToString()));
				}

				size_t sz = mxGetM(p);
				double * arrStart = mxGetPr(p);
				this->mdlOutputs[s] = gcnew array<double>(sz);
				for (size_t i = 0 ; i < sz; i++)
				{
					mdlOutputs[s][i] = *(arrStart + i);
				}
				mxDestroyArray(p);
				Runtime::InteropServices::Marshal::FreeHGlobal(IntPtr(sC));
			}

			Runtime::InteropServices::Marshal::FreeHGlobal(IntPtr(paramFileLocC));

			// Close the simulation output file.
			matClose(pmat);
			pmat = NULL;
		}
		finally
		{
			Threading::Monitor::Exit(matlabLock);
		}
	}

	void TrainSimulator::SetInput(mdlInputNames inputName, array<double>^ inputVals ){
		if( !this->mdlInputs->ContainsKey(inputName) ) return;
		this->mdlInputs[inputName] = inputVals;
	}

	array<double>^ TrainSimulator::GetOutput(mdlOutputNames outputName)
	{
		if (!mdlOutputs->ContainsKey(outputName))
		{
			// We do not have this output.
			return nullptr;
		}
		return mdlOutputs[outputName];
	}

	double TrainSimulator::GetConst(mdlConstantNames constName)
	{
		return TrainSimulator::GetConst(constName.ToString());
	}

	double TrainSimulator::GetConst(String^ paramName)
	{
		char* constNameC = (char*)System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(paramName).ToPointer();
		MATFile * pmat = NULL;

		// Locate the parameters file.
		String^ mdlHome = Path::Combine(modelDirectory, this->mdlName.ToString());
		String^ constFileLoc = Path::Combine(mdlHome, String::Concat(this->mdlName,"_consts.mat"));
		char * constFileLocC = (char *)System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(constFileLoc).ToPointer();
		
		Threading::Monitor::Enter(matlabLock);
		try{
			// Open the parameters file for reading.
			pmat = matOpen(constFileLocC, "r");
			if (NULL == pmat)
			{
				throw gcnew Exception(String::Format("Unable to open {0} for reading. Cannot update tuneable parameters.", constFileLoc));
			}
			
			mxArray * constVariable = matGetVariable(pmat,constNameC);
			
			// Close the constants file.
			matClose(pmat);
			pmat = NULL;

			if (NULL == constVariable)
			{
				throw gcnew Exception(String::Format("Couldn't find variable {0} in the constants file", paramName));
			}
			double value = mxGetPr(constVariable)[0];
			mxDestroyArray(constVariable);
			return value;
		}
		finally {
			Threading::Monitor::Exit(matlabLock);
			// Free C style strings.
			Runtime::InteropServices::Marshal::FreeHGlobal(IntPtr(constNameC));
			Runtime::InteropServices::Marshal::FreeHGlobal(IntPtr(constFileLocC));
		}
	}

	array<double>^ TrainSimulator::GetParam(mdlParameterNames paramName)
	{
		return TrainSimulator::GetParam(paramName.ToString());
	}

	array<double>^ TrainSimulator::GetParam(String^ paramName)
	{
		char* name = (char*)System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(paramName).ToPointer();
		MATFile * pmat = NULL;

		// Locate the parameters file.
		String^ mdlHome = Path::Combine(modelDirectory, this->mdlName.ToString());
		String^ paramFileLoc = Path::Combine(mdlHome, String::Concat(this->mdlName,"_params.mat"));
		char * paramFileLocC = (char *)System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(paramFileLoc).ToPointer();

		Threading::Monitor::Enter(matlabLock);
		try{
			// Open the parameters file for reading.
			pmat = matOpen(paramFileLocC, "r");
			if (NULL == pmat)
			{
				throw gcnew Exception(String::Format("Unable to open {0} for reading. Cannot update tuneable parameters.", paramFileLoc));
			}

			mxArray * parametersStruct = matGetVariable(pmat,"params");
			if (NULL == parametersStruct)
			{
				matClose(pmat);
				throw gcnew Exception("Cannot find 'params' variable in parameter file.");
			}

			// Close the parameters file.
			matClose(pmat);
			pmat = NULL;
			array<double>^ values = this->getRTPparam(parametersStruct, name);
			mxDestroyArray(parametersStruct);
			return values;
		}
		finally {
			Threading::Monitor::Exit(matlabLock);
			// Free C style strings.
			Runtime::InteropServices::Marshal::FreeHGlobal(IntPtr(name));
			Runtime::InteropServices::Marshal::FreeHGlobal(IntPtr(paramFileLocC));
		}
	}

	void TrainSimulator::createInputOrder(array<mdlInputNames>^ inputsInOrder){
		this->inputsOrder = inputsInOrder;
	}

	void TrainSimulator::createOutputOrder(array<mdlOutputNames>^ outputsInOrder){
		this->outputsOrder = outputsInOrder;
	}

	String^ TrainSimulator::GetOutputFileLoc(){
		return this->outFile;
	}

}
