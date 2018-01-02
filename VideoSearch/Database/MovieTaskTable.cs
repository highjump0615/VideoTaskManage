using System;
using System.Data;
using System.Data.SQLite;
using VideoSearch.Model;
using VideoSearch.VideoService;

namespace VideoSearch.Database
{
    public class MovieTaskTable : TableManager
    {
        public MovieTaskTable()
            : base("MovieTask")
        {
        }

        private static MovieTaskTable _table = null;
        public static MovieTaskTable Table
        {
            get
            {
                if (_table == null)
                    _table = new MovieTaskTable();

                return _table;
            }
        }

        #region Overrite

        public override void Load(DataItemBase parent)
        {
            String sql = "select * from MovieTask where MoviePos=@MoviePos order by DisplayID Asc";

            Load(parent, sql, new SQLiteParameter[]
                {
                    new SQLiteParameter("@MoviePos",parent.ID)
                });

        }

        public override DataItemBase DataItemWithRow(DataRow row)
        {
            if (row == null)
                return null;

            MovieTaskType taskType = (MovieTaskType)row["TaskType"];
            MovieTaskState state = (MovieTaskState)row["State"];

            if (taskType == MovieTaskType.SearchTask)
                return new MovieTaskSearchItem(string.Format("{0}", row["ID"]),
                    string.Format("{0}", row["DisplayID"]),
                    string.Format("{0}", row["TaskId"]),
                    string.Format("{0}", row["Name"]),
                    string.Format("{0}", row["MoviePos"]),
                    taskType,
                    state);
            else if (taskType == MovieTaskType.OutlineTask)
                return new MovieTaskSummaryItem(string.Format("{0}", row["ID"]),
                    string.Format("{0}", row["DisplayID"]),
                    string.Format("{0}", row["TaskId"]),
                    string.Format("{0}", row["Name"]),
                    string.Format("{0}", row["MoviePos"]),
                    taskType,
                    state);
            else if (taskType == MovieTaskType.CompressTask)
                return new MovieTaskCompressItem(string.Format("{0}", row["ID"]),
                    string.Format("{0}", row["DisplayID"]),
                    string.Format("{0}", row["TaskId"]),
                    string.Format("{0}", row["Name"]),
                    string.Format("{0}", row["MoviePos"]),
                    taskType,
                    state);

            return null;
        }

        public override int Add(DataItemBase newItem)
        {
            MovieTaskItem item = (MovieTaskItem)newItem;
            String sql = "insert into MovieTask (ID,DisplayID,TaskId,Name,MoviePos,TaskType,State) values (@ID,@DisplayID,@TaskId,@Name,@MoviePos,@TaskType,@State)";
            return DBManager.ExecuteCommand(sql, new SQLiteParameter[]
                {
                    new SQLiteParameter("@ID",item.ID),
                    new SQLiteParameter("@DisplayID",item.DisplayID),
                    new SQLiteParameter("@TaskId",item.TaskId),
                    new SQLiteParameter("@Name",item.Name),
                    new SQLiteParameter("@MoviePos",item.MoviePos),
                    new SQLiteParameter("@TaskType",item.TaskType),
                    new SQLiteParameter("@State",item.State),
                });
        }

        public override int Update(DataItemBase newItem)
        {
            MovieTaskItem item = (MovieTaskItem)newItem;

            String sql = "update MovieTask set ID=@ID,DisplayID=@DisplayID,TaskId=@TaskId,Name=@Name,MoviePos=@MoviePos,TaskType=@TaskType,State=@State where ID=@ID";
            return DBManager.ExecuteCommand(sql, new SQLiteParameter[]
            {
                    new SQLiteParameter("@ID",item.ID),
                    new SQLiteParameter("@DisplayID",item.DisplayID),
                    new SQLiteParameter("@TaskId",item.TaskId),
                    new SQLiteParameter("@Name",item.Name),
                    new SQLiteParameter("@MoviePos",item.MoviePos),
                    new SQLiteParameter("@TaskType",item.TaskType),
                    new SQLiteParameter("@State",item.State),
            });
        }
        #endregion
    }
}
