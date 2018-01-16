using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using VideoSearch.Database;
using VideoSearch.Utils;
using VideoSearch.VideoService;

namespace VideoSearch.Model
{
    public enum MovieTaskType
    {
        UnInitTask = 0,
        OutlineTask,
        CompressTask,
        SearchTask,
        UnknownTask,
        CarDetectTask,
        FaceDetectTask,
        ReadyForPlay
    }

    public class MovieTaskItem : DataItemBase
    {

        #region Constructor & Init
        
        public MovieTaskItem(DataItemBase parent = null)
            : base()
        {
            Parent = parent;

            SetLevel(4);

            Table = MovieTaskTable.Table;
        }

        public MovieTaskItem(DataItemBase parent, String id, String displayID, String taskId, String name, String moviePos,
                        MovieTaskType taskType, MovieTaskState state) 
            : this(parent)
        {
            ID = id;
            DisplayID = displayID;
            TaskId = taskId;
            Name = name;
            MoviePos = moviePos;
            TaskType = taskType;
            State = state;

            InitFromServer();
        }

        public MovieTaskItem(DataItemBase parent, String taskId, String name, MovieTaskType taskType) 
            : this(parent)
        {
            ID = String.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            DisplayID = parent.GetItemDisplayID(ID);
            TaskId = taskId;
            TaskType = taskType;
            Name = ((name != null && name.Length > 0) ? name : ItemNamePrefix) + " " + DisplayID;
            MoviePos = parent.ID;

            State = MovieTaskState.CreateReady;

            InitFromServer();
        }

        protected void InitFromServer()
        {
            XElement response = ApiManager.Instance.GetQueryTask(TaskId);

            if (response != null)
            {
                SectionCount = StringUtils.String2Int(response.Element("SectionCount").Value);
                Progress = StringUtils.String2Double(response.Element("Progress").Value) / 100.0;
                State = (MovieTaskState)StringUtils.String2Int(response.Element("Status").Value);

                if (State == MovieTaskState.CreateReady || State == MovieTaskState.Creating)
                {
                    _monitorThread = new Thread(new ThreadStart(TaskProcess));
                    _monitorThread.Start();
                }
                else if (State == MovieTaskState.Created)
                    UpdateProperty();
            }
        }
        
        protected override void DisposeItem()
        {
            base.DisposeItem();

            if (_monitorThread != null)
            {
                _monitorThread.Abort();
                _monitorThread = null;
            }
        }
        #endregion

        #region Command

        public string MovieTaskCommand
        {
            get { return "MovieTaskCommand"; }
        }

        protected override void OnCommand(Object parameter)
        {
            if (parameter != null && parameter.GetType() == typeof(String) && (String)parameter == MovieTaskCommand)
            {
                ProcessTask();
            }
            else
                base.OnCommand(parameter);
        }

        protected void ProcessTask()
        {
            if(State == MovieTaskState.CreateFail && Parent != null)
            {
                // delete
                Parent.DeleteItem(this);
            }
        }

        #endregion

        #region Override (ClearFromDB)
        public override bool ClearFromDB()
        {
            if (TaskId != "0")
            {
                if(ApiManager.Instance.DeleteTask(TaskId))
                {
                    return base.ClearFromDB();
                }
            }
            return false;
        }
        #endregion

        #region Property

        private String _taskId = "0";
        public String TaskId
        {
            get { return _taskId; }
            set
            {
                if (_taskId != value)
                {
                    _taskId = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("TaskId"));
                }
            }
        }

        private MovieTaskType _taskType = MovieTaskType.UnInitTask;
        public MovieTaskType TaskType
        {
            get
            {
                return _taskType;
            }

            set
            {
                _taskType = value;

                switch (_taskType)
                {
                    case MovieTaskType.OutlineTask:
                        IconPath = "Resources/Images/View/TreeView/tree_icon_OutlineTask.png";
                        ItemNamePrefix = "视频摘要";
                        break;
                    case MovieTaskType.CompressTask:
                        IconPath = "Resources/Images/View/TreeView/tree_icon_CompressTask.png";
                        ItemNamePrefix = "视频浓缩";
                        break;
                    case MovieTaskType.SearchTask:
                        IconPath = "Resources/Images/View/TreeView/tree_icon_SearchTask.png";
                        ItemNamePrefix = "视频检索";
                        break;
                }
            }
        }

        public String TaskTypeName
        {
            get { return ItemNamePrefix; }
        }

        private String _moviePos = "";
        public String MoviePos
        {
            get { return _moviePos; }
            set
            {
                if (_moviePos != value)
                {
                    _moviePos = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("MoviePos"));
                    OnPropertyChanged(new PropertyChangedEventArgs("MovieDisplayPos"));
                }
            }
        }

        public String MovieDisplayPos
        {
            get { return (Parent == null) ? "" : Parent.Name; }
        }

        private String _workingTime;
        public String WorkingTime
        {
            get { return _workingTime; }
            set
            {
                if (_workingTime != value)
                {
                    _workingTime = value;

                    if (_workingTime.Length > 0)
                    {
                        DateTime time = new DateTime(Decimal.ToInt64(Decimal.Parse(_workingTime)), DateTimeKind.Utc).ToLocalTime();
                        DisplayWorkingTime = String.Format("{0}", time.ToString("HH:mm:ss"));

                    }
                    else
                        DisplayWorkingTime = "";

                    OnPropertyChanged(new PropertyChangedEventArgs("WorkingTime"));
                }
            }
        }

        private String _displayWorkingTime = "";
        public String DisplayWorkingTime
        {
            get { return _displayWorkingTime; }
            set
            {
                if (_displayWorkingTime != value)
                {
                    _displayWorkingTime = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("DisplayWorkingTime"));
                }
            }
        }

        private int _sectionCount = 0;
        public int SectionCount
        {
            get { return _sectionCount; }
            set
            {
                if(_sectionCount != value)
                {
                    _sectionCount = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("SectionCount"));
                }
            }
        }

        private String _remark = "";
        public String Remark
        {
            get { return _remark; }
            set
            {
                if (_remark != value)
                {
                    _remark = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Remark"));
                }
            }
        }

        private String _opIcon = null;
        public String OpIcon
        {
            get { return _opIcon; }
            set
            {
                if (_opIcon != value)
                {
                    _opIcon = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("OpIcon"));
                }
            }
        }

        private String _opName = "";
        public String OpName
        {
            get { return _opName; }
            set
            {
                if (_opName != value)
                {
                    _opName = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("OpName"));
                }
            }
        }

        private String _operation = "";
        public String Operation
        {
            get { return _operation; }
            set
            {
                if (_operation != value)
                {
                    _operation = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Operation"));
                }
            }
        }

        private double _opacity = 1.0;
        public double Opacity
        {
            get { return _opacity; }
            set
            {
                if (_opacity != value)
                {
                    _opacity = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Opacity"));
                }
            }
        }

        private Thickness _opNameMargin = new Thickness(24, 0, 8, 0);
        public Thickness OpNameMargin
        {
            get { return _opNameMargin; }
            set
            {
                if (_opNameMargin != value)
                {
                    _opNameMargin = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("OpNameMargin"));
                }
            }
        }

        private Visibility _progressbarVisibility = Visibility.Hidden;
        public Visibility ProgressBarVisibility
        {
            get { return _progressbarVisibility; }
            set
            {
                if (_progressbarVisibility != value)
                {
                    _progressbarVisibility = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ProgressBarVisibility"));
                }
            }
        }

        private double _progress = 0.0;
        public double Progress
        {
            get { return _progress; }
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Progress"));
                }
            }
        }

        private MovieTaskState _state = MovieTaskState.UnInited;
        public MovieTaskState State
        {
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    _state = value;

                    if (_state == MovieTaskState.CreateReady)
                    {
                        OpIcon = "/VideoSearch;component/Resources/Images/Button/MovieImport.png";
                        Operation = "未开始";
                        OpNameMargin = new Thickness(0);
                        OpName = "开始导入";
                        ProgressBarVisibility = Visibility.Hidden;
                        Opacity = 1.0;
                        Remark = "/VideoSearch;component/Resources/Images/Button/MovieImporting.png";
                        IsEnabled = false;
                        Progress = 0.0;
                    }
                    else if (_state == MovieTaskState.Creating)
                    {
                        OpIcon = "/VideoSearch;component/Resources/Images/Button/MovieImportStop.png";
                        Operation = "";
                        OpNameMargin = new Thickness(16, 0, 0, 0);
                        OpName = "转码";
                        ProgressBarVisibility = Visibility.Visible;
                        Opacity = 1.0;
                        Remark = "/VideoSearch;component/Resources/Images/Button/MovieImporting.png";
                        IsEnabled = false;
                        Progress = 0.0;
                    }
                    else if (_state == MovieTaskState.Created)
                    {
                        OpIcon = "/VideoSearch;component/Resources/Images/Button/MoviePlay.png";
                        Operation = "已完成";
                        OpNameMargin = new Thickness(16, 0, 0, 0);
                        OpName = "播放";
                        ProgressBarVisibility = Visibility.Hidden;
                        Opacity = 0.6;
                        Remark = "/VideoSearch;component/Resources/Images/Button/MovieImportReady.png";
                        IsEnabled = true;
                        Progress = 0.0;
                    }
                    else if(_state == MovieTaskState.CreateFail)
                    {
                        OpIcon = "/VideoSearch;component/Resources/Images/Button/MovieTaskDelete.png";
                        Operation = "异常结束";
                        OpNameMargin = new Thickness(16, 0, 0, 0);
                        OpName = "删除";
                        ProgressBarVisibility = Visibility.Hidden;
                        Opacity = 1.0;
                        Remark = "/VideoSearch;component/Resources/Images/Button/MovieImportReady.png";
                        IsEnabled = true;
                        Progress = 0.0;
                    }

                    if (Table != null && TaskType != MovieTaskType.UnInitTask)
                        Table.Update(this);
                }
            }
        }

        public override String CheckerName
        {
            get
            {
                if (Parent == null)
                    return "";

                int index = Parent.Children.IndexOf(this);

                if (index < 0)
                    return "";

                return String.Format("{0,2:d2}", (index + 1));
            }
        }

        public override double CheckerWidth
        {
            get { return 40.0; }
        }

        #endregion

        #region TaskProcess & Web API
        private Thread _monitorThread = null;

        public void TaskProcess()
        {
            XElement response = null;
            MovieTaskState state = MovieTaskState.UnInited;

            do
            {
                response = ApiManager.Instance.GetQueryTask(TaskId);

                if (response != null)
                {
                    state = (MovieTaskState)StringUtils.String2Int(response.Element("Status").Value);

                    if(state == MovieTaskState.Creating)
                    {
                        Progress = StringUtils.String2Double(response.Element("Progress").Value) / 100.0;
                        Console.WriteLine("*** state = {0}, progress = {1}", state, Progress);
                    }
                }

                Thread.Sleep(2000);
            } while (state == MovieTaskState.Creating || state == MovieTaskState.CreateReady);

            if (response != null && state == MovieTaskState.Created)
            {
                State = MovieTaskState.Created;
                UpdateProperty();
            }
            else
                State = MovieTaskState.CreateFail;

            _monitorThread = null;
        }

        public virtual void UpdateProperty()
        {
        }

        #endregion
    }
}
