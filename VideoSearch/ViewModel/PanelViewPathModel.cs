
using System;
using VideoSearch.ViewModel.Base;

namespace VideoSearch.ViewModel
{
    class PanelViewPathModel
    {
        private Object _parentViewModel;
        public PanelViewPathModel(Object parentViewModel)
        {
            _parentViewModel = parentViewModel;
            WireCommand();
        }

        private void WireCommand()
        {
            ClosePathCommand = new RelayCommand(ClosePath);
        }

        public RelayCommand ClosePathCommand
        {
            get;
            private set;
        }

        public void ClosePath()
        {
            if (_parentViewModel != null && _parentViewModel.GetType() == typeof(PanelViewModel))
                ((PanelViewModel)_parentViewModel).ShowResult();
        }
    }
}
