using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using VideoSearch.VideoService;

namespace VideoSearch.Model
{
    public class MovieTaskCompressItem : MovieTaskItem
    {
        #region Constructor & Init

        public MovieTaskCompressItem(DataItemBase parent = null)
            : base(parent)
        {
        }

        public MovieTaskCompressItem(DataItemBase parent, String id, String displayID, String taskId, String name, String moviePos,
                        MovieTaskType taskType, MovieTaskState state) 
            : base(parent, id, displayID, taskId, name, moviePos, taskType, state)
        {
        }

        public MovieTaskCompressItem(DataItemBase parent, String taskId, String name, MovieTaskType taskType) 
            : base(parent, taskId, name, taskType)
        {
        }

        #endregion

        #region Property
        private String _compressedPath = "";
        public String CompressedPath
        {
            get { return _compressedPath; }
            set
            {
                if (_compressedPath != value)
                {
                    _compressedPath = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CompressedPath"));
                }
            }
        }
        public String CompressedPlayPath
        {
            get
            {
                if (State != MovieTaskState.Created || CompressedPath == null || CompressedPath.Length == 0)
                    return null;

                return basePath + CompressedPath;
            }
        }
        #endregion

        private String _tbiPath = "";

        #region Override
        public override void UpdateProperty()
        {
        }
        #endregion

        /// <summary>
        /// 获取任务结果
        /// </summary>
        public void fetchTaskResultSync()
        {
            // 已获取，直接退出
            if (!String.IsNullOrEmpty(_compressedPath))
            {
                return;
            }

            XElement response = ApiManager.Instance.GetVideoSummary(TaskId);

            if (response != null)
            {
                _compressedPath = response.Element("VideoSkimmingPath").Value;
                _tbiPath = response.Element("TbiPath").Value;
            }
        }
    }
}
