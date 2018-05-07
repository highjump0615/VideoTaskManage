using System;
using System.ComponentModel;
using System.Threading.Tasks;
using VideoSearch.Model;
using VideoSearch.ViewModel.Base;
using VideoSearch.Views;

namespace VideoSearch.ViewModel
{
    public class PanelViewTaskCompressModel : ViewModelBase
    {
        private MovieTaskCompressItem _owner = null;
        public MovieTaskCompressItem taskItem
        {
            get { return _owner;  }
        }

        public PanelViewTaskCompressModel(DataItemBase item)
        {
            if (item == null || item.GetType() != typeof(MovieTaskCompressItem))
            {
                return;
            }

            _owner = (MovieTaskCompressItem)item;
        }

        public async Task InitTaskResult()
        {
            await _owner.InitFromServer();

            // 获取任务结果
            MovieTitle = _owner.Name;
            MoviePath = _owner.CompressedPlayPath;

            // 播放器设置
            var view = (PanelViewTaskCompressView)this.View;
            view.InitPlayer();
        }

        #region Property

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

        private String _moviePath = "";

        public String MoviePath
        {
            get { return _moviePath; }
            set
            {
                if (_moviePath != value)
                {
                    _moviePath = value;
                    PropertyChanging("MoviePath");
                }
            }
        }

        #endregion
    }
}
