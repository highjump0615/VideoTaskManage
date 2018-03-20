using System;
using System.Data;
using System.Data.SQLite;
using VideoSearch.Model;

namespace VideoSearch.Database
{
    public class ArticleTable : TableManager
    {
        public ArticleTable()
            : base("Article")
        {
        }

        private static ArticleTable _table = null;
        public static ArticleTable Table
        {
            get
            {
                if (_table == null)
                    _table = new ArticleTable();

                return _table;
            }
        }

        #region Overrite

        public override void Load(DataItemBase parent)
        {
            String sql = "select * from Article where VideoID=@VideoID order by ID Asc";

            Load(parent, sql, new SQLiteParameter[]
                {
                    new SQLiteParameter("@VideoID",parent.ID)
                });

        }

        public override DataItemBase DataItemWithRow(DataRow row, DataItemBase parent)
        {
            if (row == null)
                return null;

            String strTargetType = String.Format("{0}", row["TargetType"]);
            if (strTargetType == null || strTargetType.Length == 0)
                return null;

            DetailInfo info = null;
            bool isHuman = true;

            if (strTargetType.CompareTo("人") == 0)
                info = new HumanInfo();
            else
            {
                isHuman = false;
                info = new CarInfo();
            }

            info.id = String.Format("{0}", row["ID"]);
            info.videoId = String.Format("{0}", row["VideoId"]);
            info.frame = Decimal.ToInt32(Decimal.Parse(String.Format("{0}", row["Frame"])));
            info.x = Decimal.ToInt32(Decimal.Parse(String.Format("{0}", row["X"])));
            info.y = Decimal.ToInt32(Decimal.Parse(String.Format("{0}", row["Y"])));
            info.width = Decimal.ToInt32(Decimal.Parse(String.Format("{0}", row["Width"])));
            info.height = Decimal.ToInt32(Decimal.Parse(String.Format("{0}", row["Height"])));
            info.desc = String.Format("{0}", row["Description"]);
            info.keyword = String.Format("{0}", row["Keyword"]);
            info.type = strTargetType;

            if (isHuman)
            {
                HumanInfo hInfo = (HumanInfo)info;

                hInfo.pantsColor = Decimal.ToInt32(Decimal.Parse(String.Format("{0}", row["PantsColor"])));
                hInfo.pantsKind = String.Format("{0}", row["PantsKind"]);
                hInfo.otherHumanSpec = String.Format("{0}", row["OtherHumanSpec"]);
                hInfo.coatColor = Decimal.ToInt32(Decimal.Parse(String.Format("{0}", row["CoatColor"])));
                hInfo.coatKind = String.Format("{0}", row["CoatKind"]);
                hInfo.hasPack = Decimal.ToInt32(Decimal.Parse(String.Format("{0}", row["HasPack"])));
                hInfo.hasCap = Decimal.ToInt32(Decimal.Parse(String.Format("{0}", row["HasCap"])));
                hInfo.hasGlass = Decimal.ToInt32(Decimal.Parse(String.Format("{0}", row["HasGlass"])));
                hInfo.name = String.Format("{0}", row["Name"]);
            }
            else
            {
                CarInfo cInfo = (CarInfo)info;

                cInfo.carNumber = String.Format("{0}", row["CarNumber"]);
                cInfo.carColor = Decimal.ToInt32(Decimal.Parse(String.Format("{0}", row["CarColor"])));
                cInfo.memberCount = Decimal.ToInt32(Decimal.Parse(String.Format("{0}", row["MemberCount"])));
                cInfo.driver = String.Format("{0}", row["Driver"]);
                cInfo.carModel = String.Format("{0}", row["CarModel"]);
                cInfo.otherCarSpec = String.Format("{0}", row["OtherCarSpec"]);
            }

            return new ArticleItem(parent, info);
        }

        public override int Add(DataItemBase newItem)
        {
            ArticleItem item = (ArticleItem)newItem;
            DetailInfo info = item.DetailInfo;
            SQLiteParameter[] param = null;

            String sql = "insert into Article (ID, VideoId, Frame, X, Y, Width, Height, Description, Keyword, TargetType, ";
            String strVals = "values(@ID, @VideoId, @Frame, @X, @Y, @Width, @Height, @Description, @Keyword, @TargetType, ";

            if (item.TargetType.CompareTo("人") == 0)
            {
                sql += "PantsColor, PantsKind, OtherHumanSpec, CoatColor, CoatKind, HasPack, HasCap, HasGlass, Name)";
                sql += strVals + "@PantsColor, @PantsKind, @OtherHumanSpec, @CoatColor, @CoatKind, @HasPack, @HasCap, @HasGlass, @Name)";

                HumanInfo hInfo = (HumanInfo)info;

                param = new SQLiteParameter[]
                {
                    new SQLiteParameter("@ID", info.id),
                    new SQLiteParameter("@VideoId", info.videoId),
                    new SQLiteParameter("@Frame", info.frame),
                    new SQLiteParameter("@X", info.x),
                    new SQLiteParameter("@Y", info.y),
                    new SQLiteParameter("@Width", info.width),
                    new SQLiteParameter("@Height", info.height),
                    new SQLiteParameter("@Description", info.desc),
                    new SQLiteParameter("@Keyword", info.keyword),
                    new SQLiteParameter("@TargetType", info.type),

                    new SQLiteParameter("@PantsColor",hInfo.pantsColor),
                    new SQLiteParameter("@PantsKind",hInfo.pantsKind),
                    new SQLiteParameter("@OtherHumanSpec",hInfo.otherHumanSpec),
                    new SQLiteParameter("@CoatColor",hInfo.coatColor),
                    new SQLiteParameter("@CoatKind",hInfo.coatKind),
                    new SQLiteParameter("@HasPack",hInfo.hasPack),
                    new SQLiteParameter("@HasCap",hInfo.hasCap),
                    new SQLiteParameter("@HasGlass",hInfo.hasGlass),
                    new SQLiteParameter("@Name",hInfo.name),
                };
            }
            else if (item.TargetType.CompareTo("车") == 0)
            {
                sql += "CarNumber, CarColor, MemberCount, Driver, CarModel, OtherCarSpec)";
                sql += strVals + "@CarNumber, @CarColor, @MemberCount, @Driver, @CarModel, @OtherCarSpec)";

                CarInfo cInfo = (CarInfo)info;

                param = new SQLiteParameter[]
                {
                    new SQLiteParameter("@ID", info.id),
                    new SQLiteParameter("@VideoId", info.videoId),
                    new SQLiteParameter("@Frame", info.frame),
                    new SQLiteParameter("@X", info.x),
                    new SQLiteParameter("@Y", info.y),
                    new SQLiteParameter("@Width", info.width),
                    new SQLiteParameter("@Height", info.height),
                    new SQLiteParameter("@Description", info.desc),
                    new SQLiteParameter("@Keyword", info.keyword),
                    new SQLiteParameter("@TargetType", info.type),

                    new SQLiteParameter("@CarNumber",cInfo.carNumber),
                    new SQLiteParameter("@CarColor",cInfo.carColor),
                    new SQLiteParameter("@MemberCount",cInfo.memberCount),
                    new SQLiteParameter("@Driver",cInfo.driver),
                    new SQLiteParameter("@CarModel",cInfo.carModel),
                    new SQLiteParameter("@OtherCarSpec",cInfo.otherCarSpec),
                };

            }
            return param == null ? 0 : DBManager.ExecuteCommand(sql, param);
        }

        public override int Update(DataItemBase newItem)
        {
            ArticleItem item = (ArticleItem)newItem;
            DetailInfo info = item.DetailInfo;

            String sql = "update Article set ID=@ID, VideoId=@VideoId, Frame=@Frame, X=@X, Y=@Y, Width=@Width, Height=@Height, Description=@Description, Keyword=@Keyword, TargetType=@TargetType, ";

            if (item.TargetType.CompareTo("人") == 0)
            {
                sql += "PantsColor=@PantsColor, PantsKind=@PantsKind, OtherHumanSpec=@OtherHumanSpec, CoatColor=@CoatColor, CoatKind=@CoatKind, HasPack=@HasPack, HasCap=@HasCap, HasGlass=@HasGlass, Name=@Name where ID=@ID";
                HumanInfo hInfo = (HumanInfo)info;

                return DBManager.ExecuteCommand(sql, new SQLiteParameter[]
                {
                    new SQLiteParameter("@ID", info.id),
                    new SQLiteParameter("@VideoId", info.videoId),
                    new SQLiteParameter("@Frame", info.frame),
                    new SQLiteParameter("@X", info.x),
                    new SQLiteParameter("@Y", info.y),
                    new SQLiteParameter("@Width", info.width),
                    new SQLiteParameter("@Height", info.height),
                    new SQLiteParameter("@Description", info.desc),
                    new SQLiteParameter("@Keyword", info.keyword),
                    new SQLiteParameter("@TargetType", info.type),

                    new SQLiteParameter("@PantsColor",hInfo.pantsColor),
                    new SQLiteParameter("@PantsKind",hInfo.pantsKind),
                    new SQLiteParameter("@OtherHumanSpec",hInfo.otherHumanSpec),
                    new SQLiteParameter("@CoatColor",hInfo.coatColor),
                    new SQLiteParameter("@CoatKind",hInfo.coatKind),
                    new SQLiteParameter("@HasPack",hInfo.hasPack),
                    new SQLiteParameter("@HasCap",hInfo.hasCap),
                    new SQLiteParameter("@HasGlass",hInfo.hasGlass),
                    new SQLiteParameter("@Name",hInfo.name),
                });

            }
            else if (item.TargetType.CompareTo("车") == 0)
            {
                sql += "CarNumber=@CarNumber, CarColor=@CarColor, MemberCount=@MemberCount, Driver=@Driver, CarModel=@CarModel, OtherCarSpec=@OtherCarSpec where ID=@ID";
                CarInfo cInfo = (CarInfo)info;

                return DBManager.ExecuteCommand(sql, new SQLiteParameter[]
                {
                    new SQLiteParameter("@ID", info.id),
                    new SQLiteParameter("@VideoId", info.videoId),
                    new SQLiteParameter("@Frame", info.frame),
                    new SQLiteParameter("@X", info.x),
                    new SQLiteParameter("@Y", info.y),
                    new SQLiteParameter("@Width", info.width),
                    new SQLiteParameter("@Height", info.height),
                    new SQLiteParameter("@Description", info.desc),
                    new SQLiteParameter("@Keyword", info.keyword),
                    new SQLiteParameter("@TargetType", info.type),

                    new SQLiteParameter("@CarNumber",cInfo.carNumber),
                    new SQLiteParameter("@CarColor",cInfo.carColor),
                    new SQLiteParameter("@MemberCount",cInfo.memberCount),
                    new SQLiteParameter("@Driver",cInfo.driver),
                    new SQLiteParameter("@CarModel",cInfo.carModel),
                    new SQLiteParameter("@OtherCarSpec",cInfo.otherCarSpec),
                });
            }

            return 0;
        }
        #endregion
    }
}
