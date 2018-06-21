using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using VideoSearch.Utils;
using VideoSearch.VideoService;

namespace VideoSearch.Model
{
    public class MovieTaskSummaryItem : MovieTaskListItemBase
    {
        #region Constructor & Init

        public MovieTaskSummaryItem(DataItemBase parent = null)
            : base(parent)
        {
        }

        public MovieTaskSummaryItem(DataItemBase parent, String id, String displayID, String taskId, String name, String moviePos,
                        MovieTaskType taskType, MovieTaskState state) 
            : base(parent, id, displayID, taskId, name, moviePos, taskType, state)
        {
        }

        public MovieTaskSummaryItem(DataItemBase parent, String taskId, String name, MovieTaskType taskType) 
            : base(parent, taskId, name, taskType)
        {
        }

        #endregion
    }
}
