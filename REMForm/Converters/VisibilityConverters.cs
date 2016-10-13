using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using TrainSimulatorCLI;
using REMForm.ViewModels;
using REMForm.ViewModels.Options;
using REMForm.ViewModels.DriveCycle;
using REMForm.ViewModels.Simulation;

//All of the converters within this namespace are used for determining if, given the provided value, a UI element should be shown or hidden
//Key thing to note about visibility here:
///Visible: Display element
///Hidden: Don't display element, but reserve space for element in layout
///Collapsed: Don't display element or reserve space
namespace REMForm.Converters.VisibilityConverters
{
    /// <summary>
    /// Visibility converter for showing UI elements based on the provided TrainTopology value
    /// The "valid" TrainTopology enums are expected to be passed in as a parameter
    /// </summary>
    [ValueConversion(typeof(TrainTopology?), typeof(Visibility))]
    public class TrainTopologyCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return Visibility.Visible;
            }

            TrainTopology? type = (TrainTopology?)value;

            var paramArray = parameter as Array;
            bool isTopology = false;

            if (value != null)
            {
                foreach (var top in paramArray)
                {
                    TrainTopology? temp = (TrainTopology?)top;
                    if (type == temp)
                    {
                        isTopology = true;
                    }
                }
            }
            if (isTopology)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Don't need to implement this
            return null;
        }
    }
    /// <summary>
    /// Visibility converter for showing UI elements based on the provided AuxEngineType value
    /// The "valid" AuxEngineType enums are expected to be passed in as a parameter
    /// </summary>
    [ValueConversion(typeof(AuxEngineType?), typeof(Visibility))]
    public class AuxTypeVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return Visibility.Visible;
            }

            AuxEngineType? type = (AuxEngineType?)value;

            var paramArray = parameter as Array;
            bool isTopology = false;

            if (value != null)
            {
                foreach (var top in paramArray)
                {
                    AuxEngineType? temp = (AuxEngineType?)top;
                    if (type == temp)
                    {
                        isTopology = true;
                    }
                }
            }
            if (isTopology)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Don't need to implement this
            return null;
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class VisibilityHiddenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool visible = (bool)value;
            if (value != null && visible)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Don't need to implement this
            return null;
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class VisibilityCollapsedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool visible = (bool)value;
            if (value != null && visible)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Don't need to implement this
            return null;
        }
    }

    /// <summary>
    /// Visibility converter for showing the Drive Cycle graph and insights
    /// If there are drive cycles loaded, displays the UI elements
    /// </summary>
    [ValueConversion(typeof(List<DriveCycleStruct>), typeof(Visibility))]
    public class DriveCycleVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<DriveCycleStruct> driveCycles = (List<DriveCycleStruct>)value;
            if (value != null && driveCycles.Any())
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Don't need to implement this
            return null;
        }
    }

    /// <summary>
    /// Visibility converter for showing the Simulation graph and insights
    /// If there are simulation results loaded, displays the UI elements
    /// </summary>
    [ValueConversion(typeof(List<SimResult>), typeof(Visibility))]
    public class SimulationResultVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<SimResult> results = (List<SimResult>)value;
            if (value != null && results.Any())
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Don't need to implement this
            return null;
        }
    }

    /// <summary>
    /// Visibility converter for displaying the text box and button for loading a drive cycle file on the user's computer
    /// If the bool representing if the sample file should be used is false, shows the UI elements
    /// </summary>
    [ValueConversion(typeof(bool?), typeof(Visibility))]
    public class UseSampleFileVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool? useSampleFile = (bool?)value;
            if (value != null && !useSampleFile.Value)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Don't need to implement this
            return null;
        }
    }
}
