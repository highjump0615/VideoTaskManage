
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

        // 标注位置大小
        public long frame;
        public int x;
        public int y;
        public int width;
        public int height;

        // 基础信息
        public String desc;
        public String keyword;

        // 0 - 人， 1 - 车
        public int type;

        // 人信息
        public int pantsColor;
        public String pantsKind;
        public String otherHumanSpec;
        public int coatColor;
        public String coatKind;
        public int hasPack;
        public int hasCap;
        public int hasGlass;
        public String name;

        // 车辆信息
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

        public override String Order { get; set; }

        public String TargetType {
            get
            {
                return DetailInfo.type == 0 ? "人" : "车";
            }
        }

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

        public String FrameInfo
        {
            get { return $"{DetailInfo.frame}"; }
        }

        public String Description
        {
            get { return DetailInfo.desc; }
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

        #endregion
    }
}