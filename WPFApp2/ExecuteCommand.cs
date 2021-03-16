using System;
using System.Diagnostics;
using System.Windows.Input;

namespace TourFinder
{
    public class ExecuteCommand : ICommand
    {
        private readonly MainViewModel _mainViewModel;
        public event EventHandler CanExecuteChanged;

        public ExecuteCommand(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _mainViewModel.PropertyChanged += (sender, args) =>
            {
                Debug.Print("command: reveived prop changed");
                if (args.PropertyName == "Input")
                {
                    Debug.Print("command: reveived prop changed of Input");
                    CanExecuteChanged.Invoke(this, EventArgs.Empty);
                }
            };
        }

        public bool CanExecute(object parameter)
        {
            Debug.Print("command: can execute");
            return !string.IsNullOrWhiteSpace(_mainViewModel.Input);
        }

        public void Execute(object parameter)
        {
            Debug.Print("command: execute");
            _mainViewModel.Output = $"{_mainViewModel.Input}!";
            _mainViewModel.Input = string.Empty;
            Debug.Print("command: execute done");
        }
    }
}
