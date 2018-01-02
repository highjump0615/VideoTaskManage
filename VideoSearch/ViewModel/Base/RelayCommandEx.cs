using System;
using System.Windows.Input;

namespace VideoSearch.ViewModel.Base
{
    public delegate void ActionEx(object parameter);

    public class RelayCommandEx : ICommand
    {
        private readonly ActionEx _handler;
        private bool _isEnabled;

        public event EventHandler CanExecuteChanged;

        public RelayCommandEx(ActionEx handler)
        {
            _handler = handler;
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (value != _isEnabled)
                {
                    _isEnabled = value;
                    if (CanExecuteChanged != null)
                    {
                        CanExecuteChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _handler(parameter);
        }
    }
}
