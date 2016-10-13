using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using REMForm.ViewModels;
using TrainSimulatorCLI;
using REMForm.ViewModels.Options;
using System.Collections.ObjectModel;
using REMForm.ViewModels.Simulation;
using System.Windows.Controls.DataVisualization.Charting;
using System.ComponentModel;
using System.Windows.Threading;
using REMForm.Helpers;
using System.Reflection;
using UnitSystem;
using System.Threading;

namespace REMForm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Resource name of the sample drive cycle
        /// </summary>
        private const string SampleDriveCycleResourceName = "REMForm.SampleDriveCycle.csv";

        private const double FinalPowerIndex = 13.7;

        private const double FinalFuelRate = 0.000897018;

        private const int AESSTimeBeforeOn = 20;

        private const int AESSTimeAfterOff = 20;

        private const double BaseAPUEfficiency = 0.5367;

        private SimWrapper SimWrapper;

        #region Window-specific methods
        /// <summary>
        /// Constructor for the main window
        /// Instantiates a new ApplicationModel and sets the window's data context to be this model
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            ApplicationModel _model = new ApplicationModel();
            this.DataContext = _model;
        }

        /// <summary>
        /// Property to get the Application model of the window
        /// The model contains information used by practically all of the application
        /// </summary>
        private ApplicationModel Model
        {
            get
            {
                return (ApplicationModel)this.DataContext;
            }
        }

        private IEnumerable<double> PowerIndex
        {
            get
            {
                double multiplier = (Model.ParamConfigModel.AuxEngineOption.Load / FinalPowerIndex);
                for (int i = 0; i < 10; i++)
                {
                    yield return (FinalPowerIndex / 9.0) * (double)i * multiplier;
                }
            }
        }

        private IEnumerable<double> FuelRate
        {
            get
            {
                double multiplier = (Model.ParamConfigModel.AuxEngineOption.Load / FinalPowerIndex) / (Model.ParamConfigModel.AuxEngineOption.Efficiency / BaseAPUEfficiency);
                for (int i = 0; i < 10; i++)
                {
                    yield return (FinalFuelRate / 9.0) * (double)i * multiplier;
                }
            }
        }

        /// <summary>
        /// Event for when the window has been loaded.
        /// Used for some extra configurations to make which weren't able to have been set in the initial definitions
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //For some reason EnumRadioButtons throw ArgumentNullExceptions when their Checked-event is set in the xaml editor
            //It's an issue with how the elements' dependency properties are set up, but for the interest of time this will have to do

            //Drive Cycle enum setup
            this.CycleRadio_Speed.Checked += new RoutedEventHandler(CycleRadio_Speed_Checked);
            this.CycleRadio_Grade.Checked += new RoutedEventHandler(CycleRadio_Grade_Checked);
            this.CycleRadio_AuxLoad.Checked += new RoutedEventHandler(CycleRadio_AuxLoad_Checked);

            //Sim Results enum setup
            this.SimRadio_VehicleSpeed.Checked += new RoutedEventHandler(SimGraph_VehicleSpeed_Checked);
            this.SimRadio_FuelBurnRate.Checked += new RoutedEventHandler(SimGraph_FuelBurnRate_Checked);
            this.SimRadio_EngineSpeed.Checked += new RoutedEventHandler(SimGraph_EngineSpeed_Checked);
            this.SimRadio_LocoPower.Checked += new RoutedEventHandler(SimGraph_LocoPower_Checked);
            this.SimRadio_AESSState.Checked += new RoutedEventHandler(SimGraph_AESSState_Checked);
            this.SimRadio_GHGEmissions.Checked += new RoutedEventHandler(SimGraph_GHGEmissions_Checked);
        }

        /// <summary>
        /// Function to call when the window is closing
        /// The main point of this is to clean up the temp folder of the chart images
        /// </summary>
        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            //We need to invalidate the chart images, otherwise they'll hold a lock on the image files they're displaying
            this.CycleChartImage.InvalidateVisual();
            this.SimChartImage.InvalidateVisual();


        }
        #endregion

        /// <summary>
        /// Event called when the button for browsing for drive cycle csv files is clicked
        /// </summary>
        private void CycleBrowse_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".csv"; // Default file extension
            dlg.Filter = "Comma-separated values file (.csv)|*.csv"; // Filter files by extension
            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                Model.DriveCycleModel.DriveCyclePath = filename;
            }
        }

        /// <summary>
        /// Called when the user clicks on the Load Drive Cycle button.
        /// Simply calls another function which handles the logic for loading from either an embedded, sample drive cycle file, or one provided by the user
        /// </summary>
        private void LoadCycleFile_Click(object sender, RoutedEventArgs e)
        {
            LoadDriveCycles();
        }

        /// <summary>
        /// Function used for loading drive cycles from a .csv file and creating the graphs representing them
        /// </summary>
        private void LoadDriveCycles()
        {
            List<DriveCycleStruct> driveCycles = new List<DriveCycleStruct>();

            if (!Model.DriveCycleModel.UseSampleCycleFile)
            {
                if (!string.IsNullOrWhiteSpace(Model.DriveCycleModel.DriveCyclePath) && File.Exists(Model.DriveCycleModel.DriveCyclePath))
                {
                    try
                    {
                        using (FileStream stream = new FileStream(Model.DriveCycleModel.DriveCyclePath,
                            FileMode.Open,
                            FileAccess.Read,
                            FileShare.ReadWrite|FileShare.Delete))
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                driveCycles = LoadDriveCyclesHelper(reader);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBoxResult result = MessageBox.Show("There was an error in reading the specified drive cycle file. Please make sure that the file is not currently in use by another program",
                            "Error opening file",
                            MessageBoxButton.OK);
                    }
                }
            }
            else
            {
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(SampleDriveCycleResourceName))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        driveCycles = LoadDriveCyclesHelper(reader);
                    }
                }
            }

            if (driveCycles.Any())
            {
                GenerateDriveCycleGraphs(driveCycles);
            }
        }

        private void GenerateDriveCycleGraphs(List<DriveCycleStruct> driveCycles)
        {
            if (driveCycles.Count > 0 && DriveCycleTab.IsSelected)
            {
                if (Model.ParamConfigModel.TrainOption.Topology != TrainTopology.Electric)
                {
                    ProcessAESS(driveCycles, Model.ParamConfigModel.AuxEngineOption.AESS, Model.ParamConfigModel.AuxEngineOption.Type);
                }
                //set the DriveCycles to what we just read in
                Model.DriveCycleModel.DriveCycles = driveCycles;

                //Set up the charts
                List<SeriesEntry> entries = new List<SeriesEntry>()
                {
                    new SeriesEntry(new ObservableCollection<KeyValuePair<double, double>>(Model.DriveCycleModel.SpeedPoints),
                        string.Format("Speed ({0})", ApplicationModel.staticUnitSystem.Speed.Label)),
                    new SeriesEntry(new ObservableCollection<KeyValuePair<double, double>>(Model.DriveCycleModel.GradePoints), "Grade (radians)"),
                    new SeriesEntry(new ObservableCollection<KeyValuePair<double, double>>(Model.DriveCycleModel.AuxPowerloadPoints), "Auxiliary Load (kW)"),
                    new SeriesEntry(new ObservableCollection<KeyValuePair<double, double>>(Model.DriveCycleModel.KeyOnPoints), "Engine On/Off State")
                };


                CycleGraph_SetGraph(entries.ElementAt(0), DriveCycleModel.CycleChartType.Speed);

                CycleGraph_SetGraph(entries.ElementAt(1), DriveCycleModel.CycleChartType.Grade);

                CycleGraph_SetGraph(entries.GetRange(2, 2), DriveCycleModel.CycleChartType.AuxLoad);
                //SetToDependentAxisDefaults(this.CycleChart_DependantAxis1);

                CycleGraph_SetImage(Model.DriveCycleModel.ChartImageLoc);
                Model.DriveCycleModel.GraphNeedsUpdate = false;
                Model.SimulationModel.GraphNeedsUpdate = true;
            }
        }

        private List<DriveCycleStruct> LoadDriveCyclesHelper(StreamReader reader)
        {
            List<DriveCycleStruct> driveCycles = new List<DriveCycleStruct>();

            double totalDistanceTravelled = 0;
            //read from the file
            while (reader.Peek() >= 0)
            {
                string line = reader.ReadLine();
                string[] values = line.Split(',');
                DriveCycleStruct cycle = new DriveCycleStruct();
                double value;

                //Time
                if (Double.TryParse(values[0], out value))
                {
                    cycle.time = value;
                }
                else { continue; } //if we can't get the value, we can't get a good drive cycle entry, so skip it

                //Forward Velocity
                if (Double.TryParse(values[1], out value))
                {
                    cycle.velocity = value;
                }
                else { continue; }


                if (Double.TryParse(values[0], out value) &&
                    Double.TryParse(values[1], out value))
                {
                    if (driveCycles.Count > 0)
                    {
                        double timeDelta = cycle.time - driveCycles.Last().time;
                        double vel = cycle.velocity;
                        totalDistanceTravelled += vel * timeDelta;
                    }
                    cycle.distance = totalDistanceTravelled;
                }

                //Grade
                if (Double.TryParse(values[2], out value))
                {
                    cycle.grade = value;
                }
                else { continue; }

                if (Double.TryParse(values[3], out value))
                {
                    cycle.keyOn = (value == 1);
                }
                else { continue; }

                if (Double.TryParse(values[4], out value))
                {
                    cycle.auxPowerLoad = value;
                }
                else { continue; }

                driveCycles.Add(cycle);
            }

            return driveCycles;
        }

        private void ProcessAESS(List<DriveCycleStruct> driveCycles, bool aess, AuxEngineType auxType)
        {
            if (ApplicationModel.staticTopology != TrainTopology.Electric)
            {
                if (aess)
                {
                    int startIndex = 0;
                    int endIndex = 0;
                    for (int i = 0; i < driveCycles.Count; i++)
                    {
                        DriveCycleStruct driveCycle = driveCycles[i];

                        startIndex = 1;
                        endIndex = driveCycles.Count - 1;

                        if (driveCycle.time - AESSTimeBeforeOn > driveCycles.First().time)
                        {
                            startIndex = driveCycles.Select((d, indx) => new { d, indx })
                                .Where(x => x.d.time >= driveCycle.time - AESSTimeBeforeOn)
                                .Select(x => x.indx)
                                .First();
                        }

                        if (driveCycle.time + AESSTimeAfterOff < driveCycles.Last().time)
                        {
                            endIndex = driveCycles.Select((d, indx) => new { d, indx })
                                .Where(x => x.d.time <= driveCycle.time + AESSTimeAfterOff)
                                .Select(x => x.indx)
                                .Last();
                        }
                        bool anyMovement = driveCycles.GetRange(startIndex, (endIndex - startIndex) + 1).Any(d => d.velocity > 0);

                        if (anyMovement)
                        {
                            driveCycle.keyOn = true;
                            if (auxType == AuxEngineType.HEP)
                            {
                                driveCycle.auxPowerLoad = PowerIndex.Last();
                            }
                            else
                            {
                                driveCycle.auxPowerLoad = 0;
                            }
                        }
                        else
                        {
                            driveCycle.keyOn = false;
                            if (auxType == AuxEngineType.None)
                            {
                                driveCycle.auxPowerLoad = 0;
                            }
                            else
                            {
                                driveCycle.auxPowerLoad = PowerIndex.Last();
                            }

                        }

                        driveCycles[i] = driveCycle;
                    }
                }
                else
                {
                    for (int i = 0; i < driveCycles.Count; i++)
                    {
                        DriveCycleStruct cycle = driveCycles[i];

                        cycle.auxPowerLoad = auxType == AuxEngineType.None ||
                            auxType == AuxEngineType.APU
                            ? 0 : PowerIndex.Last();

                        driveCycles[i] = cycle;
                    }
                }
            }
        }

        /// <summary>
        /// Event for when the simulate button is clicked
        /// This uses a background worker to run some logic in another thread, and display a progress bar
        /// However, the most time-costly operation is in the UI rendering thread, which causes the progress bar to freeze up
        /// Going forward, the rendering of the progress bar should be put into another thread as well, to avoid freezing
        /// </summary>
        private void Simulate_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                bool invalidConfig = false;
                TrainTopology topology = TrainTopology.EngineElectric;
                EnergyStorageSystem storageSystem = EnergyStorageSystem.Battery;

                //we can't access/modify window elements in this thread, so we need to use the window's dispatcher for that
                Dispatcher.Invoke((Action)(() =>
                {
                    if (Model.DriveCycleModel.DriveCycles == null || Model.DriveCycleModel.DriveCycles.Count <= 0)
                    {
                        this.SimStatusMessage.Content = "No Drive Cycles loaded";
                        invalidConfig = true;
                    }
                    else
                    {
                        this.SimStatusMessage.Content = string.Empty;
                    }

                    this.SimChartContainer.Visibility = System.Windows.Visibility.Hidden;
                    this.SimRadios.Visibility = System.Windows.Visibility.Hidden;
                    this.SimInsights.Visibility = System.Windows.Visibility.Hidden;
                    topology = Model.ParamConfigModel.TrainOption.Topology;
                    storageSystem = Model.ParamConfigModel.TrainOption.StorageSystem;
                }));

                if (invalidConfig)
                {
                    return;
                }

                SimWrapper = new SimWrapper(topology, storageSystem);

                //Get the drive cycles
                List<DriveCycleStruct> cycles = new List<DriveCycleStruct>();

                Dispatcher.Invoke((Action)(() =>
                {
                    cycles = Model.DriveCycleModel.DriveCycles;
                }));

                Dictionary<mdlInputNames, double[]> inputs = new Dictionary<mdlInputNames, double[]>();
                inputs.Add(mdlInputNames.TIME, cycles.Select(cycle => (double)cycle.time).ToArray());
                inputs.Add(mdlInputNames.KEYON, cycles.Select(cycle => (double)(cycle.keyOn ? 1 : 0)).ToArray());
                inputs.Add(mdlInputNames.GRADE, cycles.Select(cycle => (double)cycle.grade).ToArray());
                inputs.Add(mdlInputNames.FORWARDVELOCITY, cycles.Select(cycle => (double)cycle.velocity).ToArray());
                inputs.Add(mdlInputNames.AUXPOWERLOAD, cycles.Select(cycle => (double)cycle.auxPowerLoad * 1000).ToArray());

                SimWrapper.AddInputs(inputs);

                Dispatcher.Invoke((Action)(() =>
                {
                    SimWrapper.UpdateParameters(Model.ParamConfigModel);
                }));
                //Simulate!
                SimWrapper.Simulate();

                //simulation results
                double[] speed = SimWrapper.Speed;
                double[] time = SimWrapper.Time;
                double[] distance = SimWrapper.DumbIntegrate(time, speed).ToArray();
                double[] engineSpeed = SimWrapper.EngineSpeed;
                double[] fuelRate = topology == TrainTopology.FuelCell ? SimWrapper.FuelH2Rate : SimWrapper.FuelRate;
                double[] apuFuelRate = SimWrapper.APUFuelRate;

                //total fuel use by all engines
                double[] totalFuelUse = topology == TrainTopology.FuelCell ? SimWrapper.TotalH2FuelUse : SimWrapper.TotalFuelUse;

                double[] primeEngineSpeed = SimWrapper.PrimeEngineSpeed;
                double[] primeTorque = SimWrapper.PrimeTorque;

                double[] totalGHGEmissions = SimWrapper.TotalGHGEmissions;

                double[] APUTotalFuelUse = SimWrapper.APUTotalFuelUse;
                //double maxAPUTotalFuelUse = Math.Round(APUTotalFuelUse.Max(), Constants.NumDecimals);


                double[] AESSOnOff = SimWrapper.AESSOnOffState;

                double[] enginePower = (topology == TrainTopology.FuelCell) ? SimWrapper.FuelCellPower : SimWrapper.EnginePower;

                double[] essPower = SimWrapper.ESSPower;
                double[] totalESSPower = SimWrapper.DumbIntegrate(time, essPower).ToArray();

                double[] tractivePower = SimWrapper.TractivePower;

                double[] gridEnergy = SimWrapper.GridEnergy;

                double[] auxPower = SimWrapper.APUPower;

                int numItems = time.Count();

                List<SimResult> simResults = new List<SimResult>();

                for (int i = 0; i < numItems; i++)
                {
                    SimResult temp = new SimResult
                    {
                        Speed = speed[i],
                        Time = time[i],
                        EngineSpeed = engineSpeed[i],
                        FuelRate = fuelRate[i],
                        EnginePower = enginePower[i],
                        AuxFuelConsumption = APUTotalFuelUse[i],
                        ApuFuelRate = apuFuelRate[i],
                        ApuPower = auxPower[i],
                        AESSOnOffState = AESSOnOff[i],
                        GHGEmissions = totalGHGEmissions[i],
                        TractivePower = tractivePower[i],
                        TotalFuelUse = totalFuelUse[i],
                        TotalGHGEmissions = totalGHGEmissions[i],
                        DistanceTravelled = distance[i],
                        TotalESSPower = totalESSPower[i],
                        ESSPower = (topology == TrainTopology.EngineHybrid || topology == TrainTopology.FuelCell) ?
                                    essPower[i] :
                                    0,
                        GridEnergy = (topology == TrainTopology.Electric) ?
                                    gridEnergy[i] :
                                    0,

                    };

                    simResults.Add(temp);
                }

                //UI stuff, mainly for rendering the charts so they can be turned into images
                //This really needs to be cleaned up eventually
                Dispatcher.Invoke((Action)(() =>
                {
                    Model.SimulationModel.Results = simResults;

                }));

                SimGraph_GenerateAll();

                Dispatcher.Invoke((Action)(() =>
                {
                    Model.SimulationModel.SimOutputLoc = SimWrapper.Simulator.GetOutputFileLoc();
                }));
            };
            worker.RunWorkerCompleted += (o, ea) =>
            {
                this.SimBusyIndicator.BusyContent = "Please wait...";
                SimBusyIndicator.IsBusy = false;
                if (Model.SimulationModel.Results.Any())
                {
                    this.SimChartContainer.Visibility = System.Windows.Visibility.Visible;
                    this.SimRadios.Visibility = System.Windows.Visibility.Visible;
                    this.SimInsights.Visibility = System.Windows.Visibility.Visible;
                }
            };

            SimBusyIndicator.IsBusy = true;
            worker.RunWorkerAsync();

        }

        #region Drive Cycle Graph Functions

        /// <summary>
        /// Event which fires when the Speed radio button is checked on the drive cycle tab
        /// Sets the chart image to show the drive cycle chart of time vs speed
        /// </summary>
        private void CycleRadio_Speed_Checked(object sender, RoutedEventArgs e)
        {
            CycleGraph_SetImage(Model.DriveCycleModel.ImageLoc(DriveCycleModel.CycleChartType.Speed));
        }

        /// <summary>
        /// Event which fires when the Grade radio button is checked on the drive cycle tab
        /// Sets the chart image to show the drive cycle chart of time vs grade
        /// </summary>
        private void CycleRadio_Grade_Checked(object sender, RoutedEventArgs e)
        {
            CycleGraph_SetImage(Model.DriveCycleModel.ImageLoc(DriveCycleModel.CycleChartType.Grade));
        }

        /// <summary>
        /// Event which fires when the AuxLoad radio button is checked on the drive cycle tab
        /// Sets the chart image to show the drive cycle chart of time vs AuxLoad
        /// </summary>
        private void CycleRadio_AuxLoad_Checked(object sender, RoutedEventArgs e)
        {
            CycleGraph_SetImage(Model.DriveCycleModel.ImageLoc(DriveCycleModel.CycleChartType.AuxLoad));
        }

        /// <summary>
        /// Sets the Drive Cycle graph to the specified image
        /// </summary>
        /// <param name="path">Path of the image to set the graph to</param>
        private void CycleGraph_SetImage(string path)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(path);
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();

            this.CycleChartImage.Source = bitmapImage;
            this.CycleChartImage.InvalidateVisual();
            Model.DriveCycleModel.ChartImageLoc = path;
        }

        /// <summary>
        /// Sets the values used by the Drive cycle graph, and updates the graph
        /// </summary>
        /// <param name="points">List of points (x vs y) to set the graph to use</param>
        /// <param name="chartType">The type of graph (speed or grade)</param>
        private void CycleGraph_SetGraph(SeriesEntry entry, REMForm.ViewModels.DriveCycleModel.CycleChartType chartType)
        {
            IEnumerable<SeriesEntry> list = new SeriesEntry[] { entry };
            CycleGraph_SetGraph(list, chartType);
        }

        private void CycleGraph_SetGraph(IEnumerable<SeriesEntry> entries, REMForm.ViewModels.DriveCycleModel.CycleChartType chartType)
        {
            this.CycleChartImage.Source = null;

            ChartInfo chartInfo = GetCycleChartInfo(chartType);

            for (int i = 0; i < chartInfo.Series.Count; i++)
            {
                //If there's an entry for a series
                if (i < entries.Count())
                {
                    chartInfo.Axes.ElementAt(i).Title = entries.ElementAt(i).DependantTitle;

                    if (i == 1)
                    {
                        chartInfo.Series.ElementAt(i).Style = (Style)Resources["SecondarySeries"];
                    }
                    else if (i == 2)
                    {
                        chartInfo.Series.ElementAt(i).Style = (Style)Resources["TertiarySeries"];
                    }
                    else if (i == 3)
                    {
                        chartInfo.Series.ElementAt(i).Style = (Style)Resources["FourthSeries"];
                    }
                }
            }

            RenderAndSaveChart(entries, chartInfo, Model.DriveCycleModel.ImageLoc(chartType));


            //We've rendered the points, no need to hold onto them
            foreach (SeriesEntry e in entries)
            {
                e.Points.Clear();
            }
        }

        #endregion

        #region Simulation Results Graph Functions


        /// <summary>
        /// Event which fires when the VehicleSpeed radio button is checked on the simulation tab
        /// Sets the chart image to show the simulation chart of time vs vehiclespeed
        /// 
        /// contains drive cycle speed and simulated speed
        /// </summary>
        private void SimGraph_VehicleSpeed_Checked(object sender, RoutedEventArgs e)
        {
            SimGraph_SetImage(Model.SimulationModel.ImageLoc(SimulationModel.SimChartType.VehicleSpeed));
        }

        /// <summary>
        /// Event which fires when the Fuel Burn Rate radio button is checked on the simulation tab
        /// Sets the chart image to show the simulation chart of time vs fuel burn rate
        /// 
        /// contains prime mover fuel consumption and APU fuel consumption
        /// </summary>
        private void SimGraph_FuelBurnRate_Checked(object sender, RoutedEventArgs e)
        {
            SimGraph_SetImage(Model.SimulationModel.ImageLoc(SimulationModel.SimChartType.FuelBurnRate));
        }

        /// <summary>
        /// Event which fires when the EngineSpeed radio button is checked on the simulation tab
        /// Sets the chart image to show the simulation chart of time vs EngineSpeed
        /// 
        /// contains the engine speed
        /// </summary>
        private void SimGraph_EngineSpeed_Checked(object sender, RoutedEventArgs e)
        {
            SimGraph_SetImage(Model.SimulationModel.ImageLoc(SimulationModel.SimChartType.EngineSpeed));
        }

        /// <summary>
        /// Event which fires when the GHGEmissions radio button is checked on the simulation tab
        /// Sets the chart image to show the simulation chart of time vs GHGEmissions
        /// 
        /// contains Locomotive GHG Emissions
        /// </summary>
        private void SimGraph_GHGEmissions_Checked(object sender, RoutedEventArgs e)
        {
            SimGraph_SetImage(Model.SimulationModel.ImageLoc(SimulationModel.SimChartType.GHGEmissions));
        }

        /// <summary>
        /// Event which fires when the Locomotive Power radio button is checked on the simulation tab
        /// Sets the chart image to show the simulation chart of time vs Locomotive Power
        /// 
        /// contains prime mover (engine / fuel cell), ESS, and traction power
        /// </summary>
        private void SimGraph_LocoPower_Checked(object sender, RoutedEventArgs e)
        {
            SimGraph_SetImage(Model.SimulationModel.ImageLoc(SimulationModel.SimChartType.LocomotivePower));
        }

        /// <summary>
        /// Event which fires when the Auxiliary Fuel Consumption radio button is checked on the simulation tab
        /// Sets the chart image to show the simulation chart of time vs Auxiliary Fuel Consumption
        /// </summary>
        private void SimGraph_AuxFuelConsumption_Checked(object sender, RoutedEventArgs e)
        {
            SimGraph_SetImage(Model.SimulationModel.ImageLoc(SimulationModel.SimChartType.AuxFuelConsumption));
        }

        /// <summary>
        /// Event which fires when the AESS State radio button is checked on the simulation tab
        /// Sets the chart image to show the simulation chart of time vs AESS State
        /// </summary>
        private void SimGraph_AESSState_Checked(object sender, RoutedEventArgs e)
        {
            SimGraph_SetImage(Model.SimulationModel.ImageLoc(SimulationModel.SimChartType.AESSState));
        }

        /// <summary>
        /// Sets the Simulation graph to the specified image
        /// </summary>
        /// <param name="path">Path of the image to set the graph to</param>
        private void SimGraph_SetImage(string path)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(path);
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();

            this.SimChartImage.Source = bitmapImage;
            this.SimChartImage.InvalidateVisual();
            Model.SimulationModel.ChartImageLoc = path;
        }

        /// <summary>
        /// Sets the values used by the simulation graph, and updates the graph
        /// </summary>
        /// <param name="entries">List of points/titles to set for the graph</param>
        /// <param name="chartType">The type of simulation graph, used for image location</param>
        /// <param name="topology">The typology of the train. Used for determining what to show</param>
        /// <param name="ess"> The Energy Storage System of the train. Used for determining what to show</param>
        private void SimGraph_SetGraph(SimulationModel.SimChartType chartType, TrainTopology topology, EnergyStorageSystem ess, string depAxisTitle, string indepAxisTitle, params SeriesEntry[] entries)
        {
            this.SimChartImage.Source = null;

            ChartInfo chartInfo = GetSimChartInfo(chartType, topology, ess);

            if (chartInfo.Axes.Count == 1)
            {
                chartInfo.Axes.ElementAt(0).Title = depAxisTitle;
            }
            else
            {
                for (int i = 0; i < chartInfo.Axes.Count; i++)
                {
                    chartInfo.Axes.ElementAt(i).Title = entries.ElementAt(i).DependantTitle;
                }
            }

            RenderAndSaveChart(entries, chartInfo, Model.SimulationModel.ImageLoc(chartType));

            //We've rendered the points, no need to hold onto them
            foreach (SeriesEntry e in entries)
            {
                e.Points.Clear();
            }
        }

        private void SimGraph_GenerateAll()
        {
            Dispatcher.Invoke((Action)(() =>
            {

                Model.SimulationModel.GraphNeedsUpdate = false;
                bool useDistance = Model.SimulationModel.UseDistance;


                SimBusyIndicator.IsBusy = true;
                var simResults = Model.SimulationModel.Results;

                Model.SimulationModel.TotalWeight = Model.ParamConfigModel.VehicleOption.Parameters.LocomotiveMass +
                                    (Model.ParamConfigModel.VehicleOption.Parameters.CarMass * Model.ParamConfigModel.VehicleOption.Parameters.NumCars);

                double totalFuelUse = Model.SimulationModel.TotalFuelUseMass + Model.SimulationModel.TotalAPUFuelUse;

                Model.SimulationModel.LoadMovedPerKgFuel = totalFuelUse > 0 ? Math.Round(Model.SimulationModel.TotalWeight / totalFuelUse, Constants.NumDecimals) : 0; //units don't matter here

                this.SimBusyIndicator.BusyContent = "Generating Vehicle Speed...";

                //This is for updating the busyIndicator's view.
                //This doesn't help with the fact the progress bar freezes.
                //In order to get around this we'd have to put the BusyIndicator on a separate rendering thread or something
                this.SimBusyIndicator.Dispatcher.Invoke(DispatcherPriority.Render, (Action)delegate() { });

                if (simResults.Count > 0)
                {
                    //Set up the charts
                    string indepTitle = string.Format("{0} ({1})",
                        useDistance ? "Distance" : "Time",
                        useDistance ? ApplicationModel.staticUnitSystem.ShortDistance.Label : "s");

                    SetSimChartIndepTitle(indepTitle);
                    Unit distanceUnit = ApplicationModel.staticUnitSystem.ShortDistance.Unit;
                    //Generate Vehicle Speed
                    this.SimBusyIndicator.BusyContent = "Generating Vehicle Speed...";
                    this.SimBusyIndicator.Dispatcher.Invoke(DispatcherPriority.Render, (Action)delegate() { });

                    SeriesEntry VehicleSpeed = new SeriesEntry(new ObservableCollection<KeyValuePair<double, double>>(
                        simResults.Select(result =>
                            {
                                Speed speed = new Speed(result.Speed * Constants.MpsToKph, Unit.KPH);
                                double indepValue = (useDistance ?
                                        (new Distance(result.DistanceTravelled, Unit.m)).GetValue(distanceUnit) :
                                        result.Time);
                                return new KeyValuePair<double, double>(
                                        indepValue,
                                        speed.GetValue(Model.UnitSystem.Speed.Unit)
                                    );
                            })),
                        "Vehicle Speed");

                    SeriesEntry DriveCycleSpeed = new SeriesEntry(new ObservableCollection<KeyValuePair<double, double>>(
                        Model.DriveCycleModel.DriveCycles.Select(driveCycle =>
                            {
                                Speed speed = new Speed(driveCycle.velocity * Constants.MpsToKph, Unit.KPH);
                                double indepValue = (useDistance ?
                                        (new Distance(driveCycle.distance, Unit.m)).GetValue(distanceUnit) :
                                        driveCycle.time);
                                return new KeyValuePair<double, double>(
                                        indepValue,
                                        speed.GetValue(Model.UnitSystem.Speed.Unit));
                            })),
                        "Drive Cycle Speed");

                        SimGraph_SetGraph(SimulationModel.SimChartType.VehicleSpeed,
                            ApplicationModel.staticTopology,
                            ApplicationModel.staticESS,
                            string.Format("Speed ({0})", ApplicationModel.staticUnitSystem.Speed.Label),
                            indepTitle,
                            DriveCycleSpeed, VehicleSpeed);


                    //Generate Fuel Consumption
                    this.SimBusyIndicator.BusyContent = "Generating Fuel Burn Rate...";
                    this.SimBusyIndicator.Dispatcher.Invoke(DispatcherPriority.Render, (Action)delegate() { });

                    SeriesEntry FuelRate = new SeriesEntry(new ObservableCollection<KeyValuePair<double, double>>(
                        simResults.Select(result =>
                            {
                                Mass rate = new Mass(result.FuelRate, Unit.Kg);
                                double indepValue = (useDistance ?
                                        (new Distance(result.DistanceTravelled, Unit.m)).GetValue(distanceUnit) :
                                        result.Time);
                                return new KeyValuePair<double, double>(
                                        indepValue,
                                        rate.GetValue(ApplicationModel.staticUnitSystem.Mass.Unit)
                                    );
                            })),
                        "Prime Mover Fuel Consumption Rate");


                    SeriesEntry ApuFuelRate = new SeriesEntry(new ObservableCollection<KeyValuePair<double, double>>(
                        simResults.Select(result =>
                            {
                                Speed speed = new Speed(result.Speed * Constants.MpsToKph, Unit.KPH);
                                double indepValue = (useDistance ?
                                        (new Distance(result.DistanceTravelled, Unit.m)).GetValue(distanceUnit) :
                                        result.Time);
                                return new KeyValuePair<double, double>(
                                        indepValue,
                                        result.ApuFuelRate
                                    );
                            })),
                        "Auxiliary Fuel Consumption Rate");

                        SimGraph_SetGraph(SimulationModel.SimChartType.FuelBurnRate,
                            ApplicationModel.staticTopology,
                            ApplicationModel.staticESS,
                            string.Format("Fuel Consumption Rate ({0}/s)", ApplicationModel.staticUnitSystem.Mass.Label),
                            indepTitle,
                            FuelRate, ApuFuelRate);

                    //Generate Engine Speed
                    this.SimBusyIndicator.BusyContent = "Generating Engine Speed...";
                    this.SimBusyIndicator.Dispatcher.Invoke(DispatcherPriority.Render, (Action)delegate() { });

                    SeriesEntry EngineSpeed = new SeriesEntry(new ObservableCollection<KeyValuePair<double, double>>(
                        simResults.Select(result =>
                            {
                                RotationalSpeed engineSpeed = new RotationalSpeed(result.EngineSpeed, Unit.rads);
                                double indepValue = (useDistance ?
                                        (new Distance(result.DistanceTravelled, Unit.m)).GetValue(distanceUnit) :
                                        result.Time);
                                return new KeyValuePair<double, double>(
                                        indepValue,
                                        engineSpeed.GetValue(ApplicationModel.staticUnitSystem.RotationalSpeed.Unit)
                                    );
                            })),
                        string.Format("Engine Speed ({0})", Model.UnitSystem.RotationalSpeed.Label));

                        SimGraph_SetGraph(SimulationModel.SimChartType.EngineSpeed,
                            ApplicationModel.staticTopology,
                            ApplicationModel.staticESS,
                            string.Format("Engine Speed {0}", ApplicationModel.staticUnitSystem.RotationalSpeed.Label),
                            indepTitle,
                            EngineSpeed);

                    //Generate GHG Emissions
                    this.SimBusyIndicator.BusyContent = "Generating GHG Emissions...";
                    this.SimBusyIndicator.Dispatcher.Invoke(DispatcherPriority.Render, (Action)delegate() { });

                    SeriesEntry GHGEmissions = new SeriesEntry(new ObservableCollection<KeyValuePair<double, double>>(
                        simResults.Select(result =>
                            {
                                Speed speed = new Speed(result.Speed * Constants.MpsToKph, Unit.KPH);
                                double indepValue = (useDistance ?
                                        (new Distance(result.DistanceTravelled, Unit.m)).GetValue(distanceUnit) :
                                        result.Time);
                                return new KeyValuePair<double, double>(
                                        indepValue,
                                            result.GHGEmissions);
                            })),
                        string.Format("Cumulative GHG Emissions ({0} CO2 eq.)", Model.UnitSystem.Mass.Label));

                        SimGraph_SetGraph(SimulationModel.SimChartType.GHGEmissions,
                            ApplicationModel.staticTopology,
                            ApplicationModel.staticESS,
                            string.Format("GHG Emissions ({0} CO2 eq.)", ApplicationModel.staticUnitSystem.Mass.Label),
                            indepTitle,
                            GHGEmissions);


                    //Generate Engine power
                    this.SimBusyIndicator.BusyContent = "Generating Engine Power...";
                    this.SimBusyIndicator.Dispatcher.Invoke(DispatcherPriority.Render, (Action)delegate() { });

                    SeriesEntry EnginePower = new SeriesEntry(new ObservableCollection<KeyValuePair<double, double>>(
                        simResults.Select(result =>
                            {
                                Speed speed = new Speed(result.Speed * Constants.MpsToKph, Unit.KPH);
                                double indepValue = (useDistance ?
                                        (new Distance(result.DistanceTravelled, Unit.m)).GetValue(distanceUnit) :
                                        result.Time);
                                return new KeyValuePair<double, double>(
                                        indepValue,
                                            result.EnginePower / 1000);
                            })),
                        "Prime Mover Power (kW)"); //hp?

                    SeriesEntry ESSPower = new SeriesEntry(new ObservableCollection<KeyValuePair<double, double>>(
                        simResults.Select(result =>
                            {
                                double indepValue = (useDistance ?
                                        (new Distance(result.DistanceTravelled, Unit.m)).GetValue(distanceUnit) :
                                        result.Time);
                                return new KeyValuePair<double, double>(
                                        indepValue,
                                            result.ESSPower / 1000);
                            })),
                        "ESS Power (kW)");

                    SeriesEntry TractionPower = new SeriesEntry(new ObservableCollection<KeyValuePair<double, double>>(
                        simResults.Select(result =>
                            {
                                double indepValue = (useDistance ?
                                        (new Distance(result.DistanceTravelled, Unit.m)).GetValue(distanceUnit) :
                                        result.Time);
                                return new KeyValuePair<double, double>(
                                        indepValue,
                                            result.TractivePower / 1000);
                            })),
                        "Tractive Power (kW)");

                    SeriesEntry AuxPower = new SeriesEntry(new ObservableCollection<KeyValuePair<double, double>>(
                        simResults.Select(result =>
                            {
                                double indepValue = (useDistance ?
                                        (new Distance(result.DistanceTravelled, Unit.m)).GetValue(distanceUnit) :
                                        result.Time);
                                return new KeyValuePair<double, double>(
                                        indepValue,
                                            result.ApuPower / 1000);
                            })),
                        "Auxiliary Power (kW)");

                    List<SeriesEntry> powerEntries = new List<SeriesEntry>();
                    powerEntries.Add(EnginePower);
                    powerEntries.Add(TractionPower);
                    if (Model.ParamConfigModel.TrainOption.Topology == TrainTopology.EngineHybrid ||
                        Model.ParamConfigModel.TrainOption.Topology == TrainTopology.FuelCell)
                    {
                        powerEntries.Add(ESSPower);
                    }
                    if (Model.ParamConfigModel.AuxEngineOption.Type != AuxEngineType.None)
                    {
                        powerEntries.Add(AuxPower);
                    }

                    SimGraph_SetGraph(SimulationModel.SimChartType.LocomotivePower,
                    ApplicationModel.staticTopology,
                    ApplicationModel.staticESS,
                        string.Format("Power ({0})", ApplicationModel.staticUnitSystem.Power.Label),
                        indepTitle,
                        powerEntries.ToArray());

                    //Generate AESS
                    this.SimBusyIndicator.BusyContent = "Generating AESS On/Off State...";
                    this.SimBusyIndicator.Dispatcher.Invoke(DispatcherPriority.Render, (Action)delegate() { });

                    SeriesEntry AESSState = new SeriesEntry(new ObservableCollection<KeyValuePair<double, double>>(
                        simResults.Select(result =>
                            {
                                Speed speed = new Speed(result.Speed * Constants.MpsToKph, Unit.KPH);
                                double indepValue = (useDistance ?
                                        (new Distance(result.DistanceTravelled, Unit.m)).GetValue(distanceUnit) :
                                        result.Time);
                                return new KeyValuePair<double, double>(
                                        indepValue,
                                            result.AESSOnOffState);
                            })),
                        string.Format("AESS On/Off State", Model.UnitSystem.Speed.Label));

                        SimGraph_SetGraph(SimulationModel.SimChartType.AESSState,
                            ApplicationModel.staticTopology,
                            ApplicationModel.staticESS,
                            "AESS On/Off State",
                            indepTitle,
                            AESSState);


                    SimGraph_SetImage(Model.SimulationModel.ChartImageLoc);
                    SaveSimResults.Visibility = System.Windows.Visibility.Visible;

                    //SimOutputLoc will not be empty if we already ran a simulation
                    //This file, since running a new simulation, is now invalid and must be deleted
                    if (!string.IsNullOrWhiteSpace(Model.SimulationModel.SimOutputLoc))
                    {
                        if (File.Exists(Model.SimulationModel.SimOutputLoc))
                        {
                            File.Delete(Model.SimulationModel.SimOutputLoc);
                        }
                    }
                }
            }));

        }

        public void SetSimChartIndepTitle(string title)
        {
            this.SimVehicleSpeedChart_IndependantAxis.Title = title;
            this.SimEngineSpeedChart_IndependantAxis.Title = title;
            this.SimLocoPowerChart_IndependantAxis.Title = title;
            this.SimFuelBurnRateChart_IndependantAxis.Title = title;
            this.SimGHGEmissionsChart_IndependantAxis.Title = title;
            this.SimAESSChart_IndependantAxis.Title = title;
        }
        #endregion

        /// <summary>
        /// Renders the provided chart, converts it to an image, and stores it at the specified location
        /// </summary>
        private void RenderAndSaveChart(IEnumerable<SeriesEntry> entries, ChartInfo chartInfo, string imagePath)
        {
            Chart chart = chartInfo.Chart;
            List<LineSeries> series = chartInfo.Series;
            List<LinearAxis> axis = chartInfo.Axes;

            if (entries != null && entries.Count() > 0)
            {
                if (entries.Count() > 1)
                {
                    ToggleLegend(chart, true);
                }
                chart.Visibility = System.Windows.Visibility.Visible;
                for (int i = 0; i < series.Count; i++)
                {
                    LineSeries serie = series.ElementAt(i);

                    serie.Visibility = System.Windows.Visibility.Visible;
                    ToggleLegendItem(serie, true);

                    BindingOperations.ClearBinding(serie, LineSeries.ItemsSourceProperty);
                    serie.ClearValue(LineSeries.ItemsSourceProperty);
                }
                for (int i = 0; i < axis.Count; i++)
                {
                    LinearAxis axi = axis.ElementAt(i);

                    axi.Visibility = System.Windows.Visibility.Visible;
                }
                /*
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                */
                for (int i = 0; i < series.Count; i++)
                {
                    if (i < entries.Count())
                    {
                        series.ElementAt(i).ItemsSource = FilterPoints(chart.ActualWidth,
                            chart.ActualHeight,
                            entries.ElementAt(i).Points);
                        series.ElementAt(i).Title = entries.ElementAt(i).DependantTitle;
                    }
                    else
                    {
                        series.ElementAt(i).Visibility = System.Windows.Visibility.Hidden;
                        ToggleLegendItem(series.ElementAt(i), false);

                        //if we have multiple axis defined
                        if (i < axis.Count())
                        {
                            axis.ElementAt(i).Visibility = System.Windows.Visibility.Hidden;
                        }

                    }
                }

                chart.UpdateLayout();

                RenderChartToImage(chart, imagePath);

                //We've rendered the points, no need to hold onto them
                chart.Visibility = System.Windows.Visibility.Hidden;
                foreach (var serie in series)
                {
                    serie.Visibility = System.Windows.Visibility.Hidden;
                }
                ToggleLegend(chart, false);

            }
        }

        /// <summary>
        /// Takes a chart object and renders it to an image in the specified location
        /// </summary>
        /// <param name="chart">The chart to render</param>
        /// <param name="imagePath">Physical location (in temp) to render the chart image to</param>
        private static void RenderChartToImage(Chart chart, string imagePath)
        {
            int dpi = 144;
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)chart.ActualWidth * dpi / 96, (int)chart.ActualHeight * dpi / 96, dpi, dpi, PixelFormats.Pbgra32);

            Size size = new Size(chart.ActualWidth, chart.ActualHeight);
            Rectangle rect = new Rectangle() { Width = chart.ActualWidth, Height = chart.ActualHeight, Fill = new VisualBrush(chart) };
            rect.Measure(size);
            rect.Arrange(new Rect(size));
            rect.UpdateLayout();

            bmp.Render(rect);

            PngBitmapEncoder enc = new PngBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(bmp));
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
            using (var s = File.OpenWrite(imagePath))
            {
                enc.Save(s);
            }
        }

        #region Set To Default button click events

        /// <summary>
        /// This is called when the default button for vehicle options is clicked.
        /// It sets the parameter's VehicleOption object back to its defaults (but keeps the type as custom so as to keep the UI items visible)
        /// </summary>
        private void VehicleDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            Model.ParamConfigModel.VehicleOption = new VehicleOptions();
        }

        /// <summary>
        /// This is called when the default button for vehicle options is clicked.
        /// It sets the parameter's VehicleOption object back to its defaults (but keeps the type as custom so as to keep the UI items visible)
        /// </summary>
        private void VehicleDefaultFreightButton_Click(object sender, RoutedEventArgs e)
        {
            Model.ParamConfigModel.VehicleOption = new VehicleOptions(VehicleOptions.VehicleType.Freight);
        }

        /// <summary>
        /// This is called when the default button for vehicle options is clicked.
        /// It sets the parameter's VehicleOption object back to its defaults (but keeps the type as custom so as to keep the UI items visible)
        /// </summary>
        private void VehicleDefaultSwitchButton_Click(object sender, RoutedEventArgs e)
        {
            Model.ParamConfigModel.VehicleOption = new VehicleOptions(VehicleOptions.VehicleType.Switch);
        }

        /// <summary>
        /// This is called when the default button for vehicle options is clicked.
        /// It sets the parameter's VehicleOption object back to its defaults (but keeps the type as custom so as to keep the UI items visible)
        /// </summary>
        private void VehicleDefaultPassengerButton_Click(object sender, RoutedEventArgs e)
        {
            Model.ParamConfigModel.VehicleOption = new VehicleOptions(VehicleOptions.VehicleType.Passenger);
        }

        /// <summary>
        /// This is called when the default button for powertrain options is clicked.
        /// It sets the parameter's PowerTrain object back to its defaults (but keeps the type as custom so as to keep the UI items visible)
        /// </summary>
        private void PowertrainDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            Model.ParamConfigModel.PowertrainOption = new PowerTrainOptions();
        }

        /// <summary>
        /// This is called when the default button for auxEngine options is clicked.
        /// It sets the parameter's AuxEngineOptions object back to its defaults (but keeps the type as custom so as to keep the UI items visible)
        /// </summary>
        private void AuxEngineDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            Model.ParamConfigModel.AuxEngineOption = new AuxEngineOptions();
        }
        #endregion

        /// <summary>
        /// Event fired when the SaveSimResults button is clicked.
        /// This will simply copy the output file from the simulation
        /// </summary>
        private void SaveSimResults_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "SimulationResults";
            dlg.DefaultExt = ".mat";
            dlg.Filter = "MATLab file (.mat)|*.mat|Comma-Separated Values File (.csv)|*.csv";
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                string outputFileName = dlg.FileName;
                string extension = System.IO.Path.GetExtension(outputFileName);
                if (extension.Contains("mat"))
                {
                    File.Copy(Model.SimulationModel.SimOutputLoc, outputFileName, true);
                }
                else
                {
                    File.Copy(System.IO.Path.ChangeExtension(Model.SimulationModel.SimOutputLoc, ".csv"), outputFileName, true);
                }
            }
        }

        /// <summary>
        /// Method used for filtering out data points so we're not putting as heavy of a load onto the charting framework
        /// </summary>
        /// <param name="chartWidth">Width of the chart, in pixels</param>
        /// <param name="chartHeight">height of the chart, in pixels</param>
        /// <param name="points">The full set of data points</param>
        private ObservableCollection<KeyValuePair<double, double>> FilterPoints(double chartWidth, double chartHeight, ObservableCollection<KeyValuePair<double, double>> points)
        {
            ObservableCollection<KeyValuePair<double, double>> filteredPoints = new ObservableCollection<KeyValuePair<double, double>>();

            //We need to convert logical distance to physical distance ( difference in values vs difference in pixels)
            double xMax = points.Select(point => point.Key).Max();
            double yMax = points.Select(point => point.Value).Max();
            double xRatio = chartWidth / (xMax == 0 ? 1 : xMax);
            double yRatio = chartHeight / (yMax == 0 ? 1 : yMax);

            filteredPoints.Add(points.ElementAt(0));
            foreach (KeyValuePair<double, double> pair in points)
            {
                //We need to check if the current pair is within the specified threshold of the last added pair
                KeyValuePair<double, double> latestPair = filteredPoints.Last();

                //we're only concerned with the distance between the two

                double distance = ((xRatio * xRatio) * ((latestPair.Key * latestPair.Key) - (2 * latestPair.Key * pair.Key) + (pair.Key * pair.Key))) +
                                ((yRatio * yRatio) * ((latestPair.Value * latestPair.Value) - (2 * latestPair.Value * pair.Value) + (pair.Value * pair.Value)));
                if (distance >= Constants.FilterThreshold)
                {
                    filteredPoints.Add(new KeyValuePair<double, double>(Math.Round(pair.Key, 4), Math.Round(pair.Value, 4)));
                }
            }
            return filteredPoints;
        }

        /// <summary>
        /// Sets a dependent linear axis (the y axis) to have the default settings we like)
        /// </summary>
        /// <param name="axis"></param>
        private void SetToDependentAxisDefaults(LinearAxis axis)
        {
            axis.Orientation = AxisOrientation.Y;
            axis.ShowGridLines = true;
            axis.Minimum = 0;
        }

        /// <summary>
        /// Function for showing or hiding the styling of a legend for a chart.
        /// BasicChart has the legend shown, NoLengend is a style based on BasicChart, but hides the legend
        /// </summary>
        /// <param name="chart">Chart to apply the styling to</param>
        /// <param name="showLegend">Whether or not to show the legend</param>
        private void ToggleLegend(Chart chart, bool showLegend)
        {
            if (showLegend)
            {
                chart.Style = (Style)Resources["BasicChart"];
            }
            else
            {
                chart.Style = (Style)Resources["NoLegend"];
            }
        }

        /// <summary>
        /// Function for showing or hiding the styling of a legendItem for a chart.
        /// null SHOULD end up as the default, HideLegendItem sets the visibility of the legendItem to collapsed
        /// </summary>
        /// <param name="chart">Chart to apply the styling to</param>
        /// <param name="showItem">Whether or not to show the legend</param>
        private void ToggleLegendItem(LineSeries series, bool showItem)
        {
            if (showItem)
            {
                series.LegendItemStyle = null;
            }
            else
            {
                series.LegendItemStyle = (Style)Resources["HideLegendItem"];
            }
        }



        public ChartInfo GetCycleChartInfo(DriveCycleModel.CycleChartType chartType)
        {
            Chart chart;
            List<LineSeries> series = new List<LineSeries>();
            List<LinearAxis> axes = new List<LinearAxis>();
            switch (chartType)
            {
                case DriveCycleModel.CycleChartType.AuxLoad:
                    chart = this.CycleAuxChart;
                    series.Add(this.CycleAuxChart_Line0);
                    series.Add(this.CycleAuxChart_Line1);
                    axes.Add(this.CycleAuxChart_DependantAxis0);
                    axes.Add(this.CycleAuxChart_DependantAxis1);
                    break;
                case DriveCycleModel.CycleChartType.Grade:
                    chart = this.CycleGradeChart;
                    series.Add(this.CycleGradeChart_Line0);
                    axes.Add(this.CycleGradeChart_DependantAxis0);
                    break;
                case DriveCycleModel.CycleChartType.Speed:
                default:
                    chart = this.CycleSpeedChart;
                    series.Add(this.CycleSpeedChart_Line0);
                    axes.Add(this.CycleSpeedChart_DependantAxis0);
                    break;
            }

            return new ChartInfo(chart, axes, series);
        }

        public ChartInfo GetSimChartInfo(SimulationModel.SimChartType chartType, TrainTopology topology, EnergyStorageSystem ess)
        {
            Chart chart;
            List<LineSeries> series = new List<LineSeries>();
            List<LinearAxis> axes = new List<LinearAxis>();
            switch (chartType)
            {
                case SimulationModel.SimChartType.AESSState:
                    chart = this.SimAESSChart;

                    series.Add(this.SimAESSChart_Line0);
                    axes.Add(this.SimAESSChart_DependantAxis0);
                    break;
                case SimulationModel.SimChartType.EngineSpeed:
                    chart = this.SimEngineSpeedChart;

                    series.Add(this.SimEngineSpeedChart_Line0);
                    axes.Add(this.SimEngineSpeedChart_DependantAxis0);
                    break;
                case SimulationModel.SimChartType.FuelBurnRate:
                    chart = this.SimFuelBurnRateChart;

                    series.Add(this.SimFuelBurnRateChart_Line0);
                    axes.Add(this.SimFuelBurnRateChart_PrimeMoverConsumption);

                    series.Add(this.SimFuelBurnRateChart_Line1);
                    break;
                case SimulationModel.SimChartType.GHGEmissions:
                    chart = this.SimGHGEmissionsChart;

                    series.Add(this.SimGHGEmissionsChart_Line0);
                    axes.Add(this.SimGHGEmissionsChart_GHGEmissions);
                    break;
                case SimulationModel.SimChartType.LocomotivePower:
                    chart = this.SimLocoPowerChart;

                    series.Add(this.SimLocoPowerChart_Line0);
                    axes.Add(this.SimLocoPowerChart_PrimeMover);

                    series.Add(this.SimLocoPowerChart_Line1);
                    series.Add(this.SimLocoPowerChart_Line2);
                    series.Add(this.SimLocoPowerChart_Line3);
                    break;
                case SimulationModel.SimChartType.VehicleSpeed:
                default:
                    chart = this.SimVehicleSpeedChart;

                    series.Add(this.SimVehicleSpeedChart_Line0);
                    axes.Add(this.SimVehicleSpeedChart_DriveCycleSpeed);

                    series.Add(this.SimVehicleSpeedChart_Line1);
                    /*chart = this.SimVehicleSpeedChart;
                    LineSeries temp = new LineSeries();
                    temp.Title = "Test";
                    temp.IndependentValueBinding = new Binding("Key");
                    temp.DependentValueBinding = new Binding("Value");
                    series.Add(temp*/
                    break;
            }

            return new ChartInfo(chart, axes, series);
        }

        private void MetricMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            if (this.ImperialMenuItem != null)
            {
                this.ImperialMenuItem.IsChecked = false;
            }
            if (this.MetricMenuItem.IsChecked && Model != null)
            {
                Model.UnitSystem = new MetricSystem();

                SetGraphsNeedUpdate();
            }
        }

        private void ImperialMenuItem_Checked(object sender, RoutedEventArgs e)
        {
            if (this.MetricMenuItem != null)
            {
                this.MetricMenuItem.IsChecked = false;
            }
            if (this.ImperialMenuItem.IsChecked && Model != null)
            {
                Model.UnitSystem = new USImperialSystem();

                SetGraphsNeedUpdate();
            }
        }

        private void MetricMenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!this.ImperialMenuItem.IsChecked)
            {
                //Check this if ImperialMenuItem isn't checked
                this.MetricMenuItem.IsChecked = true;
            }
        }

        private void ImperialMenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!this.MetricMenuItem.IsChecked)
            {
                //Check this if MetricMenuItem isn't checked
                this.ImperialMenuItem.IsChecked = true;
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                TabControl tabControl = sender as TabControl;
                TabItem item = tabControl.SelectedItem as TabItem;
                if (item != null)
                {

                }
            }
        }
        private void SetGraphsNeedUpdate()
        {
            Model.DriveCycleModel.GraphNeedsUpdate = true;

            Model.SimulationModel.GraphNeedsUpdate = true;

        }

        private void Reload_Graphs(object sender, RoutedEventArgs e)
        {
            if (DriveCycleTab.IsSelected)
            {
                GenerateDriveCycleGraphs(Model.DriveCycleModel.DriveCycles);
            }
            if (SimulationTab.IsSelected)
            {

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (o, ea) =>
                {
                    Thread.Sleep(1000);
                    SimGraph_GenerateAll();
                };
                worker.RunWorkerCompleted += (o, ea) =>
                {
                    Model.SimulationModel.GraphNeedsUpdate = false;
                    SimBusyIndicator.IsBusy = false;
                };

                SimBusyIndicator.IsBusy = true; ;
                worker.RunWorkerAsync();
            }
        }
    }
}
