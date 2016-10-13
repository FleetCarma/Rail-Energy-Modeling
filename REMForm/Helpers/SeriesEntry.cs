using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace REMForm.Helpers
{
    /// <summary>
    /// Helper class for containing information on a series that will be displayed on a graph
    /// </summary>
    public class SeriesEntry
    {
        /// <summary>
        /// KV-pair collection, the points of the series
        /// </summary>
        public ObservableCollection<KeyValuePair<double, double>> Points { get; private set; }

        /// <summary>
        /// Dependant axis Title of the series
        /// </summary>
        public string DependantTitle { get; private set; }

        /// <summary>
        /// Independant axis Title of the series
        /// </summary>
        public string IndependantTitle { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public SeriesEntry(ObservableCollection<KeyValuePair<double, double>> points, string title, string indepTitle = null)
        {
            Points = points;
            DependantTitle = title;
            if (!string.IsNullOrWhiteSpace(indepTitle))
            {
                IndependantTitle = indepTitle;
            }
        }
    }
}
