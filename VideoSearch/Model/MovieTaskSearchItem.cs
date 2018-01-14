using System;
using VideoSearch.VideoService;

namespace VideoSearch.Model
{
    public class MovieTaskSearchItem : MovieTaskItem
    {
        #region Constructor & Init

        public MovieTaskSearchItem(DataItemBase parent = null)
            : base(parent)
        {
        }

        public MovieTaskSearchItem(DataItemBase parent, String id, String displayID, String taskId, String name, String moviePos,
                        MovieTaskType taskType, MovieTaskState state) 
            : base(parent, id, displayID, taskId, name, moviePos, taskType, state)
        {
        }

        public MovieTaskSearchItem(DataItemBase parent, String taskId, String name, MovieTaskType taskType) 
            : base(parent, taskId, name, taskType)
        {
        }

        #endregion

    }
}
