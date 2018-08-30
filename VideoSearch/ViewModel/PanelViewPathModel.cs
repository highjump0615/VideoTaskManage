
using System;
using System.Collections.Generic;
using System.Data;
using VideoSearch.Database;
using VideoSearch.Model;
using VideoSearch.ViewModel.Base;

namespace VideoSearch.ViewModel
{
    class PanelViewPathModel : ViewModelBase
    {
        private Object _parentViewModel;
        private EventItem parentItem;

        private List<ArticleItem> articlesSelected;

        public PanelViewPathModel(DataItemBase owner, Object parentViewModel, List<ArticleItem> articles)
        {
            parentItem = (EventItem)owner;
            _parentViewModel = parentViewModel;

            articlesSelected = articles;

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

        public List<CameraItem> LoadArticles()
        {
            // 加载标注信息
            var listItem = new List<CameraItem>();

            foreach (ArticleItem article in articlesSelected)
            {
                var cameraId = article.Parent.Parent.ID;

                // 匹配摄像头
                foreach (CameraItem c in parentItem.Children)
                {
                    if (c.ID == cameraId)
                    {
                        listItem.Add(c);
                        break;
                    }
                }
            }

            return listItem;
        }
    }
}
