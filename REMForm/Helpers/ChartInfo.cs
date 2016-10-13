using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.DataVisualization.Charting;

namespace REMForm.Helpers
{
    public class ChartInfo
    {
        public Chart Chart { get; set; }

        public List<LinearAxis> Axes { get; set; }

        public List<LineSeries> Series { get; set;}

        public ChartInfo(Chart chart, List<LinearAxis> axes, List<LineSeries> series)
        {
            Chart = chart;
            Axes = axes;
            Series = series;
        }
    }
}
