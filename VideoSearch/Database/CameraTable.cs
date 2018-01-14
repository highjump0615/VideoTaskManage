using System;
using System.Data;
using System.Data.SQLite;
using VideoSearch.Model;

namespace VideoSearch.Database
{
    public class CameraTable : TableManager
    {
        public CameraTable()
            : base("Camera")
        {
        }

        private static CameraTable _table = null;
        public static CameraTable Table
        {
            get
            {
                if (_table == null)
                    _table = new CameraTable();

                return _table;
            }
        }

        #region Overrite

        public override void Load(DataItemBase parent)
        {
            if (parent.ID == null || parent.ID.Length == 0)
                return;

            String sql = "select * from Camera where EventPos=@EventPos order by DisplayID Asc";

            Load(parent, sql, new SQLiteParameter[]
                {
                    new SQLiteParameter("@EventPos",parent.ID)
                });
        }

        public override DataItemBase DataItemWithRow(DataRow row, DataItemBase parent)
        {
            if (row == null)
                return null;

            return new CameraItem(parent,
                                  string.Format("{0}", row["EventPos"]),
                                  string.Format("{0}", row["ID"]),
                                  string.Format("{0}", row["DisplayID"]),
                                  string.Format("{0}", row["Name"]),
                                  string.Format("{0}", row["Address"]),
                                  string.Format("{0}", row["Longitude"]),
                                  string.Format("{0}", row["Latitude"]),
                                  string.Format("{0}", row["CameraType"]),
                                  string.Format("{0}", row["CameraSource"]),
                                  string.Format("{0}", row["PortCount"]));
        }

        public override int Add(DataItemBase newItem)
        {
            CameraItem item = (CameraItem)newItem;
            String sql = "insert into Camera (ID,DisplayID,Name,EventPos,Address,Longitude,Latitude,CameraType,CameraSource,PortCount) values (@ID,@DisplayID,@Name,@EventPos,@Address,@Longitude,@Latitude,@CameraType,@CameraSource,@PortCount)";
            return DBManager.ExecuteCommand(sql, new SQLiteParameter[]
                {
                    new SQLiteParameter("@ID",item.ID),
                    new SQLiteParameter("@DisplayID",item.DisplayID),
                    new SQLiteParameter("@Name",item.Name),
                    new SQLiteParameter("@EventPos",item.EventPos),
                    new SQLiteParameter("@Address",item.Address),
                    new SQLiteParameter("@Longitude",item.Longitude),
                    new SQLiteParameter("@Latitude",item.Latitude),
                    new SQLiteParameter("@CameraType",item.Type),
                    new SQLiteParameter("@CameraSource",item.Source),
                    new SQLiteParameter("@PortCount",item.PortCount),
                });
        }

        public override int Update(DataItemBase newItem)
        {
            CameraItem item = (CameraItem)newItem;

            String sql = "update Camera set ID=@ID,DisplayID=@DisplayID,Name=@Name,EventPos=@EventPos,Address=@Address,Longitude=@Longitude,Latitude=@Latitude,CameraType=@CameraType,CameraSource=@CameraSource,PortCount=@PortCount where ID=@ID and EventPos=@EventPos";
            return DBManager.ExecuteCommand(sql, new SQLiteParameter[]
            {
                    new SQLiteParameter("@ID", item.ID),
                    new SQLiteParameter("@DisplayID",item.DisplayID),
                    new SQLiteParameter("@Name", item.Name),
                    new SQLiteParameter("@EventPos", item.EventPos),
                    new SQLiteParameter("@Address", item.Address),
                    new SQLiteParameter("@Longitude", item.Longitude),
                    new SQLiteParameter("@Latitude", item.Latitude),
                    new SQLiteParameter("@CameraType", item.Type),
                    new SQLiteParameter("@CameraSource", item.Source),
                    new SQLiteParameter("@PortCount", item.PortCount),
            });
        }

        #endregion
    }

}
