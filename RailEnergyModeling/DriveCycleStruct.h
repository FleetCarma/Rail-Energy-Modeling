
namespace TrainSimulatorCLI
{
    using namespace System;
    using namespace System::Collections::Generic;

    public value struct DriveCycleStruct
    {
        static const int DriveCycleStructStruct_COLUMNS = 5;

        double time;
        double velocity;
        double grade;
        bool keyOn;
        double auxPowerLoad;
        double distance; //set while reading in values
    };
}