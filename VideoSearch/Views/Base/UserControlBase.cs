using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VideoSearch.ViewModel.Base;

namespace VideoSearch.Views
{
    public class UserControlBase : UserControl
    {
        protected void onDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // VM里设置view
            ViewModelBase vm = (ViewModelBase)e.NewValue;
            if (vm != null)
            {
                vm.View = this;
            }
        }
    }
}
