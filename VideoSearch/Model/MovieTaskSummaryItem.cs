using System;
using System.Collections.Generic;
using VideoSearch.VideoService;

namespace VideoSearch.Model
{
    public class MovieTaskSummaryItem : MovieTaskItem
    {
        #region Constructor & Init

        public MovieTaskSummaryItem()
            : base()
        {
        }

        public MovieTaskSummaryItem(String id, String displayID, String taskId, String name, String moviePos,
                        MovieTaskType taskType, MovieTaskState state) 
            : base(id, displayID, taskId, name, moviePos, taskType, state)
        {
        }

        public MovieTaskSummaryItem(String taskId, String name, MovieTaskType taskType, DataItemBase parent) 
            : base(taskId, name, taskType, parent)
        {
        }

        #endregion

        #region Override
        public override void UpdateProperty()
        {
            TaskSnapShotResponse snapshotResponse = VideoAnalysis.GetTaskSnapshot(TaskId);

            if (snapshotResponse != null)
            {
                List<TaskSnapShot> objList = snapshotResponse.ObjList;

                foreach(TaskSnapShot objs in objList)
                {
                    int i = 0;
                }
            }
        }
        #endregion
    }
}
