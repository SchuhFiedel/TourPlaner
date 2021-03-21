using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using System;
using TourFinder.BackendStuff.DB;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace TourFinder
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _output = "Hello There!";
        private string _input;
        private Tour _selectedTour;
        //Random rand = new Random();


        /* Probably need to delete this later*/
        public ObservableCollection<Log> loglist { get; set; } = new ObservableCollection<Log>() {
                                                                                                   new Log() { Date = "0.0.0", Distance= 50, Duration="0.0", Feeling="Nice"},
                                                                                                   new Log() { Date = "11111", Distance= 15155, Duration="1.2.23.0"}
                                                                                                  };
        public ObservableCollection<Tour> tourlist { get; set; } = new ObservableCollection<Tour>() {
                                                                                                      new Tour() { Name = "Tour1" , Distance = 15, Description = "Nice and long"},
                                                                                                      new Tour() { Name = "Tour2" , Description = "Nice and short"}, 
                                                                                                      new Tour() { Name = "Tour3" , Description = "Not nice"} 
                                                                                                    };

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
                    Output = value.Name + "\n" + value.Description + "\n" + value.Distance;
                    OnPropertyChanged(nameof(TourSelection));
                }
            }
        }

        public string Input
        {
            get
            {
                Debug.Print("read Input");
                return _input;
            }
            set
            {
                Debug.Print("write Input");
                if (Input != value)
                {
                    Debug.Print("set Input-value");
                    _input = value;

                    // it does not work to fire an event from outside in C#
                    // can be achieved by creating a method like "RaiseCanExecuteChanged".
                    // this.ExecuteCommand.CanExecuteChanged?.Invoke(this, EventArgs.Empty);

                    // this triggers the UI and the ExecuteCommand
                    Debug.Print("fire propertyChanged: Input");
                    OnPropertyChanged(nameof(Input));
                }
            }
        }

        public string Output
        {
            get
            {
                Debug.Print("read Output");
                return _output;
            }
            set
            {
                Debug.Print("write Output");
                if (_output != value)
                {
                    Debug.Print("set Output");
                    
                    _output = value;
                    Debug.Print("fire propertyChanged: Output");
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ExecuteSearch { get; }
        public ICommand ExecuteTourListBox { get; }

        public event PropertyChangedEventHandler PropertyChanged;



        public MainViewModel()
        {

            // Alternative: https://docs.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern#id0090030
            this.ExecuteTourListBox = new RelayCommand((_) => Output = TourSelection.Name);
            this.ExecuteSearch = new RelayCommand((_) => Output = Input);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Debug.Print($"propertyChanged \"{propertyName}\"");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            //MessageBox.Show();
        }
    }
}
