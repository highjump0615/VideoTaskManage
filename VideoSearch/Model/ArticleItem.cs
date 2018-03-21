
using System;
using System.ComponentModel;
using System.Windows;
using VideoSearch.Database;

namespace VideoSearch.Model
{
    public class DetailInfo
    {
        public String id;
        public String videoId;
        public long frame;
        public int x;
        public int y;
        public int width;
        public int height;
        public String desc;
        public String keyword;
        public String type;
    }

    public class HumanInfo : DetailInfo
    {
        public int pantsColor;
        public String pantsKind;
        public String otherHumanSpec;
        public int coatColor;
        public String coatKind;
        public int hasPack;
        public int hasCap;
        public int hasGlass;
        public String name;
    }

    public class CarInfo : DetailInfo
    {
        public String carNumber;
        public int carColor;
        public int memberCount;
        public String driver;
        public String carModel;
        public String otherCarSpec;
    }

    

    public class ArticleItem : DataItemBase
    {
        public static int LEVEL = 4;

        #region Constructor & Init

        public ArticleItem(DataItemBase parent = null)
            : base()
        {
            Parent = parent;

            SetLevel(4);

            Table = ArticleTable.Table;
        }

        public ArticleItem(DataItemBase parent, DetailInfo info) 
            : this(parent)
        {
            ID = info.id;
            DetailInfo = info;
        }
        
        #endregion

        #region Command

        public string ArticleCommand
        {
            get { return "ArticleCommand"; }
        }

        protected override void OnCommand(Object parameter)
        {
            if (parameter != null && parameter.GetType() == typeof(String) && (String)parameter == ArticleCommand)
            {
                ProcessArticle();
            }
            else
                base.OnCommand(parameter);
        }

        protected void ProcessArticle()
        {

        }

        #endregion

        #region Property

        public DetailInfo DetailInfo = null;

        private String _cameraName = "";
        public String CameraName
        {
            get { return _cameraName; }
            set
            {
                if (_cameraName != value)
                {
                    _cameraName = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CameraName"));
                }
            }
        }

        private String _cameraPos = "";
        public String CameraPos
        {
            get { return _cameraPos; }
            set
            {
                if (_cameraPos != value)
                {
                    _cameraPos = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CameraPos"));
                }
            }
        }

        private String _movieName = "";
        public String MovieName
        {
            get { return _movieName; }
            set
            {
                if (_movieName != value)
                {
                    _movieName = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("MovieName"));
                }
            }
        }

        private String _targetType = "";
        public String TargetType
        {
            get { return _targetType; }
            set
            {
                if (_targetType != value)
                {
                    _targetType = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("TargetType"));
                }
            }
        }

        private String _frameInfo = "";
        public String FrameInfo
        {
            get { return _frameInfo; }
            set
            {
                if (_frameInfo != value)
                {
                    _frameInfo = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("FrameInfo"));
                }
            }
        }

        private String _description = "";
        public String Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Description"));
                }
            }
        }
        #endregion

        #region Override
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
    }
}