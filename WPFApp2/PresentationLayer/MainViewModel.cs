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

namespace TourFinder
{
    public class MainViewModel : INotifyPropertyChanged
    {

        private static string IMGDIR;
        private string _description;
        private string _input;
        private string _imagePath;
        private Tour _selectedTour;
        private Window PopOutWindow;

        private BusinessLayerManager BLM = new BusinessLayerManager();

        public event PropertyChangedEventHandler PropertyChanged;

        //Commands
        public ICommand ExecuteSearch { get; }
        public ICommand ExecuteTourListBox { get; }

        public ICommand ExecuteOpenAddTourWindow { get; }
        public ICommand ExecuteOpenTourUpdateWindow{get;}

        public ICommand ExecuteDeleteTour { get; }
        public ICommand ExecuteAddTour { get; }
        public ICommand ExecuteUpdateTour { get; }
        public ICommand ExecuteCopyTour { get; }
        


        /* Probably need to delete this later*/

        public ObservableCollection<Tour> Tourlist { get; set; } = new ObservableCollection<Tour>();
        public ObservableCollection<Log> DataGridLogList { get; set; } = new ObservableCollection<Log>();
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
        public TourAddUtility TourAddUtilityProperty { get; } = new TourAddUtility();
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
                return _imagePath;
            }
            set
            {
                if(_imagePath != value)
                {
                    _imagePath = IMGDIR + value;
                    OnPropertyChanged(nameof(ImagePath));
                }
            }
        }



        /* Functions from here on */

        public MainViewModel()
        {
            IMGDIR = ConfigurationManager.AppSettings["ImageFolder"].ToString();


            // https://docs.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern#id0090030
            this.ExecuteTourListBox = new RelayCommand((_) => Description = TourSelection.Name);


            //execute function with no return value:
            this.ExecuteOpenAddTourWindow = new RelayCommand((_) => OpenAddTourWindow());
            this.ExecuteOpenTourUpdateWindow = new RelayCommand((_) => OpenUpdateTourWindow());


            this.ExecuteDeleteTour = new RelayCommand((_) => DeleteTour());
            this.ExecuteAddTour = new RelayCommand((_) => AddTour());
            this.ExecuteCopyTour = new RelayCommand((_) => CopyTour());
            this.ExecuteUpdateTour = new RelayCommand((_) => UpdateTour());

            //To fill list with saved tours from DataLayer
            FillTourList();
        }

        
        public void FillTourList()
        {
            Tourlist = new ObservableCollection<Tour>(BLM.GetAllToursFromDB());
        }


        //CRUD TOUR COMMAND FUNCTIONS
        public void AddTour()
        {
            Tour tmpTour = BLM.CreateNewTour(TourAddUtilityProperty.Name, TourAddUtilityProperty.StartLocation, TourAddUtilityProperty.EndLocation, TourAddUtilityProperty.Description);
            ImagePath = tmpTour.MapImagePath;
            Tourlist.Add(tmpTour);
            PopOutWindow.Close();
        }

        public void UpdateTour()
        {
            if (TourSelection != null)
                Tourlist = new ObservableCollection<Tour>(BLM.UpdateTourGetList(TourSelection, TourAddUtilityProperty.Name, TourAddUtilityProperty.Description));
            PopOutWindow.Close();
        }

        public void CopyTour()
        {
            if (TourSelection != null)
                Tourlist = new ObservableCollection<Tour>(BLM.CopyTourGetList(TourSelection));
        }

        public void DeleteTour()
        {
            if (TourSelection != null)
                Tourlist = new ObservableCollection<Tour>(BLM.DeleteTourGetList(TourSelection));
        }

        //CRUD LOG COMMAND FUNCTIONS
        public void AddLog()
        {
            throw new NotImplementedException();
            PopOutWindow.Close();
        }

        public void UpdateLog()
        {
            throw new NotImplementedException();
        }

        public void CopyLog()
        {
            throw new NotImplementedException();
        }

        public void DeleteLog()
        {
            throw new NotImplementedException();
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
        }

        public void OpenUpdateTourWindow()
        {
            if(TourSelection != null)
            {
                PopOutWindow = new UpdateTourWindow { DataContext = this };
                PopOutWindow.Show();
            }
        }

        public void OpenAddLogWindow()
        {
            throw new NotImplementedException();
            //PopOutWindow = new AddLogWindow();
            PopOutWindow.Show();
        }

        public void OpenUpdateLogWindow()
        {
            throw new NotImplementedException();
            //PopOutWindow = new UpdateLogWindow();
            PopOutWindow.Show();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Debug.Print($"propertyChanged \"{propertyName}\"");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
