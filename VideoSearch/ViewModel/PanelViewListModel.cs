using System.Collections.ObjectModel;
using System.Data;
using VideoSearch.Database;
using VideoSearch.Model;
using VideoSearch.ViewModel.Base;

namespace VideoSearch.ViewModel
{
    public class PanelViewListModel : ListViewModel
    {
        private ObservableCollection<ArticleItem> _articles = new ObservableCollection<ArticleItem>();
        public ObservableCollection<ArticleItem> Articles
        {
            get { return _articles; }
            set { _articles = value; }
        }

        public PanelViewListModel(DataItemBase owner, object parentViewModel = null) : base(owner, parentViewModel)
        {
            EventItem itemEvent = (EventItem)owner;
            
            // 加载标注信息
            var sql = "select Camera.*, Article.* " +
                "from Article " +
                "join Movie on Movie.id = Article.videoId " +
                "join Camera on Camera.id = Movie.cameraPos; " +
                $"where Camera.eventPos = {itemEvent.ID}";

            DataTable dt = DBManager.GetDataTable(sql, null);
            foreach (DataRow row in dt.Rows)
            {
                var item = (ArticleItem)ArticleTable.Table.DataItemWithRow(row, null);

                // 匹配摄像头
                foreach (CameraItem c in itemEvent.Children)
                {
                    foreach (MovieItem m in c.Children)
                    {
                        if (m.ID == item.DetailInfo.videoId)
                        {
                            item.Parent = m;
                            break;
                        }
                    }
                }

                Articles.Add(item);
            }

            updateGridIndex();
        }

        private void updateGridIndex()
        {
            foreach (ArticleItem ai in Articles)
            {
                ai.Order = $"{Articles.IndexOf(ai) + 1}";
            }
        }
    }
}
