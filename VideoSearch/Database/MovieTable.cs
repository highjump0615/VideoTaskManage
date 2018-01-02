using System;
using System.Data;
using System.Data.SQLite;
using VideoSearch.Model;
using VideoSearch.VideoService;

namespace VideoSearch.Database
{
    public class MovieTable : TableManager
    {
        public MovieTable()
            : base("Movie")
        {
        }

        private static MovieTable _table = null;
        public static MovieTable Table
        {
            get
            {
                if (_table == null)
                    _table = new MovieTable();

                return _table;
            }
        }

        #region Overrite

        public override void Load(DataItemBase parent)
        {
            String sql = "select * from Movie where CameraPos=@CameraPos order by DisplayID Asc";

            Load(parent, sql, new SQLiteParameter[]
                {
                    new SQLiteParameter("@CameraPos",parent.ID)
                });

        }

        public override DataItemBase DataItemWithRow(DataRow row)
        {
            if (row == null)
                return null;

            return new MovieItem(string.Format("{0}", row["ID"]),
                    string.Format("{0}", row["DisplayID"]),
                    string.Format("{0}", row["VideoId"]),
                    string.Format("{0}", row["Name"]),
                    string.Format("{0}", row["CameraPos"]),
                    string.Format("{0}", row["SrcPath"]),
                    (ConvertStatus)row["State"],
                    string.Format("{0}", row["MovieTask"]));
        }

        public override int Add(DataItemBase newItem)
        {
            MovieItem item = (MovieItem)newItem;
            String sql = "insert into Movie (ID,DisplayID,VideoId,Name,CameraPos,SrcPath,State,MovieTask) values (@ID,@DisplayID,@VideoId,@Name,@CameraPos,@SrcPath,@State,@MovieTask)";
            return DBManager.ExecuteCommand(sql, new SQLiteParameter[]
                {
                    new SQLiteParameter("@ID",item.ID),
                    new SQLiteParameter("@DisplayID",item.DisplayID),
                    new SQLiteParameter("@VideoId",item.VideoId),
                    new SQLiteParameter("@Name",item.Name),
                    new SQLiteParameter("@CameraPos",item.CameraPos),
                    new SQLiteParameter("@SrcPath",item.SrcPath),
                    new SQLiteParameter("@State",item.State),
                    new SQLiteParameter("@MovieTask",item.MovieTask),
                });
        }

        public override int Update(DataItemBase newItem)
        {
            MovieItem item = (MovieItem)newItem;

            String sql = "update Movie set ID=@ID,DisplayID=@DisplayID,VideoId=@VideoId,Name=@Name,CameraPos=@CameraPos,SrcPath=@SrcPath,State=@State,MovieTask=@MovieTask where ID=@ID";
            return DBManager.ExecuteCommand(sql, new SQLiteParameter[]
            {
                    new SQLiteParameter("@ID",item.ID),
                    new SQLiteParameter("@DisplayID",item.DisplayID),
                    new SQLiteParameter("@VideoId",item.VideoId),
                    new SQLiteParameter("@Name",item.Name),
                    new SQLiteParameter("@CameraPos",item.CameraPos),
                    new SQLiteParameter("@SrcPath",item.SrcPath),
                    new SQLiteParameter("@State",item.State),
                    new SQLiteParameter("@MovieTask",item.MovieTask),
            });
        }
        #endregion
    }
}
