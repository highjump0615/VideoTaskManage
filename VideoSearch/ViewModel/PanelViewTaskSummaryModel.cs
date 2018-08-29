using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using VideoSearch.Model;
using VideoSearch.ViewModel.Base;

namespace VideoSearch.ViewModel
{
    public class PanelViewTaskSummaryModel : PanelViewSummaryBaseModel
    {

        public PanelViewTaskSummaryModel(DataItemBase item)
        {
            if (item != null && item.GetType() == typeof(MovieTaskSummaryItem))
            {
                MovieTaskSummaryItem summaryItem = (MovieTaskSummaryItem)item;

                _owner = summaryItem;

                DisplayType = _owner.DisplayType;
                ItemSizeIndex = _owner.ItemSizeIndex;

                var task = InitTaskResult();
             }
        }        
        
    }

}
