using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using VideoSearch.ViewModel;

namespace VideoSearch
{
    class Globals
    {
        private static Globals _Instance;
        public static Globals Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new Globals();

                return _Instance;
            }
        }

        public MainViewModel MainVM { get; set; }

        public void ShowWaitCursor(bool wait)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = wait ? Cursors.Wait : null;
            });
        }
    }
}
