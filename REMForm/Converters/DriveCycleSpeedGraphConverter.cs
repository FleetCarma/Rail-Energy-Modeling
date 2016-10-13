using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using TrainSimulatorCLI;
using System.Collections.ObjectModel;
using System.Windows;
using REMForm.DataStructs;
using System.Windows.Media;
using OxyPlot;

namespace REMForm.Converters.DataConverters
{
    public class DriveCycleGraphConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<DriveCycleStruct> driveCycles = (List<DriveCycleStruct>)value;
            DriveCycleGraph graphData = new DriveCycleGraph();
            if (value != null && driveCycles.Any())
            {
                IEnumerable<DataPoint> speedPoints = driveCycles.Select(cycle => new DataPoint(cycle.time, cycle.velocity));
                IEnumerable<DataPoint> gradePoints = driveCycles.Select(cycle => new DataPoint(cycle.time, cycle.grade));

                graphData.VelocityTimePoints = new List<DataPoint>(speedPoints);
                graphData.GradeTimePoints = new List<DataPoint>(speedPoints);


                PlotModel velocityModel = new PlotModel();
                velocityModel.Series.Add(graphData.VelocitySeries);
                velocityModel.InvalidatePlot(true);
                PlotModel gradeModel = new PlotModel();
                gradeModel.Series.Add(graphData.GradeSeries);
                gradeModel.InvalidatePlot(true);
                graphData.VelocityModel = velocityModel;
                graphData.GradeModel = gradeModel;
                return graphData;

            }
            return graphData;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Don't need to implement this
            return null;
        }
    }
}
