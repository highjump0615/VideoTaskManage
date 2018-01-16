using System;
using System.ComponentModel;
using System.IO;
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

                String playPath = "D:\\VideoInvestigationDataDB\\AnalysisFile";

                return playPath + CompressedPath;
            }
        }
        #endregion

        #region Override
        public override void UpdateProperty()
        {
            XElement response = ApiManager.Instance.GetVideoSummary(TaskId);

            if(response != null)
            {
                _compressedPath = response.Element("VideoSkimmingPath").Value;
            }
        }
        #endregion
    }
}
