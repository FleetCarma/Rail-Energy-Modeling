using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using TrainSimulatorCLI;
using REMForm.Helpers;
using System.IO;
using UnitSystem;

namespace REMForm.ViewModels
{
    /// <summary>
    /// Model for the drive cycle tab
    /// </summary>
    public class DriveCycleModel : NotifyPropertyChangedHandler
    {
        //Chart image file name constants
        private const string CYCLESPEED_FILENAME = "_cyclespeed.png";
        private const string CYCLEGRADE_FILENAME = "_cyclegrade.png";
        private const string CYCLEAUX_FILENAME = "_cycleaux.png";
        private const string CYCLEKEYON_FILENAME = "_cyclekeyon.png";

        //Private Variables
        private string _driveCyclePath;
        private List<DriveCycleStruct> _driveCycles;
        private CycleChartType _chartType;
        private string _chartImageLoc;
        private bool _useSampleCycleFile;
        private bool _graphNeedsUpdate;

        /// <summary>
        /// List of loaded drive cycles
        /// </summary>
        public List<DriveCycleStruct> DriveCycles
        {
            get { return _driveCycles; }
            set
            {
                _driveCycles = value;
                NotifyPropertyChanged("DriveCycles");
            }
        }

        /// <summary>
        /// The type of chart to display
        /// </summary>
        public CycleChartType ChartType
        {
            get { return _chartType; }
            set
            {
                _chartType = value;
                NotifyPropertyChanged("ChartType");
            }
        }

        /// <summary>
        /// If the sample cycle file should be used
        /// </summary>
        public bool UseSampleCycleFile
        {
            get { return _useSampleCycleFile; }
            set
            {
                _useSampleCycleFile = value;
                NotifyPropertyChanged("UseSampleCycleFile");
            }
        }

        /// <summary>
        /// The filepath of the file to load drive cycles from
        /// </summary>
        public string DriveCyclePath
        {
            get { return _driveCyclePath; }
            set
            {
                _driveCyclePath = value;
                NotifyPropertyChanged("DriveCyclePath");
            }
        }

        /// <summary>
        /// The location on the user's computer where the image of the chart to display is stored
        /// </summary>
        public string ChartImageLoc
        {
            get { return _chartImageLoc; }
            set
            {
                _chartImageLoc = value;
                NotifyPropertyChanged("ChartImageLoc");
            }
        }

        /// <summary>
        /// If the drive Cycle Graph needs to be updated, and thus a message displayed to the user
        /// Cannot be set if there are no drive cycles loaded
        /// </summary>
        public bool GraphNeedsUpdate
        {
            get
            {
                return _graphNeedsUpdate;
            }
            set
            {
                if (DriveCycles.Any())
                {
                    _graphNeedsUpdate = value;
                    NotifyPropertyChanged("GraphNeedsUpdate");
                }
            }
        }

        public override void NotifyAllPropertiesChanged()
        {
            NotifyPropertyChanged(string.Empty);
        }

        /// <summary>
        /// The "base" file location.
        /// Points to the user's temp folder, and includes a random file name
        /// </summary>
        public string BaseFileLoc { get; private set; }

        /// <summary>
        /// KV-pair representing time vs Kph
        /// </summary>
        public IEnumerable<KeyValuePair<double, double>> SpeedPoints
        {
            get
            {
                return DriveCycles.Select(cycle => new KeyValuePair<double, double>(
                        cycle.time,
                        (new Speed(cycle.velocity * Constants.MpsToKph, Unit.KPH))
                            .GetValue(ApplicationModel.staticUnitSystem.Speed.Unit))
                    );
            }
        }

        /// <summary>
        /// KV-pair representing time vs Grade (radians)
        /// </summary>
        public IEnumerable<KeyValuePair<double, double>> GradePoints
        {
            get
            {
                return DriveCycles.Select(cycle => new KeyValuePair<double, double>(cycle.time, cycle.grade));
            }
        }

        /// <summary>
        /// KV-pair representing time vs AuxPowerLoad
        /// </summary>
        public IEnumerable<KeyValuePair<double, double>> AuxPowerloadPoints
        {
            get
            {
                return DriveCycles.Select(cycle => new KeyValuePair<double, double>(cycle.time, cycle.auxPowerLoad));
            }
        }

        /// <summary>
        /// KV-pair representing time vs KeyOn
        /// </summary>
        public IEnumerable<KeyValuePair<double, double>> KeyOnPoints
        {
            get
            {
                return DriveCycles.Select(cycle => new KeyValuePair<double, double>(cycle.time, (cycle.keyOn ? 1 : 0)));
            }
        }


        /// <summary>
        /// Helper function used to get the associated file location + name of the image of the specified chart
        /// Used in both finding and storing the chart image
        /// </summary>
        /// <param name="type">Type of chart</param>
        /// <returns>File location of the desired chart</returns>
        public string ImageLoc(CycleChartType type)
        {
            switch (type)
            {
                case CycleChartType.Grade:
                    return BaseFileLoc + CYCLEGRADE_FILENAME;
                case CycleChartType.AuxLoad:
                    return BaseFileLoc + CYCLEAUX_FILENAME;
                case CycleChartType.Speed:
                default:
                    return BaseFileLoc + CYCLESPEED_FILENAME;
            }
        }


        /// <summary>
        /// Constructor for a drive cycle model
        /// </summary>
        public DriveCycleModel()
        {
            UseSampleCycleFile = false;
            _graphNeedsUpdate = false;
            ChartType = CycleChartType.Speed;
            BaseFileLoc = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            DriveCycles = new List<DriveCycleStruct>();

            ChartImageLoc = ImageLoc(ChartType);
        }

        /// <summary>
        /// Destructor for a drive cycle model
        /// </summary>
        ~DriveCycleModel()
        {
            foreach (CycleChartType type in Enum.GetValues(typeof(CycleChartType)))
            {
                if (File.Exists(ImageLoc(type)))
                {
                    File.Delete(ImageLoc(type));
                }
            }
        }

        /// <summary>
        /// Enum for the type of chart to use/display
        /// </summary>
        public enum CycleChartType
        {
            Speed,
            Grade,
            AuxLoad
        }
    }
}
