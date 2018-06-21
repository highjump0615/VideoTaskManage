using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using VideoSearch.Model;
using VideoSearch.ViewModel.Base;

namespace VideoSearch.ViewModel
{
    public class PanelViewTaskSearchModel : PanelViewSummaryBaseModel
    {

        public PanelViewTaskSearchModel(DataItemBase item)
        {
            if (item != null && item.GetType() == typeof(MovieTaskSearchItem))
            {
                MovieTaskSearchItem searchItem = (MovieTaskSearchItem)item;
                _owner = searchItem;

                DisplayType = _owner.DisplayType;
                ItemSizeIndex = _owner.ItemSizeIndex;

                var task = InitTaskResult();
            }
        }

        #region Property
        #endregion
    }
}
