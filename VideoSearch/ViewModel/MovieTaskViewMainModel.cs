using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using VideoSearch.Model;
using VideoSearch.ViewModel.Base;

namespace VideoSearch.ViewModel
{
    public class ClipInfo
    {
        public ClipInfo(long frame, int x0, int y0, int x1, int y1)
        {
            Frame = frame;
            X0 = x0;
            Y0 = y0;
            X1 = x1;
            Y1 = y1;
        }

        public long Frame
        {
            get;
            set;
        }

        public int X0
        {
            get;
            set;
        }

        public int Y0
        {
            get;
            set;
        }

        public int X1
        {
            get;
            set;
        }

        public int Y1
        {
            get;
            set;
        }
    }

    public class MovieTaskViewMainModel : ViewModelBase
    {
        public MovieItem movieItem;

        #region Constructor
        public MovieTaskViewMainModel(DataItemBase item, Object parentViewModel = null)
        {
            if (item != null && item.GetType() == typeof(MovieItem))
            {
                movieItem = (MovieItem)item;

                MovieTitle = movieItem.Name;

                // 如果没导入完，显示加载中。。
                if (movieItem.State != VideoService.ConvertStatus.ConvertedOk)
                {
                    String strNotice = "正在导入，请稍后";
                    bool bProgress = true;
                    switch (movieItem.State)
                    {
                        case VideoService.ConvertStatus.ConvertedFail:
                            strNotice = "导入失败，请重新导入";
                            bProgress = false;
                            break;

                        case VideoService.ConvertStatus.ImportReady:
                            strNotice = "未导入，请导入视频转码";
                            bProgress = false;
                            break;
                    }

                    // 显示加载中提示
                    Globals.Instance.MainVM.ShowWorkMask(strNotice, bProgress);

                    // 不让用视频处理任务
                    var viewMain = Globals.Instance.MainVM.View as MainWindow;
                    viewMain.EnableMovieTaskMenu(false);
                }
            }

            _parentViewModel = parentViewModel;
        }
        #endregion

        #region Property
        private Object _parentViewModel = null;

        private String _movieTitle = "";
        public String MovieTitle
        {
            get { return _movieTitle; }
            set
            {
                if (_movieTitle != value)
                {
                    _movieTitle = value;
                    PropertyChanging("MovieTitle");
                }
            }
        }

        private ObservableCollection<ClipInfo> _markList = new ObservableCollection<ClipInfo>();
        public ObservableCollection<ClipInfo> MarkList
        {
            get
            {
                return _markList;
            }
            set
            {
                _markList = value;
            }
        }
        #endregion

    }
}
