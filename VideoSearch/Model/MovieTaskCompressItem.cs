using System;
using System.ComponentModel;
using System.IO;
using VideoSearch.VideoService;

namespace VideoSearch.Model
{
    public class MovieTaskCompressItem : MovieTaskItem
    {
        #region Constructor & Init

        public MovieTaskCompressItem()
            : base()
        {
        }

        public MovieTaskCompressItem(String id, String displayID, String taskId, String name, String moviePos,
                        MovieTaskType taskType, MovieTaskState state) 
            : base(id, displayID, taskId, name, moviePos, taskType, state)
        {
        }

        public MovieTaskCompressItem(String taskId, String name, MovieTaskType taskType, DataItemBase parent) 
            : base(taskId, name, taskType, parent)
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

                String playPath = "D:\\VideoInvestigationDataDB\\CvtFile";

                return Path.Combine(playPath, CompressedPath);
            }
        }
        #endregion

        #region Override
        public override void UpdateProperty()
        {
            SummaryResponse summaryResponse = VideoAnalysis.GetVideoSummary(TaskId);

            if(summaryResponse != null)
            {
                _compressedPath = summaryResponse.VideoSkimmingPath;
            }
        }
        #endregion
    }
}
