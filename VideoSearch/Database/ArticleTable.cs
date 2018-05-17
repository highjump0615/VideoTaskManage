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

        public override DataItemBase DataItemWithRow(DataRow row, DataItemBase parent, String pfxField = "")
        {
            if (row == null)
                return null;

            DetailInfo info = new DetailInfo();

            info.id = String.Format("{0}", row[$"ArticleID"]);

            info.videoId = String.Format("{0}", row[$"{pfxField}VideoId"]);

            info.frame = Decimal.ToInt32(Decimal.Parse(String.Format("{0}", row[$"{pfxField}Frame"])));
            info.x = Decimal.ToInt32(Decimal.Parse(String.Format("{0}", row[$"{pfxField}X"])));
            info.y = Decimal.ToInt32(Decimal.Parse(String.Format("{0}", row[$"{pfxField}Y"])));
            info.width = Decimal.ToInt32(Decimal.Parse(String.Format("{0}", row[$"{pfxField}Width"])));
            info.height = Decimal.ToInt32(Decimal.Parse(String.Format("{0}", row[$"{pfxField}Height"])));

            info.desc = String.Format("{0}", row[$"{pfxField}Description"]);
            info.keyword = String.Format("{0}", row[$"{pfxField}Keyword"]);
            info.type = Decimal.ToInt32(Decimal.Parse(String.Format("{0}", row[$"{pfxField}TargetType"])));

            // 人
            info.pantsColor = Decimal.ToInt32(Decimal.Parse(String.Format("{0}", row[$"{pfxField}PantsColor"])));
            info.pantsKind = String.Format("{0}", row[$"{pfxField}PantsKind"]);
            info.otherHumanSpec = String.Format("{0}", row[$"{pfxField}OtherHumanSpec"]);
            info.coatColor = Decimal.ToInt32(Decimal.Parse(String.Format("{0}", row[$"{pfxField}CoatColor"])));
            info.coatKind = String.Format("{0}", row[$"{pfxField}CoatKind"]);
            info.hasPack = Decimal.ToInt32(Decimal.Parse(String.Format("{0}", row[$"{pfxField}HasPack"])));
            info.hasCap = Decimal.ToInt32(Decimal.Parse(String.Format("{0}", row[$"{pfxField}HasCap"])));
            info.hasGlass = Decimal.ToInt32(Decimal.Parse(String.Format("{0}", row[$"{pfxField}HasGlass"])));
            info.name = String.Format("{0}", row[$"{pfxField}Name"]);

            // 车
            info.carNumber = String.Format("{0}", row[$"{pfxField}CarNumber"]);
            info.carColor = Decimal.ToInt32(Decimal.Parse(String.Format("{0}", row[$"{pfxField}CarColor"])));
            info.memberCount = Decimal.ToInt32(Decimal.Parse(String.Format("{0}", row[$"{pfxField}MemberCount"])));
            info.driver = String.Format("{0}", row[$"{pfxField}Driver"]);
            info.carModel = String.Format("{0}", row[$"{pfxField}CarModel"]);
            info.otherCarSpec = String.Format("{0}", row[$"{pfxField}OtherCarSpec"]);
            

            return new ArticleItem(parent, info);
        }

        public override int Add(DataItemBase newItem)
        {
            ArticleItem item = (ArticleItem)newItem;
            DetailInfo info = item.DetailInfo;
            SQLiteParameter[] param = null;

            String sql = "insert into Article (ID, VideoId, Frame, X, Y, Width, Height, Description, Keyword, TargetType, ";
            String strVals = "values(@ID, @VideoId, @Frame, @X, @Y, @Width, @Height, @Description, @Keyword, @TargetType, ";

            sql += "PantsColor, PantsKind, OtherHumanSpec, CoatColor, CoatKind, HasPack, HasCap, HasGlass, Name, CarNumber, CarColor, MemberCount, Driver, CarModel, OtherCarSpec)";
            sql += strVals + "@PantsColor, @PantsKind, @OtherHumanSpec, @CoatColor, @CoatKind, @HasPack, @HasCap, @HasGlass, @Name, @CarNumber, @CarColor, @MemberCount, @Driver, @CarModel, @OtherCarSpec)";

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

                // 人
                new SQLiteParameter("@PantsColor",info.pantsColor),
                new SQLiteParameter("@PantsKind",info.pantsKind),
                new SQLiteParameter("@OtherHumanSpec",info.otherHumanSpec),
                new SQLiteParameter("@CoatColor",info.coatColor),
                new SQLiteParameter("@CoatKind",info.coatKind),
                new SQLiteParameter("@HasPack",info.hasPack),
                new SQLiteParameter("@HasCap",info.hasCap),
                new SQLiteParameter("@HasGlass",info.hasGlass),
                new SQLiteParameter("@Name",info.name),

                // 车
                new SQLiteParameter("@CarNumber",info.carNumber),
                new SQLiteParameter("@CarColor",info.carColor),
                new SQLiteParameter("@MemberCount",info.memberCount),
                new SQLiteParameter("@Driver",info.driver),
                new SQLiteParameter("@CarModel",info.carModel),
                new SQLiteParameter("@OtherCarSpec",info.otherCarSpec),
            };

            return param == null ? 0 : DBManager.ExecuteCommand(sql, param);
        }

        public override int Update(DataItemBase newItem)
        {
            ArticleItem item = (ArticleItem)newItem;
            DetailInfo info = item.DetailInfo;

            String sql = "update Article set ID=@ID, VideoId=@VideoId, Frame=@Frame, X=@X, Y=@Y, Width=@Width, Height=@Height, Description=@Description, Keyword=@Keyword, TargetType=@TargetType, ";
            sql += "PantsColor=@PantsColor, PantsKind=@PantsKind, OtherHumanSpec=@OtherHumanSpec, CoatColor=@CoatColor, CoatKind=@CoatKind, HasPack=@HasPack, HasCap=@HasCap, HasGlass=@HasGlass, Name=@Name, " +
                "CarNumber=@CarNumber, CarColor=@CarColor, MemberCount=@MemberCount, Driver=@Driver, CarModel=@CarModel, OtherCarSpec=@OtherCarSpec where ID=@ID " +
                "where ID=@ID";

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

                // 人
                new SQLiteParameter("@PantsColor",info.pantsColor),
                new SQLiteParameter("@PantsKind",info.pantsKind),
                new SQLiteParameter("@OtherHumanSpec",info.otherHumanSpec),
                new SQLiteParameter("@CoatColor",info.coatColor),
                new SQLiteParameter("@CoatKind",info.coatKind),
                new SQLiteParameter("@HasPack",info.hasPack),
                new SQLiteParameter("@HasCap",info.hasCap),
                new SQLiteParameter("@HasGlass",info.hasGlass),
                new SQLiteParameter("@Name",info.name),

                // 车
                new SQLiteParameter("@CarNumber",info.carNumber),
                new SQLiteParameter("@CarColor",info.carColor),
                new SQLiteParameter("@MemberCount", info.memberCount),
                new SQLiteParameter("@Driver", info.driver),
                new SQLiteParameter("@CarModel", info.carModel),
                new SQLiteParameter("@OtherCarSpec", info.otherCarSpec),
            });
        }
        #endregion
    }
}
