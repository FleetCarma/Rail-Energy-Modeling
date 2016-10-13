using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using REMForm.Helpers;
using UnitSystem;

namespace REMForm.ViewModels.DriveCycle
{
    /// <summary>
    /// Model for displaying data on the loaded drive cycles
    /// This is not a part of any view, rather it is created through a converter within the .xaml code
    /// (See DriveCycleInsightConverter.cs for how this is created)
    /// </summary>
    public class DriveCycleInsightsModel : NotifyPropertyChangedHandler
    {
        //Private variables
        private double _topSpeed;
        private double _averageSpeed;
        private double _averageMovingSpeed;
        private double _peakGrade;
        private double _totalTime;

        /// <summary>
        /// The highest speed of the loaded drive cycle
        /// </summary>
        public double TopSpeed
        {
            get
            {
                Speed val = new Speed(_topSpeed);
                UnitDescription desc = ApplicationModel.staticUnitSystem.Speed;
                return (int)val.GetValue(desc.Unit);
            }
            set
            {
                //This is only ever stored as kph, since it's not a user-entered value
                _topSpeed = value;
                NotifyPropertyChanged("TopSpeed");
            }
        }

        /// <summary>
        /// The average speed of the loaded drive cycle (includes speeds of zero)
        /// </summary>
        public double AverageSpeed
        {
            get
            {
                Speed val = new Speed(_averageSpeed);
                UnitDescription desc = ApplicationModel.staticUnitSystem.Speed;
                return (int)val.GetValue(desc.Unit); 
            }
            set
            {
                //This is only ever stored as kph, since it's not a user-entered value
                _averageSpeed = value;
                NotifyPropertyChanged("AverageSpeed");
            }
        }

        /// <summary>
        /// The average speed of the loaded drive cycle (does not include speeds of zero)
        /// </summary>
        public double AverageMovingSpeed
        {
            get
            {
                Speed val = new Speed(_averageMovingSpeed);
                UnitDescription desc = ApplicationModel.staticUnitSystem.Speed;
                return (int)val.GetValue(desc.Unit);
            }
            set
            {
                //This is only ever stored as kph, since it's not a user-entered value
                _averageMovingSpeed = value;
                NotifyPropertyChanged("AverageMovingSpeed");
            }
        }

        /// <summary>
        /// The highest grade value in the loaded drive cycle
        /// </summary>
        public double PeakGrade
        {
            get { return _peakGrade; }
            set
            {
                _peakGrade = value;
                NotifyPropertyChanged("PeakGrade");
            }
        }

        /// <summary>
        /// The total length time that the drive cycle represents (in seconds)
        /// </summary>
        public double TotalTime
        {
            get { return _totalTime; }
            set
            {
                _totalTime = value;
                NotifyPropertyChanged("TotalTime");
            }
        }

        public override void NotifyAllPropertiesChanged()
        {
            NotifyPropertyChanged(string.Empty);
        }
    }
}
