using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using VideoSearch.Database;
using VideoSearch.VideoService;
using VideoSearch.Windows;
using System.Xml.Linq;
using VideoSearch.Utils;
using System.Threading.Tasks;
using System.Configuration;

namespace VideoSearch.Model
{
    public class MovieItem : DataItemBase
    {
        public static int LEVEL = 3;

        #region Constructor & Init & Destructor
        public MovieItem(DataItemBase parent = null) : base()
        {
            Parent = parent;

            SetLevel(3);
            ItemNamePrefix = "MovieSeq";
            IconPath = "Resources/Images/View/TreeView/tree_icon_Movie.png";

            State = ConvertStatus.ImportReady;
            Table = MovieTable.Table;
            ItemsTable = MovieTaskTable.Table;
        }

        public MovieItem(DataItemBase parent, String id, String displayID, int videoId, String name, String cameraPos, String srcPath,
                        ConvertStatus state, String movieTask)
            : this(parent)
        {
            ID = id;
            DisplayID = displayID;
            VideoId = videoId;
            Name = name;
            CameraPos = cameraPos;
            SrcPath = srcPath;
            OrgPath = "";
            MovieLength = 0;
            State = state;
            MovieTask = movieTask;

            //if(VideoId != "0")
            //    InitFromServer();
        }

        public MovieItem(DataItemBase parent, String srcPath) : this(parent)
        {
            SrcPath = srcPath;
            ID = String.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            DisplayID = parent.GetItemDisplayID(ID);
            Name = Path.GetFileNameWithoutExtension(srcPath);
            CameraPos = parent.ID;
            MovieLength = 0;
            State = ConvertStatus.ImportReady;
        }

        private void FillDataFromXml(XElement videoInfo, bool progessOnly = false)
        {
            try
            {
                if (!progessOnly)
                {
                    SubmitTime = GMTTimeToString(StringUtils.String2Int64(videoInfo.Element("SubmitTime").Value));
                    OrgPath = videoInfo.Element("FilePath").Value;
                    MovieLength = StringUtils.String2UInt64(videoInfo.Element("FileSize").Value);
                    //ThumbnailPath = videoInfo.Element("FirstFrameBmp").Value;
                    //CvtPath = videoInfo.Element("CvtPath").Value;                    

                    //State = (ConvertStatus)StringUtils.String2Int(videoInfo.Element("TranscodeStatus").Value);
                }

                // 转码百分比是10~100%
                //Progress = 0.1 + StringUtils.String2Double(videoInfo.Element("Progress").Value) / 100.0 * 0.9;
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 获取视频信息
        /// </summary>
        /// <returns></returns>
        public async Task InitFromServer(bool FetchOnly = false)
        {
            // 还没提交的视频，退出
            if (VideoId <= 0)
            {
                return;
            }

            // 已获取信息，退出
            if (IsFetched())
            {
                if (!FetchOnly)
                {
                    await StartMonitoring();
                }

                return;
            }

            XElement videoInfo = await ApiManager.Instance.GetQueryVideo(VideoId);
            if(videoInfo != null)
            {
                FillDataFromXml(videoInfo);

                if (!FetchOnly)
                {
                    await StartMonitoring();
                }
            }
        }

        private async Task StartMonitoring()
        {
            if (State != ConvertStatus.PlayReady || Progress < 1.0)
            {
                if (State == ConvertStatus.ConvertReady)
                    await SubmitVideo();
                else
                {
                    //_monitorThread = new Thread(new ThreadStart(ConvertMonitorThread));
                    //_monitorThread.Start();
                }
            }
        }

        public bool IsFetched()
        {
            return MovieLength > 0;
        }

        public override void DisposeItem()
        {
            base.DisposeItem();

            if(_monitorThread != null)
            {
                _monitorThread.Abort();
                _monitorThread = null;
            }
            if (_importThread != null)
            {
                _importThread.Abort();
                _importThread = null;
            }
        }
        #endregion

        #region Command

        public string MovieImportCommand
        {
            get { return "MovieImportCommand"; }
        }

        protected override void OnCommand(Object parameter)
        {
            if (parameter != null && parameter.GetType() == typeof(String) && (String)parameter == MovieImportCommand)
            {
                Import();
            }
            else
                base.OnCommand(parameter);
        }
        #endregion

        #region Property

        private int _videoId = 0;
        public int VideoId
        {
            get { return _videoId; }
            set
            {
                if(_videoId != value)
                {
                    _videoId = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("VideoId"));
                }
            }
        }

        private String _srcPath = "";
        public String SrcPath
        {
            get { return _srcPath; }
            set
            {
                if(_srcPath != value)
                {
                    _srcPath = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("SrcPath"));
                }
            }
        }

        private String _orgPath = "";
        public String OrgPath
        {
            get { return _orgPath; }
            set
            {
                if (_orgPath != value)
                {
                    _orgPath = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("OrgPath"));
                }
            }
        }

        //private String _cvtPath = "";
        //public String CvtPath
        //{
        //    get { return _cvtPath; }
        //    set
        //    {
        //        if (_cvtPath != value)
        //        {
        //            _cvtPath = value;
        //            OnPropertyChanged(new PropertyChangedEventArgs("CvtPath"));
        //        }
        //    }
        //}

        public String ThumbnailPath
        {
            get
            {
                var filePathWithExt = OrgPath + ".bmp";
                String fullPath = ConfigurationManager.AppSettings["dataPathBase"] + "\\OrgFile";

                return Path.Combine(fullPath, filePathWithExt);
            }
        }

        public String PlayPath
        {
            get
            {
                if (State != ConvertStatus.ConvertedOk || OrgPath == null || OrgPath.Length == 0)
                    return null;

                String playPath = ConfigurationManager.AppSettings["dataPathBase"] + "\\OrgFile";

                return Path.Combine(playPath, OrgPath);
            }
        }

        public bool IsMultiPlayable
        {
            get { return (IsChecked && State == ConvertStatus.ConvertedOk); }
        }

        private String _cameraPos = "";
        public String CameraPos
        {
            get { return _cameraPos; }
            set
            {
                if(_cameraPos != value)
                {
                    _cameraPos = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CameraPos"));
                    OnPropertyChanged(new PropertyChangedEventArgs("CameraDisplayPos"));
                }
            }
        }

        public String CameraDisplayPos
        {
            get { return (Parent == null) ? "" : Parent.DisplayID; }
        }

        private String _submitTime;
        public String SubmitTime
        {
            get { return _submitTime; }
            set
            {
                if (_submitTime != value)
                {
                    _submitTime = value;

                    if (_submitTime.Length > 0)
                    {
                        DateTime time = new DateTime(Decimal.ToInt64(Decimal.Parse(_submitTime)), DateTimeKind.Utc).ToLocalTime();
                        DisplaySubmitTime = String.Format("{0}", time.ToString("HH:mm:ss"));

                    }
                    else
                        DisplaySubmitTime = "";

                    OnPropertyChanged(new PropertyChangedEventArgs("SubmitTime"));
                }
            }
        }

        private String _displaySubmitTime = "";
        public String DisplaySubmitTime
        {
            get { return _displaySubmitTime; }
            set
            {
                if(_displaySubmitTime != value)
                {
                    _displaySubmitTime = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("DisplaySubmitTime"));
                }
            }
        }

        private String _displayCompleteTime = "";
        public String DisplayCompleteTime
        {
            get { return _displayCompleteTime; }
            set
            {
                if (_displayCompleteTime != value)
                {
                    _displayCompleteTime = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("DisplayCompleteTime"));
                }
            }
        }

        private String _movieSize = "";
        public String MovieSize
        {
            get { return _movieSize; }
            set
            {
                if (_movieSize != value)
                {
                    _movieSize = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("MovieSize"));
                }
            }
        }

        private UInt64 _movieLength = 0;
        public UInt64 MovieLength
        {
            get { return _movieLength; }
            set
            {
                if (_movieLength != value)
                {
                    _movieLength = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("MovieLength"));

                    if (_movieLength > 0)
                        MovieSize = GetSizeString(_movieLength);
                    else
                        MovieSize = "";
                }
            }
        }

        private String _movieTask = "";
        public String MovieTask
        {
            get { return _movieTask; }
            set
            {
                if (_movieTask != value)
                {
                    _movieTask = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("MovieTask"));
                }
            }
        }

        private String _remark = "/VideoSearch;component/Resources/Images/Button/MovieImporting.png";
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

        private double _progress = 0.0;
        public double Progress
        {
            get { return _progress; }
            set {
                if (_progress != value)
                {
                    _progress = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Progress"));
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

        public double OpacityCheck
        {
            get
            {
                return Opacity == 1.0 ? 1.0 : 0.5;
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

        private Visibility _buttonVisibility = Visibility.Visible;
        public Visibility ButtonVisibility
        {
            get { return _buttonVisibility; }
            set
            {
                if (_buttonVisibility != value)
                {
                    _buttonVisibility = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("ButtonVisibility"));
                }
            }
        }

        private int _operationPos = 0;
        public int OperationPos
        {
            get { return _operationPos; }
            set
            {
                if (_operationPos != value)
                {
                    _operationPos = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("OperationPos"));
                    OnPropertyChanged(new PropertyChangedEventArgs("IsIndeterminate"));
                    OnPropertyChanged(new PropertyChangedEventArgs("OperationAlignment"));
                }
            }
        }

        public TextAlignment OperationAlignment
        {
            get
            {
                return (_operationPos == 1) ? TextAlignment.Center : TextAlignment.Left;
            }
        }

        public bool IsIndeterminate
        {
            get { return (_operationPos == 1) ? true : false; }
        }

        private ConvertStatus _state = ConvertStatus.UnInited;
        public ConvertStatus State
        {
            get { return _state; }
            set
            {
                if(_state != value)
                {
                    _state = value;

                    if (_state == ConvertStatus.ImportReady)
                    {
                        OpIcon = "/VideoSearch;component/Resources/Images/Button/MovieImport.png";
                        Operation = "已加载";
                        OpNameMargin = new Thickness(0);
                        OpName = "开始导入";
                        ButtonVisibility = Visibility.Visible;
                        ProgressBarVisibility = Visibility.Hidden;
                        OperationPos = 0;
                        Opacity = 1.0;
                        IsEnabled = true;
                        Progress = 0.0;
                    }
                    else if (_state == ConvertStatus.Importing)
                    {
                        OpIcon = "/VideoSearch;component/Resources/Images/Button/MovieImportPause.png";
                        Operation = "";
                        OpNameMargin = new Thickness(16, 0, 0, 0);
                        OpName = "停止";
                        ButtonVisibility = Visibility.Visible;
                        ProgressBarVisibility = Visibility.Visible;
                        OperationPos = 0;
                        Opacity = 1.0;
                        IsEnabled = true;
                        Progress = 0.0;
                    }
                    else if (_state == ConvertStatus.ConvertReady)
                    {
                        OpIcon = "/VideoSearch;component/Resources/Images/Button/MovieImportStop.png";
                        Operation = "";
                        OpNameMargin = new Thickness(0);
                        OpName = "";
                        ButtonVisibility = Visibility.Hidden;
                        ProgressBarVisibility = Visibility.Visible;
                        OperationPos = 1;
                        Opacity = 0.7;
                        IsEnabled = true;
                    }
                    else if (_state == ConvertStatus.Converting)
                    {
                        OpIcon = "/VideoSearch;component/Resources/Images/Button/MovieImportStop.png";
                        Operation = "";
                        OpNameMargin = new Thickness(16, 0, 0, 0);
                        OpName = "停止";
                        ButtonVisibility = Visibility.Visible;
                        ProgressBarVisibility = Visibility.Visible;
                        OperationPos = 0;
                        Opacity = 0.7;
                        Remark = "/VideoSearch;component/Resources/Images/Button/MovieImporting.png";
                        IsEnabled = false;
                    }
                    else if (_state == ConvertStatus.ImportingPaused)
                    {
                        OpIcon = "/VideoSearch;component/Resources/Images/Button/MovieImportResume.png";
                        Operation = "";
                        OpNameMargin = new Thickness(16, 0, 0, 0);
                        OpName = "开始";
                        ButtonVisibility = Visibility.Visible;
                        ProgressBarVisibility = Visibility.Visible;
                        OperationPos = 0;
                        Opacity = 1.0;
                        IsEnabled = true;
                    }
                    else
                    {
                        OpIcon = "/VideoSearch;component/Resources/Images/Button/MoviePlay.png";
                        Operation = "已导入";
                        OpNameMargin = new Thickness(16, 0, 0, 0);
                        OpName = "播放";
                        ButtonVisibility = Visibility.Visible;
                        ProgressBarVisibility = Visibility.Hidden;
                        OperationPos = 0;
                        Opacity = 1.0;
                        IsEnabled = true;
                    }

                    updateTable();
                }
            }
        }
        #endregion

        #region Override (ClearFromDB)
        public override async Task<bool> ClearFromDBAsync()
        {
            if (VideoId > 0)
            {
                if (await ApiManager.Instance.DeleteVideo(VideoId))
                {
                    return base.ClearFromDB();
                }
            }
            return false;
        }
        #endregion

        #region Movie Import & Web API

        private Thread _importThread = null;
        private Thread _monitorThread = null;

        public void Import()
        {
            Console.WriteLine("=== Import start ===");
            if(State == ConvertStatus.ImportReady)
            {
                State = ConvertStatus.Importing;
                Progress = 0.0;

                _importThread = new Thread(new ThreadStart(ImportThread));
                _importThread.Start();
                Console.WriteLine("=== Import Importing ===");
            }
            else if(State == ConvertStatus.Importing)
            {
                State = ConvertStatus.ImportingPaused;
                Console.WriteLine("=== Import ImportingPaused ===");
            }
            else if(State == ConvertStatus.ImportingPaused)
            {
                State = ConvertStatus.Importing;
                Console.WriteLine("=== Importing ===");
            }
            else if (State == ConvertStatus.ConvertReady)
            {
                State = ConvertStatus.Converting;
                var taskSubmit = SubmitVideo();
            }
            else if (State == ConvertStatus.Converting)
            {

                Console.WriteLine("=== Import Converting ===");
            }
            else if(State == ConvertStatus.PlayReady)
            {
                Console.WriteLine("=== Import PlayReady ===");

                PlayerWindow.PlayMovie(Name, PlayPath);
            }
            Console.WriteLine("=== Import End ===");
        }

        protected bool prepareImport()
        {
            String orgPath = "D:\\VideoInvestigationDataDB";
            //D:\VideoInvestigationDataDB\OrgFile

            if (!Directory.Exists(orgPath))
                Directory.CreateDirectory(orgPath);

            orgPath = Path.Combine(orgPath, "OrgFile");
            if (!Directory.Exists(orgPath))
                Directory.CreateDirectory(orgPath);

            orgPath = Path.Combine(orgPath, Parent.DisplayID);
            if (!Directory.Exists(orgPath))
                Directory.CreateDirectory(orgPath);

            orgPath = Path.Combine(orgPath, ID);
            if (!Directory.Exists(orgPath))
                Directory.CreateDirectory(orgPath);

            OrgPath = Path.Combine(orgPath, ID);

            return true;
        }

        private void ImportThread()
        {
            if (OrgPath.Length == 0)
                prepareImport();

            UInt64 write_len = 0, file_len = GetFileSize(SrcPath);
            int read_unit = 1048576, read_len;
            byte[] bytes = new byte[read_unit];

            using (FileStream srcStream = new FileStream(SrcPath, FileMode.Open, FileAccess.Read))
            {
                srcStream.Seek(0, SeekOrigin.Begin);

                using (FileStream dstStream = new FileStream(OrgPath, FileMode.Create, FileAccess.Write))
                {
                    do
                    {
                        if (State == ConvertStatus.ImportingPaused)
                            continue;

                        read_len = srcStream.Read(bytes, 0, read_unit);

                        dstStream.Write(bytes, 0, read_len);
                        write_len += (UInt64)read_len;

                        // 复制文件是至10%
                        Progress = (double)write_len / (double)file_len;
                    } while (write_len < file_len);

                    dstStream.Flush();
                    dstStream.Close();
                }

                srcStream.Close();
            }

            if (write_len == file_len)
            {
                State = ConvertStatus.Imported;
                _importThread = null;

                var submitTask = SubmitVideo();
            }
        }

        private void ConvertMonitorThread()
        {
            ConvertStatus status = ConvertStatus.UnInited;
            XElement videoInfo = null;
            do
            {
                Thread.Sleep(1000);

                var taskQuery = ApiManager.Instance.GetQueryVideo(VideoId);
                taskQuery.Wait();
                videoInfo = taskQuery.Result;

                FillDataFromXml(videoInfo, true);

                status = (ConvertStatus)int.Parse(videoInfo.Element("TranscodeStatus").Value);
                Console.WriteLine($"Convert status:{status}, Progress : {Progress}");

                if (status == ConvertStatus.ConvertedFail)
                {
                    // 失败，退出
                    break;
                }

            } while (status != ConvertStatus.ConvertedOk ||
                        Progress < 1.0);

            if (videoInfo != null && status == ConvertStatus.ConvertedOk)
            {
                FillDataFromXml(videoInfo);
                State = ConvertStatus.PlayReady;
            }
            else
                State = ConvertStatus.ConvertReady;

            _monitorThread = null;
        }

        protected async Task SubmitVideo()
        {
            String videoPath = "file://" + Parent.DisplayID + "/" + ID + "/" + ID;
            XElement response = await ApiManager.Instance.SubmitVideo(videoPath);

            if (response != null && StringUtils.String2Int(response.Element("State").Value) == 0)
            {
                VideoId = int.Parse(response.Element("VideoId").Value);

                XElement videoInfo = await ApiManager.Instance.GetQueryVideo(VideoId);
                if (videoInfo != null)
                {
                    FillDataFromXml(videoInfo);

                    // set thumbnail

                    State = ConvertStatus.ConvertedOk;

                    //_monitorThread = new Thread(new ThreadStart(ConvertMonitorThread));
                    //_monitorThread.Start();
                }
            }
            else
            {
                State = ConvertStatus.ConvertReady;

                MessageBox.Show("提交视频失败!");
            }
        }

        protected String GMTTimeToString(Int64 time)
        {
            DateTime gmtTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            DateTime cvtTime = new DateTime(time * 10000 + gmtTime.Ticks, DateTimeKind.Utc);

            return String.Format("{0}", cvtTime.Ticks);
        }

        protected UInt64 GetFileSize(String filePath)
        {
            FileInfo info = new FileInfo(filePath);

            if (info == null)
                return 0;

            return (UInt64)info.Length;
        }

        protected String GetSizeString(UInt64 length)
        {
            if (length >= 1024)
            {
                length /= 1024;
                if (length >= 1024)
                {
                    length /= 1024;
                    if (length >= 1024)
                    {
                        length /= 1024;
                        return length.ToString() + "G";
                    }
                    return length.ToString() + "M";
                }
                return length.ToString() + "K";
            }
            return length.ToString() + "B";
        }
        #endregion
    }
}
