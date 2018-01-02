using System;
using VideoSearch.VideoService;

namespace VideoSearch.Model
{
    public class MovieTaskSearchItem : MovieTaskItem
    {
        #region Constructor & Init

        public MovieTaskSearchItem()
            : base()
        {
        }

        public MovieTaskSearchItem(String id, String displayID, String taskId, String name, String moviePos,
                        MovieTaskType taskType, MovieTaskState state) 
            : base(id, displayID, taskId, name, moviePos, taskType, state)
        {
        }

        public MovieTaskSearchItem(String taskId, String name, MovieTaskType taskType, DataItemBase parent) 
            : base(taskId, name, taskType, parent)
        {
        }

        #endregion

    }
}
