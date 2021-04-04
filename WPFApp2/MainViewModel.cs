using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using System;
using TourFinder.BackendStuff.DB;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TourFinder
{
    public class MainViewModel : INotifyPropertyChanged
    {

        private static string IMGDIR = "D:\\Documents\\_Mein Stuff\\ProgrammingStuff\\FH_SS2021\\SWEI\\WPFApp2\\WPFApp2\\img\\maps\\";
        private string _output = "AAAAAAAAA";
        private string _input;
        private Tour _selectedTour;
        private string _imagePath;
        //Random rand = new Random();
        private Window PopOutWindow;

        public ICommand ExecuteSearch { get; }
        public ICommand ExecuteTourListBox { get; }
        public event PropertyChangedEventHandler PropertyChanged;


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
                    DataGridLogList = _selectedTour.LogList;
                    Output = value.Name + "\n" + value.Description + "\n" + value.Distance;
                    ImagePath = _selectedTour.MapImagePath;
                    OnPropertyChanged(nameof(DataGridLogList));
                    OnPropertyChanged(nameof(TourSelection));
                    OnPropertyChanged(nameof(ImagePath));
                }
            }
        }

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

        public MainViewModel()
        {

            // Alternative: https://docs.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern#id0090030
            this.ExecuteTourListBox = new RelayCommand((_) => Output = TourSelection.Name);
            this.ExecuteSearch = new RelayCommand((_) => Output = Input);

            //execute function with no return value:
            this.ExecuteSearch = new RelayCommand((_) => GetRouteGetMap());
        }

        public async void GetRouteGetMap()
        {
            RestDataClass rb = RestDataClass.Instance();
            ImagePath = await rb.GetRouteSaveImg();
            Output = ImagePath;
            Debug.Print("WE DID IT BOIS WE GOT EM \n" + ImagePath + "\n" + _imagePath);
        }

        public void OpenAddWindow()
        {
            Debug.Print("AddTour Window Opened");
            PopOutWindow = new AddTourWindow();
            PopOutWindow.Show();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Debug.Print($"propertyChanged \"{propertyName}\"");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            //MessageBox.Show();
        }
        

    }
}
