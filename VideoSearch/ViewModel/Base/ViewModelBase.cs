using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace VideoSearch.ViewModel.Base
{
    public abstract class ViewModelBase : ObservableObject
    {
        public ViewModelBase ViewModelParent { get; set; }

        public Control View { get; set; }

        public void Dispose()
        {
            OnDispose();
        }

        protected virtual void OnDispose()
        {
            Console.WriteLine(string.Format("Disposing {0} , {1}", this.GetType().Name, this.GetType().FullName));
        }
    }
}
