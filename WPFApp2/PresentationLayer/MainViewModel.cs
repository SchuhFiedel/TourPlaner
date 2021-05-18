using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using TourFinder.Models;
using System.Configuration;
using TourFinder.BusinessLayer;
using System;
using System.Windows.Media.Imaging;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace TourFinder
{
    public class MainViewModel : INotifyPropertyChanged
    {

        private static string IMGDIR;
        private static string FALLBACKIMG;
        BitmapImage _tourImage;
        private string _description;
        private string _input;
        private string _imagePath;
        private int _savedImagesCounter = 0;
        private Tour _selectedTour;
        private Log _logSelection;
        private Window PopOutWindow;
        private ObservableCollection<Tour> _tourlist;
        private ObservableCollection<Log> _dataGridLogList;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("MainViewModel.cs");
        

        private BusinessLayerManager BLM = new BusinessLayerManager();

        public event PropertyChangedEventHandler PropertyChanged;


        #region Commands
        public ICommand ExecuteSearch { get; }

        //TourCommands
        public ICommand ExecuteTourListBox { get; }
        public ICommand ExecuteOpenAddTourWindow { get; }
        public ICommand ExecuteOpenTourUpdateWindow{get;}
        public ICommand ExecuteDeleteTour { get; }
        public ICommand ExecuteAddTour { get; }
        public ICommand ExecuteUpdateTour { get; }
        public ICommand ExecuteCopyTour { get; }

        //LogCommands
        public ICommand ExecuteOpenAddLogWindow { get; }
        public ICommand ExecuteOpenUpdateLogWindow { get; }
        public ICommand ExecuteAddLog { get; }
        public ICommand ExecuteUpdateLog { get; }
        public ICommand ExecuteCopyLog { get; }
        public ICommand ExecuteDeleteLog { get; }

        //Export Import Commands
        public ICommand ExecuteImportTours { get; }
        public ICommand ExecuteExportTours { get; }

        //PDF Commands
        public ICommand ExecutePrintTourInfo { get; }
        public ICommand ExecutePrintAllTourInfo { get; }
        public ICommand ExecutePrintSummary { get; }

        #endregion

        //Other Properties
        public ObservableCollection<Tour> Tourlist {
            get
            {
                return _tourlist;
            }
            set
            {
                if (_tourlist != value)
                {
                    _tourlist = value;
                    OnPropertyChanged(nameof(Tourlist));
                }
            }
        }
        public ObservableCollection<Log> DataGridLogList {
            get
            {
                return _dataGridLogList;
            }
            set 
            {   
                if(_dataGridLogList != value)
                {
                    _dataGridLogList = value;
                    OnPropertyChanged(nameof(DataGridLogList));
                }
            }
        }
        public Tour TourSelection
        {
            get
            {
                return _selectedTour;
            }
            set
            {
                if(_selectedTour != value)
                {
                    _selectedTour = value;
                    if(_selectedTour != null)
                    {
                        DataGridLogList = _selectedTour.LogList;
                        Description = value.Name + "\n" + value.Description + "\n" + value.Distance + "km\n" + value.MapImagePath;
                        ImagePath = _selectedTour.MapImagePath;

                        TourAddUtilityProperty.Name = _selectedTour.Name;
                        TourAddUtilityProperty.Description = _selectedTour.Description;
                        OnPropertyChanged(nameof(TourAddUtilityProperty));
                    }
                    else
                    {
                        DataGridLogList = null;
                        Description = null;
                        ImagePath = null;
                    }
                    OnPropertyChanged(nameof(DataGridLogList));
                    OnPropertyChanged(nameof(TourSelection));
                    OnPropertyChanged(nameof(ImagePath));
                }
            }
        }
        public Log LogSelection
        {
            get
            {
                return _logSelection;
            }
            set
            {
                if(_logSelection != value)
                {
                    _logSelection = value;

                    if (value != null)
                        LogAddUtility = value;
                    else 
                        LogAddUtility = new Log();

                    OnPropertyChanged(nameof(LogSelection));
                }
            }
        }
        public TourAddUtility TourAddUtilityProperty { get; } = new TourAddUtility();
        public Log LogAddUtility { get; set; } = new Log();
        public string Input
        {
            get
            {
                return _input;
            }
            set
            {
                if (Input != value)
                {
                    _input = value;
                    //this.ExecuteSearch.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                    // this triggers the UI and the ExecuteCommand
                    OnPropertyChanged(nameof(Input));
                }
            }
        }
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }
        public string ImagePath
        {
            get
            {
                if (_imagePath == null)
                {
                    _imagePath = FALLBACKIMG;

                    
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = new Uri(_imagePath);
                    image.EndInit();
                    TourImage = image;
                    OnPropertyChanged(nameof(TourImage));
                    OnPropertyChanged(nameof(ImagePath));
                }

                return _imagePath;
            }
            set
            {
                if(_imagePath != value && value != null && value != FALLBACKIMG)
                {
                    _imagePath = IMGDIR + value;
                    
                }
                try
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = new Uri(_imagePath);
                    image.EndInit();
                    TourImage = image;
                }
                catch(Exception e)
                {
                    _imagePath = FALLBACKIMG;
                    log.Info(String.Format("No Image Found for Tour: {0} - Using Fallback Image \n Error: {1}", TourSelection.Name, e.Message));
                }
                

                OnPropertyChanged(nameof(TourImage));
                OnPropertyChanged(nameof(ImagePath));
            }
        }
        public BitmapImage TourImage
        {
            get
            {
                return _tourImage;
            }
            set
            {
                if(_tourImage != value)
                {
                    _tourImage = value;
                    OnPropertyChanged(nameof(TourImage));
                }
            }
        }


        /* Functions from here on */

        public MainViewModel()
        {
            IMGDIR = ConfigurationManager.AppSettings["ImageFolder"].ToString();
            FALLBACKIMG = ConfigurationManager.AppSettings["FallbackImage"].ToString();

            // https://docs.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern#id0090030
            this.ExecuteTourListBox = new RelayCommand((_) => Description = TourSelection.Name);

            this.ExecuteSearch = new RelayCommand((_) => Search());

            //Tour Commands
            this.ExecuteOpenAddTourWindow = new RelayCommand((_) => OpenAddTourWindow());
            this.ExecuteOpenTourUpdateWindow = new RelayCommand((_) => OpenUpdateTourWindow());

            this.ExecuteDeleteTour = new RelayCommand((_) => DeleteTour());
            this.ExecuteAddTour = new RelayCommand((_) => AddTour());
            this.ExecuteCopyTour = new RelayCommand((_) => CopyTour());
            this.ExecuteUpdateTour = new RelayCommand((_) => UpdateTour());

            //Log Commands
            this.ExecuteOpenAddLogWindow = new RelayCommand((_) => OpenAddLogWindow());
            this.ExecuteOpenUpdateLogWindow = new RelayCommand((_) => OpenUpdateLogWindow());

            this.ExecuteAddLog = new RelayCommand((_) => AddLog());
            this.ExecuteUpdateLog = new RelayCommand((_) => UpdateLog());
            this.ExecuteCopyLog = new RelayCommand((_) => CopyLog());
            this.ExecuteDeleteLog = new RelayCommand((_) => DeleteLog());

            //Export Import Commands
            this.ExecuteExportTours = new RelayCommand((_) => ExportTours());
            this.ExecuteImportTours = new RelayCommand((_) => ImportTours());

            //PDF Commands
            ExecutePrintTourInfo = new RelayCommand((_) => TourToPDF());
            ExecutePrintAllTourInfo = new RelayCommand((_) => AllToursToPDF());
            ExecutePrintSummary = new RelayCommand((_) => PDFSummary());

            //To fill list with saved tours from DataLayer
            FillTourList();
        }

        
        public void FillTourList()
        {
            Tourlist = new ObservableCollection<Tour>(BLM.GetAllToursFromDB());
            log.Info(String.Format("Fill Tour List with {0} Items", Tourlist.Count));
        }

        public void Search()
        {
            log.Info(String.Format("Search initiated with Term: {0}", Input));
            Tourlist = new ObservableCollection<Tour>(BLM.Search(Input));
        }


        //CREATE UPDATE COPY DELETE TOUR COMMAND FUNCTIONS
        public void AddTour()
        {
            Tour tmpTour = BLM.CreateNewTour(TourAddUtilityProperty.Name, TourAddUtilityProperty.StartLocation, TourAddUtilityProperty.EndLocation, TourAddUtilityProperty.Description);
            ImagePath = tmpTour.MapImagePath;
            Tourlist.Add(tmpTour);
            PopOutWindow.Close();
            log.Info(String.Format("Added Tour: {0}", tmpTour.Name));
        }

        public void UpdateTour()
        {
            if (TourSelection != null)
            {
                string tmpname = TourSelection.Name;
                Tourlist = new ObservableCollection<Tour>(BLM.UpdateTourGetList(TourSelection, TourAddUtilityProperty.Name, TourAddUtilityProperty.Description));
                log.Info(String.Format("Updated Tour: {0}", tmpname ));
            }
            PopOutWindow.Close();
        }

        public void CopyTour()
        {
            if (TourSelection != null)
            {
                string tmpname = TourSelection.Name;
                Tourlist = new ObservableCollection<Tour>(BLM.CopyTourGetList(TourSelection));
                log.Info(String.Format("Copied Tour: {0}", tmpname));
            }
        }

        public void DeleteTour()
        {
            
            if (TourSelection != null)
            {
                TourAddUtilityProperty.Description = null;
                TourAddUtilityProperty.Distance = default;
                TourAddUtilityProperty.EndLocation = null;
                TourAddUtilityProperty.ImagePath = null;
                TourAddUtilityProperty.Name = null;
                TourAddUtilityProperty.StartLocation = null;

                ImagePath = FALLBACKIMG;
                string tmpname = TourSelection.Name;
                Debug.WriteLine("DeleteTour");
                Tourlist = new ObservableCollection<Tour>(BLM.DeleteTourGetList(TourSelection));
                log.Info(String.Format("Deleted Tour: {0} ", tmpname));
            }
        }

        //CREATE UPDATE COPY DELETE LOG COMMAND FUNCTIONS
        public void AddLog()
        {
            TourSelection.LogList = new ObservableCollection<Log>(BLM.CreateNewLogGetLogList(TourSelection, DateTime.Now.Date.ToShortDateString(), LogAddUtility.Report,LogAddUtility.Distance,LogAddUtility.Duration,LogAddUtility.Rating,LogAddUtility.Steps,LogAddUtility.WeightKG,LogAddUtility.BloodPreassure,LogAddUtility.Feeling,LogAddUtility.Weather));
            log.Info(String.Format("Created new Log for Tour: {0}",TourSelection.ID));
            DataGridLogList = TourSelection.LogList;
            PopOutWindow.Close();
        }

        public void UpdateLog()
        {
            if (LogSelection != null)
            {
                int id = LogSelection.ID;
                TourSelection.LogList = new ObservableCollection<Log>(BLM.UpdateLogGetList(LogSelection, DateTime.Now.Date.ToShortDateString(), LogAddUtility.Report, LogAddUtility.Distance, LogAddUtility.Duration, LogAddUtility.Rating, LogAddUtility.Steps, LogAddUtility.WeightKG, LogAddUtility.BloodPreassure, LogAddUtility.Feeling, LogAddUtility.Weather));
                DataGridLogList = TourSelection.LogList;
                log.Info(String.Format("Updated Log: {0} ", id));
                PopOutWindow.Close();
            }
        }

        public void CopyLog()
        {
            if (LogSelection != null)
            {
                int id = LogSelection.ID;
                TourSelection.LogList = new ObservableCollection<Log>(BLM.CopyLogGetList(LogSelection));
                DataGridLogList = TourSelection.LogList;
                log.Info(String.Format("Copied Log: {0} ", id ));
            }
            
        }

        public void DeleteLog()
        {
            if(LogSelection != null)
            {
                int logid = LogSelection.ID;
                TourSelection.LogList = new ObservableCollection<Log>(BLM.DeleteLogGetList(LogSelection));
                DataGridLogList = TourSelection.LogList;
                log.Info(String.Format("Deleted Log: {0} ", logid));
            }
        }


        //Open Windows
        public void OpenAddTourWindow()
        {
            //Debug.Print("AddTour Window Opened");
            TourAddUtilityProperty.Description = null;
            TourAddUtilityProperty.Distance = default;
            TourAddUtilityProperty.EndLocation = null;
            TourAddUtilityProperty.ImagePath = null;
            TourAddUtilityProperty.Name = null;
            TourAddUtilityProperty.StartLocation = null;
            PopOutWindow = new AddTourWindow{DataContext = this};
            PopOutWindow.Show();
            log.Info(String.Format("Opened Window: {0}", nameof(PopOutWindow)));
        }

        public void OpenUpdateTourWindow()
        {
            if(TourSelection != null)
            {
                PopOutWindow = new UpdateTourWindow { DataContext = this };
                PopOutWindow.Show();
                log.Info(String.Format("Opened Window: {0}", nameof(PopOutWindow)));
            }
        }

        public void OpenAddLogWindow()
        {
            if (TourSelection != null && LogAddUtility != null)
            {
                LogAddUtility.ID = default;
                LogAddUtility.Feeling = "";
                LogAddUtility.Date = "";
                LogAddUtility.BloodPreassure = "";
                LogAddUtility.Distance = default;
                LogAddUtility.Duration = "";
                LogAddUtility.Rating = default;
                LogAddUtility.Report = "";
                LogAddUtility.Steps = default;
                LogAddUtility.Weather = "";
                LogAddUtility.WeightKG = default;
                PopOutWindow = new AddLogWindow { DataContext = this };
                PopOutWindow.Show();
                log.Info(String.Format("Opened Window: {0}", nameof(PopOutWindow)));
            }
        }

        public void OpenUpdateLogWindow()
        {
            if(LogSelection != null && TourSelection != null && LogSelection.Tour.ID == TourSelection.ID)
            {
                PopOutWindow = new UpdateLogWindow { DataContext = this };
                PopOutWindow.Show();
                log.Info(String.Format("Opened Window: {0}", nameof(PopOutWindow)));
            }
        }

        //Export Import Tours
        public void ExportTours()
        {
            log.Info(String.Format("Export Tours to JSON"));
            BLM.ExportToursToJSON(Tourlist);
        }

        public void ImportTours()
        {
            log.Info(String.Format("Import Tours From JSON"));
            BLM.ImportToursFromJSON();
            Tourlist = new ObservableCollection<Tour>(BLM.GetAllToursFromDB());
        }

        //PDF Functions
        public void TourToPDF()
        {
            if (TourSelection != null)
            {
                log.Info(String.Format("Print Tour PDF of Tour: {0}", TourSelection.ID));
                BLM.PrintTourDataToPDF(TourSelection);
            }
        }

        public void AllToursToPDF()
        {
            log.Info(String.Format("Print Tour PDF of All Tours"));
            BLM.PrintAllToursToPDF(Tourlist);
        }

        public void PDFSummary()
        {
            log.Info(String.Format("Print Tour Summary of All Tour"));
            BLM.PrintSummary(Tourlist);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Debug.Print($"propertyChanged \"{propertyName}\"");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            //Jank I know
            if(propertyName == nameof(ImagePath))
            {
                Debug.WriteLine("DeleteImages");
                _savedImagesCounter = BLM.DeleteUnusedImages(_savedImagesCounter);
            }
        }
    }
}
