using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using VideoSearch.Database;
using VideoSearch.Model;
using VideoSearch.ViewModel.Base;
using VideoSearch.Windows;

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
            var sql = "select Camera.*, Article.*, Article.ID as ArticleID " +
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

                item.PropertyChangedEvent += OnArticlePropertyChanged;

                Articles.Add(item);
            }

            updateGridIndex();
        }

        private void OnArticlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                // 选择项有变化，更新删除按钮
                var countSelected = Articles.Where(x => x.IsChecked == true).Count();

                var viewMain = Globals.Instance.MainVM.View as MainWindow;
                viewMain.ToolbarMarkDelete.IsEnabled = countSelected > 0;
            }
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

        /// <summary>
        /// 删除
        /// </summary>
        public override void DeleteSelectedItems()
        {
            var articlesRemove = Articles.Where(x => x.IsChecked == true).ToList();

            // 没有已选择的，推出
            if (articlesRemove.Count <= 0)
            {
                return;
            }

            ConfirmDeleteWindow deleteDlg = new ConfirmDeleteWindow();

            Nullable<bool> result = deleteDlg.ShowDialog();

            if (result == true)
            {
                Globals.Instance.ShowWaitCursor(true);
                
                foreach (var article in articlesRemove)
                {
                    ArticleTable.Table.Remove(article);
                    Articles.Remove(article);
                }

                Globals.Instance.ShowWaitCursor(false);
                updateGridIndex();
            }
        }

        /// <summary>
        /// 导出
        /// </summary>
        public void Export()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "Export";
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV documents|*.csv";

            if (dlg.ShowDialog() == true)
            {
                string filename = dlg.FileName;

                // 保存
                Globals.Instance.ShowWaitCursor(true);

                var csv = new StringBuilder();

                // 字段名
                var newLine = "ID, 视频ID, 时间点, X, Y, 长度, 宽度, 轨迹说明, 关键词, 目标类型";
                csv.AppendLine(newLine);

                foreach (ArticleItem item in Articles)
                {
                    newLine = $"{item.ID}, {item.DetailInfo.videoId}, {item.DetailInfo.frame}, {item.DetailInfo.x}, {item.DetailInfo.y}, {item.DetailInfo.width}, {item.DetailInfo.height}";
                    newLine += $"{item.DetailInfo.desc}, {item.DetailInfo.keyword}, {item.TargetType}";

                    csv.AppendLine(newLine);
                }

                File.WriteAllText(filename, csv.ToString());

                Globals.Instance.ShowWaitCursor(false);
            }
        }
    }
}
