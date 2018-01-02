﻿using System;
using System.Windows.Input;

namespace VideoSearch.ViewModel.Base
{
    public class RelayCommand : ICommand
    {
        private readonly Action _handler;
        private bool _isEnabled;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action handler)
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
            _handler();
        }
    }
}
