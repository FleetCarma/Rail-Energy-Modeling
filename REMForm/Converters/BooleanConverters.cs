using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using REMForm.ViewModels.Options;
using System.Globalization;

namespace REMForm.Converters.BooleanConverters
{
    /// <summary>
    /// Checks if the passed in TrainTopology is one of the specified topologies passed in as a parameter
    /// </summary>
    [ValueConversion(typeof(TrainTopology?), typeof(bool))]
    public class IsTopologyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return true;
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
            return isTopology;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Don't need to implement this
            return null;
        }
    }
    /// <summary>
    /// Checks if the passed in Energy Storage System is one of the specified types passed in as a parameter
    /// </summary>
    [ValueConversion(typeof(EnergyStorageSystem?), typeof(bool))]
    public class IsESSConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return true;
            }

            EnergyStorageSystem? type = (EnergyStorageSystem?)value;

            var paramArray = parameter as Array;
            bool isESS = false;

            if (value != null)
            {
                foreach (var top in paramArray)
                {
                    EnergyStorageSystem? temp = (EnergyStorageSystem?)top;
                    if (type == temp)
                    {
                        isESS = true;
                    }
                }
            }
            return isESS;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Don't need to implement this
            return null;
        }
    }
}
