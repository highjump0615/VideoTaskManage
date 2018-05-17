
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

        public PanelViewPathModel(DataItemBase owner, Object parentViewModel)
        {
            parentItem = (EventItem)owner;
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

        public List<CameraItem> LoadArticles()
        {
            // 加载标注信息
            var sql = "select distinct Camera.ID as cameraId " +
                "from Article " +
                "join Movie on Movie.id = Article.videoId " +
                "join Camera on Camera.id = Movie.cameraPos " +
                $"where Camera.eventPos = '{parentItem.ID}' ";

            var listItem = new List<CameraItem>();

            DataTable dt = DBManager.GetDataTable(sql, null);
            foreach (DataRow row in dt.Rows)
            {
                var cameraId = String.Format("{0}", row["cameraId"]);

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
