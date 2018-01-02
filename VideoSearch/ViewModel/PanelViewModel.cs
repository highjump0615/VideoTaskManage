using Microsoft.Win32;
using System;
using VideoSearch.Model;
using VideoSearch.ViewModel.Base;

namespace VideoSearch.ViewModel
{
    public class PanelViewModel : HostViewModel
    {
        public PanelViewModel(DataItemBase owner) : base(owner)
        {
            ShowResult();
        }

        #region utility function
        protected void updateList()
        {
            if (Contents == null || Contents.GetType() == typeof(PanelViewPathModel))
            {
                ShowResult();
            }
        }

        public void ShowResult()
        {
            if (Owner.GetType() == typeof(MovieTaskSearchItem))
                Contents = new PanelViewTaskSearchModel();
            else if (Owner.GetType() == typeof(MovieTaskSummaryItem))
                Contents = new PanelViewTaskSummaryModel();
            else if (Owner.GetType() == typeof(MovieTaskCompressItem))
                Contents = new PanelViewTaskCompressModel(Owner);
            else
                Contents = new PanelViewListModel(Owner);
        }

        public void ShowPath()
        {
            Contents = new PanelViewPathModel(this);
        }
        
        public void Export()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "Export";
            dlg.DefaultExt = ".avi";
            dlg.Filter = "Text documents (.avi)|*.AVI";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
            }
        }
        #endregion
    }
}
