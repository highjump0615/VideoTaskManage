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

        /// <summary>
        /// 筛选字段：目标类型
        /// </summary>
        private int _filterTargetType = -1;
        public int FilterTargetType
        {
            get { return _filterTargetType; }
            set
            {
                _filterTargetType = value;
                PropertyChanging("FilterTargetType");
            }
        }


        /// <summary>
        /// 筛选字段：关键词
        /// </summary>
        private string _filterKeyword;
        public string FilterKeyword
        {
            get { return _filterKeyword; }
            set
            {
                _filterKeyword = value;
                PropertyChanging("FilterKeyword");
            }
        }


        // 按钮
        public RelayCommand FilterCommand
        {
            get;
            private set;
        }
        public RelayCommand ResetCommand
        {
            get;
            private set;
        }

        public PanelViewListModel(DataItemBase owner, object parentViewModel = null) : base(owner, parentViewModel)
        {
            // 初始化按钮事件
            FilterCommand = new RelayCommand(FilterArticle);
            ResetCommand = new RelayCommand(ResetArticle);

            LoadArticles();
        }

        private void LoadArticles()
        {
            EventItem itemEvent = (EventItem)Owner;

            // 加载标注信息
            var sql = "select Camera.*, Article.* " +
                "from Article " +
                "join Movie on Movie.id = Article.videoId " +
                "join Camera on Camera.id = Movie.cameraPos " +
                $"where Camera.eventPos = '{itemEvent.ID}' ";

            // 筛选目标类型
            if (FilterTargetType >= 0)
            {
                sql += $"and Article.TargetType = {FilterTargetType} ";
            }
            if (!string.IsNullOrEmpty(FilterKeyword))
            {
                sql += $"and Article.Description like '%{FilterKeyword}%' ";
            }

            Articles.Clear();

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

        /// <summary>
        /// 筛选列表
        /// </summary>
        public void FilterArticle()
        {
            LoadArticles();
        }

        /// <summary>
        /// 重置列表
        /// </summary>
        public void ResetArticle()
        {
            // 清空筛选条件
            FilterTargetType = -1;
            FilterKeyword = "";

            LoadArticles();
        }
    }
}
