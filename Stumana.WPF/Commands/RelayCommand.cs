using System;
using System.Windows.Input;

namespace Stumana.WPF.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _executeWithParameter;
        private readonly Action _executeWithoutParameter;
        private readonly Func<object, bool> _canExecute;

        // Constructor cho Action không tham số
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _executeWithoutParameter = execute ?? throw new ArgumentNullException(nameof(execute));
            if (canExecute != null)
            {
                _canExecute = _ => canExecute();
            }
        }

        // Constructor cho Action có tham số
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _executeWithParameter = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            if (_executeWithoutParameter != null)
            {
                _executeWithoutParameter();
            }
            else
            {
                _executeWithParameter(parameter);
            }
        }
    }
}
