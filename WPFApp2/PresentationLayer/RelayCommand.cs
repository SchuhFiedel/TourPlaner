using System;
using System.Windows.Input;

namespace TourFinder
{
    public class RelayCommand : ICommand
    {
        private Action<object> _execute;
        private Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute)); /*Expands to if execute!= null*/
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;
        /*Expands too (null coalescing operator)
         if(_canExecute.Invoke(parameter) != null){
            CanExecute = :canExecute.Invoke(parameter);   
        }
        else{
            CanExecute = true;
        }
         */
        public void Execute(object parameter) => _execute.Invoke(parameter);

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }

}
