using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using TourFinder.Models;
using System.Configuration;
using TourFinder.BusinessLayer;
using System.Collections.Generic;

namespace TourFinder
{
    public class MainViewModel : INotifyPropertyChanged
    {

        private static string IMGDIR ;
        private string _output;
        private string _input;
        private string _imagePath;
        private Tour _selectedTour;
        private Window PopOutWindow;

        private BusinessLayerManager BLM = new BusinessLayerManager();

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ExecuteSearch { get; }
        public ICommand ExecuteTourListBox { get; }
        public ICommand ExecuteOpenAddWindow { get; }
        public ICommand ExecuteDeleteTour { get; }
        public ICommand ExecuteAddTour { get; }
        


        /* Probably need to delete this later*/

        public ObservableCollection<Tour> Tourlist { get; set; } = new ObservableCollection<Tour>() {
            new Tour() { Name = "Tour1" , Distance = 15, Description = "Nice and long", LogList = new ObservableCollection<Log>() {
                                                                                        new Log() { Date = "0.0.0", Distance= 50, Duration="0.0", Feeling="Nice"},
                                                                                        new Log() { Date = "11111", Distance= 15155, Duration="1.2.23.0"}
                                                                                        }, MapImagePath = "map0.jpg"
            },
            new Tour() { Name = "Tour2" , Distance = 5, Description = "Nice and short", LogList = new ObservableCollection<Log>() {
                                                                                        new Log() { Date = "0.0.0", Distance= 50, Duration="0.0", Feeling="Nice"},
                                                                                        new Log() { Date = "11111", Distance= 15155, Duration="1.2.23.0"} 
                                                                                        }, MapImagePath = "map1.jpg"
            },
            new Tour() { Name = "Tour3" , Description = "Not nice", MapImagePath = "map2.jpg"}
        };
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
                        Output = value.Name + "\n" + value.Description + "\n" + value.Distance + "\n" + value.MapImagePath;
                        ImagePath = _selectedTour.MapImagePath;
                    }
                    else
                    {
                        DataGridLogList = null;
                        Output = null;
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
        public string Output
        {
            get
            {
                return _output;
            }
            set
            {
                if (_output != value)
                {
                    _output = value;
                    OnPropertyChanged(nameof(Output));
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
            this.ExecuteTourListBox = new RelayCommand((_) => Output = TourSelection.Name);

            //execute function with no return value:
            // this.ExecuteSearch = new RelayCommand((_) => GetRouteGetMap());
            this.ExecuteOpenAddWindow = new RelayCommand((_) => OpenAddWindow());
            this.ExecuteDeleteTour = new RelayCommand((_) => DeleteTour());
            this.ExecuteAddTour = new RelayCommand((_) => AddTour());

            //TEST
            GetAllSavedTours();
        }

        //TEST
        public void GetAllSavedTours()
        {
            List<Tour> tmplist = BLM.GetAllToursFromDB();
            foreach (Tour tour in tmplist)
            {
                Tourlist.Add(tour);
            }
        }
        //TEST

        public void AddTour()
        {
            Tour tmpTour = BLM.CreateNewTour(TourAddUtilityProperty.Name, TourAddUtilityProperty.StartLocation, TourAddUtilityProperty.EndLocation, TourAddUtilityProperty.Description);
            ImagePath = tmpTour.MapImagePath;
            Tourlist.Add(tmpTour);   
        }

        public void DeleteTour()
        {
// TO-DO
            if(TourSelection != null)
            {
                MessageBox.Show("DELETE TOUR: \n" + TourAddUtilityProperty.Name + 
                                             "\n" + TourAddUtilityProperty.Description + 
                                             "\n" + TourAddUtilityProperty.StartLocation + 
                                             "\n" + TourAddUtilityProperty.EndLocation);
            }
        }

        public void OpenAddWindow()
        {
            //Debug.Print("AddTour Window Opened");
            PopOutWindow = new AddTourWindow{DataContext = this};
            PopOutWindow.Show();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Debug.Print($"propertyChanged \"{propertyName}\"");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
