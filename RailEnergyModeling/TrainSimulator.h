#pragma once

namespace mat
{
#include "mat/mat.h"
}
using namespace mat;
#include "GlobalTrainModelParamStruct.h"
#include "DriveCycleStruct.h"
#include "ParamStruct.h"
#include "TrainModelParamStruct.h"
#include "SimulationResults.h"
#include "TrainSimEnums.h"

using namespace System::Collections::Generic;

namespace TrainSimulatorCLI
{

	public ref class TrainSimulator abstract
	{
	private:
		/// <summary>
		/// Lock for accessing Matlab C API that is not thread safe.
		/// </summary>
		static System::String^ matlabLock = "MATLABLock";
		
		/// <summary>
		/// The directory where the compiled models can be found.
		/// </summary>
		String^ modelDirectory;

		String^ inFile;   // Temporary file name for the input file to the model.
		char* inFileC;

		String^ outFile;  // Temporary file name for the output of the model.
		char* outFileC;

		String^ paramFile;  // Temporary file name for the model parameter file.
		char* paramFileC;

		bool cleanupOutput; //flag for indicating if the output file should be deleted.

		mdlSimlinkName mdlName; //SimLinkModel Name

		Dictionary< mdlInputNames, array<double>^ > ^ mdlInputs; //time inputs
		Dictionary< mdlOutputNames, array<double>^ > ^ mdlOutputs; // time outputs
		Dictionary< mdlConstantNames, array<double>^ > ^ mdlConstants; // constant outputs

		// getRTPparam will get the value of a parameter in a rapid simulation parameter structure.
		// That is, the structure that is obtained when you open a model in matlab and then run rsimgetrtp(bdroot)
		// This function assumes that all of the parameters in the structure are arrays
		// (all of the parameters, not just the parameter that you are adjusting).
		array<double>^ getRTPparam(mxArray * rtpParamStruct,char * paramName);

		//Gets specified parameter from parameters file
		//Throws exception if the parameter doesn't exist
		array<double>^ GetParam(String^ paramName);

		//Gets specified parameter from constants file
		//Throws exception if the parameter doesn't exist
		double GetConst(String^ paramName);

		// setRTPparam will adjust the parameters of a rapid simulation parameter structure.
		// That is, the structure that is obtained when you open a model in matlab and then run rsimgetrtp(bdroot)
		// It does the same thing as rsimsetrtpparam. rsimsetrtpparam(params,'ess_init_soc_init',0.85) should be equivalent to 
		// cct_setRTPparam(mparams, "ess_init_soc_init", 0.85) mparams is a pointer to an mxArray 
		// which is a structure in the form returned by rsimgetrtp.
		// This function does not replicate the full functionality of rsimsetrtpparam.
		// It assumes that all of the parameters in the structure are scalars
		// (all of the parameters, not just the parameter that you are adjusting).
		int setRTPparam(mxArray * rtpParamStruct,char * paramName,array<double>^ paramValues);

		// we must set this in the actual car model and make it match the order of the compiled model. 
		array<mdlInputNames >^ inputsOrder;
			
		// we must set this in the  get the outputs from the file in the order that is specified here. 
		array<mdlOutputNames>^ outputsOrder;

	protected:
			// these are called at the top of the constructor. 
			// Override them so you can set inputs Order and outputs Order
			// and specify the parameters that the model contains.
			virtual void initInputsOutputsParams() = 0;

			void createInputOrder( array< mdlInputNames > ^ );
			void createOutputOrder( array< mdlOutputNames >^ );

	public:

		//The name of the model being used
		property mdlSimlinkName Name
		{
			public: mdlSimlinkName get()
			{
				return this->mdlName;
			}
			protected: void set(mdlSimlinkName value)
			{
				this->mdlName = value;
			}
		}
		//Use this to determine which inputs the model has
		property Dictionary<mdlInputNames, array<double>^ >::KeyCollection^ Inputs
		{
			Dictionary<mdlInputNames, array<double>^ >::KeyCollection^ get()
			{
				return this->mdlInputs->Keys;
			}
		}

		// Use this to determine which outputs the model has
		property array<mdlOutputNames>^ Outputs
		{
			array<mdlOutputNames>^ get()
			{
				array<mdlOutputNames>^ outputs = gcnew array<mdlOutputNames>(this->mdlOutputs->Keys->Count);
				this->mdlOutputs->Keys->CopyTo(outputs, 0);
				return outputs;
			}
		}

		//has no effect if called on an input that is not valid.
		void SetInput(mdlInputNames, array<double>^);
			
		//throw exception if called on an output that does not exist. 
		array<double>^ GetOutput(mdlOutputNames outputName);

		//Gets specified parameter from parameters file
		//Throws exception if the parameter doesn't exist
		array<double>^ GetParam(mdlParameterNames paramName);

		//Gets specified parameter from constants file
		//Throws exception if the parameter doesn't exist
		double GetConst(mdlConstantNames constName);

		//Gets the simulation output's location
		String^ GetOutputFileLoc();

		/// <summary>
		/// Create a new VehicleSimulator
		/// </summary>
		/// <param name="simlinkName">The model name.</param>
		/// <param name="modelDirectory">The directory where the compiled model can be found.</param>
		TrainSimulator(mdlSimlinkName simlinkName, String^ modelDirectory);


		// Destructor.
		~TrainSimulator();

		/// <summary>
		/// Run the simulation.
		/// </summary>
		void Simulate(array<ParamStruct>^ paramStructs);
	};

}