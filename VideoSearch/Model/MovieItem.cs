using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Net;
using VideoSearch.Database;
using VideoSearch.VideoService;
using VideoSearch.Windows;

namespace VideoSearch.Model
{
    public class MovieItem : DataItemBase
    {
        #region Constructor & Init & Destructor
        public MovieItem() : base()
        {
            SetLevel(3);
            ItemNamePrefix = "MovieSeq";
            IconPath = "Resources/Images/View/TreeView/tree_icon_Movie.png";

            State = ConvertStatus.ImportReady;

            Table = MovieTable.Table;
            ItemsTable = MovieTaskTable.Table;
        }

        public MovieItem(String id, String displayID, String videoId, String name, String cameraPos, String srcPath,
                        ConvertStatus state, String movieTask)
            : this()
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

            if(VideoId != "0")
                InitFromServer();
        }

        public MovieItem(String srcPath, DataItemBase parent) : this()
        {
            SrcPath = srcPath;
            ID = String.Format("{0}", DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            DisplayID = parent.GetItemDisplayID(ID);
            VideoId = "0";
            Name = Path.GetFileNameWithoutExtension(srcPath);
            CameraPos = parent.ID;
            MovieLength = 0;
            State = ConvertStatus.ImportReady;
        }

        protected void InitFromServer()
        {
            QueryVideoResponse response = VideoAnalysis.QueryVideo(VideoId);

            if(response != null)
            {
                SubmitTime = GMTTimeToString(response.SubmitTime);
                OrgPath = response.FilePath;
                CvtPath = response.CvtPath;
                ThumbnailPath = response.FirstFrameBmp;
                MovieLength = response.FileSize;
                State = (ConvertStatus)response.TranscodeStatus;
                Progress = response.Progress / 100.0;

                if(State != ConvertStatus.PlayReady || Progress < 1.0)
                {
                    if (State == ConvertStatus.ConvertReady)
                        SubmitVideo();
                    else
                    {
                        _monitorThread = new Thread(new ThreadStart(ConvertMonitorThread));
                        _monitorThread.Start();
                    }
                }
            }
        }


        protected override void DisposeItem()
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

        private String _videoId = "0";
        public String VideoId
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

        private String _cvtPath = "";
        public String CvtPath
        {
            get { return _cvtPath; }
            set
            {
                if (_cvtPath != value)
                {
                    _cvtPath = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CvtPath"));
                }
            }
        }

        private String _thumbnailPath = "";
        public String ThumbnailPath
        {
            get { return _thumbnailPath; }
            set
            {
                String pathPrefix = "D:\\VideoInvestigationDataDB\\CvtFile";
                String orgPath = _thumbnailPath;
                if (orgPath.Contains(pathPrefix))
                    orgPath = orgPath.Remove(0, pathPrefix.Length);

                if (orgPath != value)
                {
                    _thumbnailPath = Path.Combine(pathPrefix, value);

                    OnPropertyChanged(new PropertyChangedEventArgs("ThumbnailPath"));
                }
            }
        }

        public String PlayPath
        {
            get
            {
                if (State != ConvertStatus.ConvertedOk || CvtPath == null || CvtPath.Length == 0)
                    return null;

                String playPath = "D:\\VideoInvestigationDataDB\\CvtFile";

                return Path.Combine(playPath, CvtPath);
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
                        Operation = "未导入";
                        OpNameMargin = new Thickness(0);
                        OpName = "开始导入";
                        ProgressBarVisibility = Visibility.Hidden;
                        Opacity = 1.0;
                        Remark = "/VideoSearch;component/Resources/Images/Button/MovieImporting.png";
                        IsEnabled = true;
                        Progress = 0.0;
                    }
                    else if (_state == ConvertStatus.Importing)
                    {
                        OpIcon = "/VideoSearch;component/Resources/Images/Button/MovieImportPause.png";
                        Operation = "";
                        OpNameMargin = new Thickness(16, 0, 0, 0);
                        OpName = "停止";
                        ProgressBarVisibility = Visibility.Visible;
                        Opacity = 1.0;
                        Remark = "/VideoSearch;component/Resources/Images/Button/MovieImporting.png";
                        IsEnabled = true;
                        Progress = 0.0;
                    }
                    else if (_state == ConvertStatus.ConvertReady || _state == ConvertStatus.Converting)
                    {
                        OpIcon = "/VideoSearch;component/Resources/Images/Button/MovieImportStop.png";
                        Operation = "";
                        OpNameMargin = new Thickness(16, 0, 0, 0);
                        OpName = "停止";
                        ProgressBarVisibility = Visibility.Visible;
                        Opacity = 1.0;
                        Remark = "/VideoSearch;component/Resources/Images/Button/MovieImporting.png";
                        IsEnabled = false;
                        Progress = 0.0;
                    }
                    else if (_state == ConvertStatus.ImportingPaused)
                    {
                        OpIcon = "/VideoSearch;component/Resources/Images/Button/MovieImportResume.png";
                        Operation = "";
                        OpNameMargin = new Thickness(16, 0, 0, 0);
                        OpName = "开始";
                        ProgressBarVisibility = Visibility.Visible;
                        Opacity = 1.0;
                        Remark = "/VideoSearch;component/Resources/Images/Button/MovieImporting.png";
                        IsEnabled = true;
                        Progress = 0.0;
                    }
                    else
                    {
                        OpIcon = "/VideoSearch;component/Resources/Images/Button/MoviePlay.png";
                        Operation = "已导入";
                        OpNameMargin = new Thickness(16, 0, 0, 0);
                        OpName = "播放";
                        ProgressBarVisibility = Visibility.Hidden;
                        Opacity = 0.6;
                        Remark = "/VideoSearch;component/Resources/Images/Button/MovieImportReady.png";
                        IsEnabled = true;
                        Progress = 0.0;
                    }

                    if (Table != null)
                        Table.Update(this);
                }
            }
        }
        #endregion

        #region Override
        public override bool ClearFromDB()
        {
            if(VideoId != "0")
            {
                if(VideoAnalysis.DeleteVideo(VideoId))
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
                SubmitVideo();
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

                SubmitVideo();
            }
        }

        private void ConvertMonitorThread()
        {
            QueryVideoResponse videoResponse = null;
            do
            {
                videoResponse = VideoAnalysis.QueryVideo(VideoId);

                if (videoResponse != null)
                {
                    Progress = videoResponse.Progress / 100.0;
                }
            } while ((ConvertStatus)videoResponse.TranscodeStatus != ConvertStatus.ConvertedOk || 
                        videoResponse.Progress < 1.0);

            if (videoResponse != null &&
                (ConvertStatus)videoResponse.TranscodeStatus == ConvertStatus.ConvertedOk)
            {
                State = ConvertStatus.PlayReady;
                ThumbnailPath = videoResponse.FirstFrameBmp;
            }
            else
                State = ConvertStatus.ConvertReady;

            _monitorThread = null;
        }

        protected void SubmitVideo()
        {
            SubmitVideoResponse response = VideoAnalysis.SubmitVideoAndTransCode(this);

            if (response.IsOk)
            {
                VideoId = String.Format("{0}", response.SubmitId);

                QueryVideoResponse videoResponse = VideoAnalysis.QueryVideo(VideoId);

                SubmitTime = GMTTimeToString(videoResponse.SubmitTime);

                MovieLength = videoResponse.FileSize;
                OrgPath = videoResponse.FilePath;
                CvtPath = videoResponse.CvtPath;
                ThumbnailPath = videoResponse.FirstFrameBmp;
                Progress = videoResponse.Progress / 100.0;

                State = ConvertStatus.Converting;

                _monitorThread = new Thread(new ThreadStart(ConvertMonitorThread));
                _monitorThread.Start();
            }
            else
            {
                State = ConvertStatus.ConvertReady;

                MessageBox.Show(response.ErrorMessage);
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
