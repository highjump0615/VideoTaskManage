using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using VideoSearch.Utils;
using VideoSearch.VideoService;

namespace VideoSearch.Model
{
    public class TaskSnapshot : INotifyPropertyChanged
    {
        public TaskSnapshot()
        {
            Margin = new Thickness(0);
            BlackBGVisible = Visibility.Visible;
            BorderThickness = 0;
            ViewPort = new Rect(0, 0, 1, 1);
        }

        #region Notify
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(String propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        #endregion
        
        #region Property

        public String PicTitle
        {
            get;
            set;
        }
        public String PicPath
        {
            get;
            set;
        }

        public Visibility BlackBGVisible
        {
            get;
            set;
        }

        public int BorderThickness
        {
            get;
            set;
        }

        public SolidBrush BorderBrush
        {
            get;
            set;
        }

        private Rect _viewPort = new Rect(0, 0, 1, 1);
        public Rect ViewPort
        {
            get;
            set;
        }

        private int _displayType = -1;
        public int DisplayType
        {
            set
            {
                if(_displayType != value)
                {
                    _displayType = value;

                    if(_displayType == 0)
                    {
                        Margin = _normalMargin;
                        BlackBGVisible = Visibility.Visible;
                        BorderThickness = 1;
                        BorderBrush = new SolidBrush(Color.White);
                        ViewPort = new Rect(0, 0, 1, 1);
                    }
                    else if(_displayType == 1)
                    {
                        Margin = _clipMargin;
                        BlackBGVisible = Visibility.Hidden;
                        BorderThickness = 1;
                        BorderBrush = new SolidBrush(Color.DarkGray);
                        ViewPort = _viewPort;
                    }

                    OnPropertyChanged("BlackBGVisible");
                    OnPropertyChanged("BorderThickness");
                    OnPropertyChanged("BorderBrush");
                    OnPropertyChanged("Margin");
                    OnPropertyChanged("ViewPort");
                }
            }
        }

        public int StartFrame
        {
            get;
            set;
        }

        public int EndFrame
        {
            get;
            set;
        }

        public int FrameCount
        {
            get;
            set;
        }

        public int ObjType
        {
            get;
            set;
        }

        private Thickness _normalMargin = new Thickness(0);
        private Thickness _clipMargin = new Thickness(0);

        private Thickness _margin = new Thickness(0);
        public Thickness Margin
        {
            get { return _margin; }

            set
            {
                _margin = value;
                Console.WriteLine("=== {0}, {1}, {2}, {3}", _margin.Left, _margin.Top, _margin.Right, _margin.Bottom);
            }
        }

        private Rect _objPath = new Rect();

        public Rect ObjPath
        {
            set
            {
                if(_objPath != value)
                {
                    _objPath = value;

                    BitmapImage thumbnail = new BitmapImage(new Uri(PicPath, UriKind.RelativeOrAbsolute));

                    if(thumbnail != null)
                    {
                        Rect rtContainer = new Rect(0, 0, 217, 180);
                        System.Windows.Size imgSize = new System.Windows.Size(thumbnail.Width, thumbnail.Height);

                        Rect bounds = calculateBounds(rtContainer, imgSize);

                        _normalMargin = new Thickness(bounds.Left, bounds.Top,
                            rtContainer.Right - bounds.Right, rtContainer.Bottom - bounds.Bottom);

                        // calc for clip
                        _viewPort = new Rect(_objPath.Left / imgSize.Width,
                            _objPath.Top / imgSize.Height,
                            _objPath.Right / imgSize.Width,
                            _objPath.Bottom / imgSize.Height);

                        imgSize = new System.Windows.Size(_objPath.Width, _objPath.Height);

                        bounds = calculateBounds(Rect.Inflate(rtContainer, -8, -8), imgSize);

                        _clipMargin = new Thickness(bounds.Left,
                            bounds.Top,
                            rtContainer.Right - bounds.Right,
                            rtContainer.Bottom - bounds.Bottom);

                    }
                }
            }
        }
        #endregion

        #region Utility
        protected Rect calculateBounds(Rect rt, System.Windows.Size imgSize)
        {
            Rect bounds = rt;

            double r1 = rt.Width / rt.Height;
            double r2 = imgSize.Width / imgSize.Height;
            double iw, ih;

            if (r1 > r2)
            {
                ih = rt.Height;
                iw = ih * r2;
            }
            else
            {
                iw = rt.Width;
                ih = iw / r2;
            }

            bounds = new Rect(bounds.Left + (bounds.Width - iw) / 2,
                              bounds.Top + (bounds.Height - ih) / 2,
                                iw, ih);

            return bounds;
        }
        #endregion
    };

    public class MovieTaskSummaryItem : MovieTaskItem
    {
        #region Property
        private List<TaskSnapshot> _snapshots = new List<TaskSnapshot>();
        public List<TaskSnapshot> Snapshots
        {
            get { return _snapshots; }
        }
        #endregion

        #region Constructor & Init

        public MovieTaskSummaryItem(DataItemBase parent = null)
            : base(parent)
        {
            testLoad();
        }

        public MovieTaskSummaryItem(DataItemBase parent, String id, String displayID, String taskId, String name, String moviePos,
                        MovieTaskType taskType, MovieTaskState state) 
            : base(parent, id, displayID, taskId, name, moviePos, taskType, state)
        {
            testLoad();
        }

        public MovieTaskSummaryItem(DataItemBase parent, String taskId, String name, MovieTaskType taskType) 
            : base(parent, taskId, name, taskType)
        {
            testLoad();
        }

        protected void testLoad()
        {
#if false
            for(int i=0; i<36; i++)
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

        #region Override
        public override void UpdateProperty()
        {
            XElement response = ApiManager.Instance.GetTaskSnapshot(TaskId);

            if (response != null)
            {
                _snapshots.Clear();

                foreach(XElement obj in response.Descendants("Obj"))
                {
                    TaskSnapshot snapshot = new TaskSnapshot();

                    int left = StringUtils.String2Int(obj.Element("ObjPathLeft").Value);
                    int right = StringUtils.String2Int(obj.Element("ObjPathRight").Value);
                    int top = StringUtils.String2Int(obj.Element("ObjPathTop").Value);
                    int bottom = StringUtils.String2Int(obj.Element("ObjPathBottom").Value);

                    String picPath = "D:\\VideoInvestigationDataDB\\AnalysisFile";

                    snapshot.PicPath = Path.Combine(picPath, obj.Element("PicPath").Value);
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
        #endregion
    }
}
