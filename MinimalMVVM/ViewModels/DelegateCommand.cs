using System;
using System.Windows.Input;

namespace MinimalMVVM.ViewModels
{
    public class DelegateCommand : ICommand
    {

        private Action<object> _action;
        private Action _actionNoParameter;


        private bool _canExecute;
        public DelegateCommand(Action<object> action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public DelegateCommand(Action action, bool canExecute)
        {
            _actionNoParameter = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged { add { } remove { } }

        public void Execute(object parameter)
        {
            if (_action == null)
                _actionNoParameter();
            else
                _action(parameter);
        }

    }
}
