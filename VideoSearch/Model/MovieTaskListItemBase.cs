﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using VideoSearch.Utils;
using VideoSearch.VideoService;

namespace VideoSearch.Model
{
    public class MovieTaskListItemBase : MovieTaskItem
    {
        #region Property
        private List<TaskSnapshot> _snapshots = new List<TaskSnapshot>();
        public List<TaskSnapshot> Snapshots
        {
            get { return _snapshots; }
        }

        public int DisplayType
        {
            get;
            set;
        }

        public int ItemSizeIndex
        {
            get;
            set;
        }
        #endregion

        #region Constructor & Init

        public MovieTaskListItemBase(DataItemBase parent = null)
            : base(parent)
        {
            Init();
        }

        public MovieTaskListItemBase(DataItemBase parent, String id, String displayID, String taskId, String name, String moviePos,
                        MovieTaskType taskType, MovieTaskState state)
            : base(parent, id, displayID, taskId, name, moviePos, taskType, state)
        {
            Init();
        }

        public MovieTaskListItemBase(DataItemBase parent, String taskId, String name, MovieTaskType taskType)
            : base(parent, taskId, name, taskType)
        {
            Init();
        }

        protected void Init()
        {
            DisplayType = 0;
            ItemSizeIndex = 0;
#if false
            for (int i=0; i<36; i++)
            {
                TaskSnapshot snapshot = new TaskSnapshot();

                String picPath = "D:\\VideoInvestigationDataDB\\AnalysisFile";

                snapshot.PicPath = Path.Combine(picPath, String.Format("bird\\{0,2:d2}.png", i+1));
                snapshot.PicTitle = Path.GetFileNameWithoutExtension(snapshot.PicPath);
                snapshot.StartFrame = 0;
                snapshot.EndFrame = 0;
                snapshot.FrameCount = 0;
                snapshot.ObjType = 0;

                snapshot.ObjPath = new Rect(60, 160, 320, 200);
                snapshot.DisplayType = 0;
                _snapshots.Add(snapshot);
            }
#endif
        }

        #endregion

        public override async Task FetchResult()
        {
            XElement response = await ApiManager.Instance.GetTaskSnapshot(TaskId);

            if (response != null)
            {
                _snapshots.Clear();

                foreach (XElement obj in response.Descendants("Obj"))
                {
                    TaskSnapshot snapshot = new TaskSnapshot();

                    int left = StringUtils.String2Int(obj.Element("ObjPathLeft").Value);
                    int right = StringUtils.String2Int(obj.Element("ObjPathRight").Value);
                    int top = StringUtils.String2Int(obj.Element("ObjPathTop").Value);
                    int bottom = StringUtils.String2Int(obj.Element("ObjPathBottom").Value);

                    String picPath = "D:\\VideoInvestigationDataDB\\AnalysisFile";

                    snapshot.PicPath = picPath + obj.Element("PicPath").Value;
                    snapshot.PicTitle = Path.GetFileNameWithoutExtension(snapshot.PicPath);
                    snapshot.StartFrame = StringUtils.String2Int(obj.Element("StartFrame").Value);
                    snapshot.EndFrame = StringUtils.String2Int(obj.Element("EndFrame").Value);
                    snapshot.FrameCount = StringUtils.String2Int(obj.Element("FrameCount").Value);
                    snapshot.ObjType = StringUtils.String2Int(obj.Element("ObjType").Value);

                    snapshot.ObjPath = new Rect(left, top, right - left, bottom - top);

                    snapshot.DisplayType = 0;

                    _snapshots.Add(snapshot);
                }
            }
        }

        public override bool IsFetched()
        {
            return Snapshots.Count > 0;
        }
    }
}
