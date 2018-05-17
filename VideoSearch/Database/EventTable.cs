using System;
using System.Data;
using System.Data.SQLite;
using VideoSearch.Model;

namespace VideoSearch.Database
{
    public class EventTable : TableManager
    {
        public EventTable()
            : base("Event")
        {
        }

        private static EventTable _table = null;
        public static EventTable Table
        {
            get
            {
                if (_table == null)
                    _table = new EventTable();

                return _table;
            }
        }

        #region Overrite

        public override void Load(DataItemBase parent)
        {
            String sql = "select * from Event order by ID Asc";

            Load(parent, sql);
        }

        public override DataItemBase DataItemWithRow(DataRow row, DataItemBase parent, String pfxField = "")
        {
            if (row == null)
                return null;

            return new EventItem(parent,
                                string.Format("{0}", row["ID"]),
                                string.Format("{0}", row["DisplayID"]),
                                string.Format("{0}", row["Name"]),
                                string.Format("{0}", row["Date"]),
                                string.Format("{0}", row["Remark"]), false);
        }

        public override int Add(DataItemBase newItem)
        {
            EventItem item = (EventItem)newItem;
            String sql = "insert into Event (ID,DisplayID,Name,Date,Remark) values (@ID,@DisplayID,@Name,@Date,@Remark)";
            return DBManager.ExecuteCommand(sql, new SQLiteParameter[]
                {
                    new SQLiteParameter("@ID",item.ID),
                    new SQLiteParameter("@DisplayID",item.DisplayID),
                    new SQLiteParameter("@Name",item.Name),
                    new SQLiteParameter("@Date",item.Date),
                    new SQLiteParameter("@Remark",item.Remark)
                });
        }

        public override int Update(DataItemBase newItem)
        {
            EventItem item = (EventItem)newItem;

            String sql = "update Event set ID=@ID,DisplayID=@DisplayID,Name=@Name,Date=@Date,Remark=@Remark where ID=@ID";
            return DBManager.ExecuteCommand(sql, new SQLiteParameter[]
            {
                    new SQLiteParameter("@ID",item.ID),
                    new SQLiteParameter("@DisplayID",item.DisplayID),
                    new SQLiteParameter("@Name",item.Name),
                    new SQLiteParameter("@Date",item.Date),
                    new SQLiteParameter("@Remark",item.Remark)
            });
        }

        #endregion
    }
}
