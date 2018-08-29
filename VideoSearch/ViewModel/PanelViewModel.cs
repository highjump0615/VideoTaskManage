using Microsoft.Win32;
using System;
using VideoSearch.Model;
using VideoSearch.VideoService;
using VideoSearch.ViewModel.Base;

namespace VideoSearch.ViewModel
{
    public class PanelViewModel : HostViewModel
    {
        public PanelViewModel(DataItemBase owner) : base(owner)
        {
            var taskItem = (MovieTaskItem)owner;
            if (taskItem.State != MovieTaskState.Created)
            {
                String strNotice = "正在处理... 请稍后";
                bool bProgress = true;
                switch (taskItem.State)
                {
                    case MovieTaskState.CreateFail:
                        strNotice = "任务处理失败，请重新提交任务";
                        bProgress = false;
                        break;
                }

                // 显示加载中提示
                Globals.Instance.MainVM.ShowWorkMask(strNotice, bProgress);
            }

            ShowResult();
        }

        #region utility function

        public void ShowResult()
        {
            if (Owner.GetType() == typeof(MovieTaskSearchItem))
                Contents = new PanelViewTaskSearchModel(Owner);
            else if (Owner.GetType() == typeof(MovieTaskSummaryItem))
                Contents = new PanelViewTaskSummaryModel(Owner);
            else if (Owner.GetType() == typeof(MovieTaskCompressItem))
                Contents = new PanelViewTaskCompressModel(Owner);
            else
                Contents = new PanelViewListModel(Owner);
        }

        #endregion
    }
}
