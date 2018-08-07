using System;
using System.Windows;
using VideoSearch.Model;
using VideoSearch.ViewModel.Base;
using VideoSearch.Windows;

namespace VideoSearch.ViewModel
{
    public class EventViewModel : ListViewModel
    {
        public EventViewModel(DataItemBase owner, Object parentViewModel = null) : base(owner, parentViewModel)
        {
        }

        #region override

        public override async void AddNewItemAsync()
        {
            if (Owner == null)
                return;

            CreateEventWindow createDlg = new CreateEventWindow();
            createDlg.EventID = Owner.AutoID;
            createDlg.EventDisplayID = Owner.GetItemDisplayID(createDlg.EventID);
            createDlg.EventName = Owner.AutoName;

            Nullable<bool> result = createDlg.ShowDialog();
            if (result == true)
            {
                await Owner.AddItemAsync(new EventItem(createDlg.NewEvent));

                // update tree
                Globals.Instance.MainVM.updateTreeList();
            }
        }
        #endregion
    }
}
