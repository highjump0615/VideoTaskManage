using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VideoSearch.VideoService
{
    #region Enum (ConvertStatus & MovieTaskState)
    public enum ConvertStatus
    {
        UnInited = -4,
        ImportReady = -3,
        Importing = -2,
        ImportingPaused = -1,
        Imported = 0,
        ConvertReady = Imported,
        Converting = 1,
        ConvertedOk = 2,
        ConvertedFail = 3,
        WaitInTaskQueue = 4,
        CancelWaitTask = 5,
        PlayReady = ConvertedOk
    }

    public enum MovieTaskState
    {
        UnInited = -1,
        CreateReady = 0,
        Creating = 1,
        Created = 2,
        CreateFail = 3,
        Merged = 4,
        WaitingDelete = 5,
        ErrorOccur = 6
    }
    #endregion

    #region Submit (Common)
    public class SubmitResponseBase
    {
        public int State = 0;
        public UInt64 SubmitId = 0;
        public String StrDesc = null;

        public bool IsOk
        {
            get { return (State == 0) ? true : false; }
        }

        public virtual String ErrorMessage
        {
            get
            {
                switch (State)
                {
                    case 0:
                        return null;
                    case -1:
                        return "磁盘空间不足";    // Not enough memory!
                    case -2:
                        return "Unknown Error!";
                    default:
                        return "Unknown Error!";
                };
            }
        }
    }

    #endregion

    #region QueryVideo & QueryVideoList
    public class QueryVideoResponse
    {

        public UInt64 VideoId;
        public String VideoName;
        public String FilePath;
        public UInt64 FileSize;
        public int TranscodeStatus;
        public int FrameCount;
        public int Width;
        public int Height;
        public Int64 SubmitTime;
        public int RelatedTaskCount;
        public double Progress;
        public String CvtPath;
        public String FirstFrameBmp;
    }

    public class VideoList
    {
        public int Count;
        public int TotalCount;
        public int State;
    }

    public class VideoElement
    {
        public UInt64 VideoId;
        public String VideoName;
        public String FilePath;
        public UInt64 FileSize;
        public int TranscodeStatus;
        public int FrameCount;
        public int Width;
        public int Height;
        public Int64 SubmitTime;
        public int RelatedTaskCount;
        public double Progress;
        public bool IsTranscode;
    }
    #endregion

    #region SubmitVideo
    public class SubmitVideoResponse : SubmitResponseBase
    {
        public String VideoId
        {
            get
            {
                return String.Format("{0}", SubmitId);
            }
        }

        public override String ErrorMessage
        {
            get
            {
                switch (State)
                {
                    case -3:
                        return "重复提交的任务!";
                    case 2:
                        return "userid长度为空";
                    case 3:
                        return "userid没有设置";
                    case 4:
                        return "srcName没有设置";
                    case 5:
                        return "srcName为空";
                    case 6:
                        return "srcfile不存在";
                    case 7:
                        return "srcfile文件名不合法";
                    default:
                        return base.ErrorMessage;
                };
            }
        }
    }
    #endregion

    #region SubmitTask
    public class SubmitTaskResponse : SubmitResponseBase
    {
        public String TaskId
        {
            get
            {
                return String.Format("{0}", SubmitId);
            }
        }
        public override String ErrorMessage
        {
            get
            {
                switch (State)
                {
                    case -3:
                        return "切分无结果!";
                    default:
                        return base.ErrorMessage;
                };
            }
        }
    }
    #endregion

    #region QueryTask & QueryTaskListDetail

    public class QueryTaskResponse
    {
        public UInt64 TaskId;
        public int Status;
        public int SectionCount;
        public double Progress;

        public bool IsReady
        {
            get { return Status == 0; }
        }
        public bool IsRunning
        {
            get { return Status == 1; }
        }
        public bool IsOk
        {
            get { return Status == 2; }
        }

        public String Message
        {
            get
            {
                switch (Status)
                {
                    case 0:
                        return "未开始";
                    case 1:
                        return "正在进行!";
                    case 2:
                        return "正常结束";
                    case 3:
                        return "任务异常结束";
                    case 4:
                        return "发送merge指令,merge完成";
                    case 5:
                        return "任务等待删除";
                    case 6:
                        return "出结果发生异常";
                    default:
                        return null;
                };
            }
        }

    }

    public class QueryListDetailResponse
    {
        public int Thickness;
        public int Sensitivity;
        public Rect RcgArea;
        public Rect UnrcgArea;
        public int ObjType;
        public int Color;
        public int AlarmLine;
        public int AlarmArea;
    }

    #endregion

    #region GetTaskSnapShot & SummaryResponse

    public class TaskSnapShot
    {
        public String PicPath;
        public int StartFrame;
        public int EndFrame;
        public int FrameCount;
        public int ObjType;
        public int ObjPathLeft;
        public int ObjPathRight;
        public int ObjPathTop;
        public int ObjPathBottom;
    }

    public class TaskSnapShotResponse
    {
        public int FrameCount;
        public double Progress;
        public String CvtPath;
        public int ObjCount;
        public List<TaskSnapShot> ObjList;
    }

    public class SummaryResponse
    {
        public String TbiPath;
        public String VideoSkimmingPath;
    }
    #endregion

}
