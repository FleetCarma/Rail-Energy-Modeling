using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using TrainSimulatorCLI;
using System.Windows;
using System.Windows.Data;
using REMForm.ViewModels.DriveCycle;

namespace REMForm.Converters.DataConverters
{
    /// <summary>
    /// Converts a list of loaded drive cycles into a model for displaying information on the list
    /// </summary>
    [ValueConversion(typeof(List<DriveCycleStruct>), typeof(DriveCycleInsightsModel))]
    public class DriveCycleInsightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<DriveCycleStruct> driveCycles = (List<DriveCycleStruct>)value;
            DriveCycleInsightsModel insights = new DriveCycleInsightsModel
            {
                AverageMovingSpeed = 0,
                AverageSpeed = 0,
                PeakGrade = 0,
                TopSpeed = 0,
                TotalTime = 0
            };
            if (value != null && driveCycles.Any())
            {
                insights.AverageSpeed = Math.Round(driveCycles.Select(cycle => cycle.velocity).Average() * 3.6, 2);
                insights.AverageMovingSpeed = Math.Round(driveCycles.Where(cycle => cycle.velocity != 0).Select(cycle => cycle.velocity).Average() * 3.6, 2);
                insights.PeakGrade = Math.Round(driveCycles.Select(cycle => cycle.grade).Max(), 2);
                insights.TopSpeed = Math.Round(driveCycles.Select(cycle => cycle.velocity).Max() * 3.6, 2);
                insights.TotalTime = driveCycles.Select(cycle => cycle.time).Max();
            }
            return insights;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Don't need to implement this
            return null;
        }
    }
}
