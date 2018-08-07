using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Threading.Tasks;
using System.Windows;
using VideoSearch.Utils;

namespace VideoSearch.Database
{
    public class DBManager
    {
        #region Constant
        public const String DBName = "VideoSearch.db";
        public const String EventTable = "Event";
        public const String CameraTable = "Camera";
        public const String MovieTable = "Movie";
        public const String MovieTaskTable = "MovieTask";
        public const String ArticleTable = "Article";
        #endregion        

        #region Property

        private static SQLiteConnection _connection = null;
        public static SQLiteConnection Connection
        {
            get
            {
                try
                {
                    if (_connection == null)
                    {
                        SQLiteConnectionStringBuilder strConnection = new SQLiteConnectionStringBuilder();
                        strConnection.DataSource = DBName;


                        _connection = new SQLiteConnection(strConnection.ToString());
                        _connection.Open();
                    }
                    else if (_connection.State == System.Data.ConnectionState.Broken)
                    {
                        _connection.Close();
                        _connection.Open();
                    }
                    else if (_connection.State == System.Data.ConnectionState.Closed)
                    {
                        _connection.Open();
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine("DBManager Exception : Msg={0} : {1} ", exception.Message, exception);

#if false
                    throw new Exception("打开数据库连接失败");
#else
                    ExceptionUtils.ShowMessage("打开数据库连接失败", exception);
#endif
                }
                return DBManager._connection;

            }
            set
            {
                _connection = value;
            }
        }

        #endregion

        #region Common Utility
        public static ObservableCollection<String> EventIDList()
        {
            ObservableCollection<String> eventList = new ObservableCollection<String>();
            String sql = "select ID from Event order by ID Asc";
            DataTable dt = GetDataTable(sql, null);
            foreach (DataRow row in dt.Rows)
            {
                eventList.Add(String.Format("{0}", row["ID"]));
            }

            return eventList;
        }

        public static ObservableCollection<String> EventDisplayIDList()
        {
            ObservableCollection<String> eventList = new ObservableCollection<String>();
            String sql = "select DisplayID from Event order by ID Asc";
            DataTable dt = GetDataTable(sql, null);
            foreach (DataRow row in dt.Rows)
            {
                eventList.Add(String.Format("{0}", row["DisplayID"]));
            }

            return eventList;
        }

        public static SQLiteCommand GetCommand()
        {
            SQLiteCommand cmd = new SQLiteCommand();
            SQLiteTransaction tran = Connection.BeginTransaction();
            cmd.Transaction = tran;
            cmd.Connection = Connection;
            return cmd;
        }

        public static DataTable GetDataTable(String sql, params SQLiteParameter[] sqliteParameter)
        {
            DataTable dt = new DataTable();

            SQLiteCommand cmd = new SQLiteCommand(sql, Connection);
            if (sqliteParameter != null)
                cmd.Parameters.AddRange(sqliteParameter);
            SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
            try
            {
                adapter.Fill(dt);
            }
            catch (Exception exception)
            {
                Console.WriteLine("DBManager Exception : Msg={0} : {1} ", exception.Message, exception);

#if false
                throw new Exception("查询失败");
#else
                ExceptionUtils.ShowMessage("查询失败", exception);
#endif
            }
            cmd.Dispose();
            Connection = null;
            return dt;
        }

        public static void Commit(SQLiteCommand cmd)
        {
            cmd.Transaction.Commit();
            cmd.Connection.Close();
            cmd.Dispose();
            Connection = null;
        }

        public static void Rollback(SQLiteCommand cmd)
        {
            cmd.Transaction.Rollback();
            cmd.Connection.Close();
            cmd.Dispose();
            Connection = null;
        }

        public static int ExecuteCommand(SQLiteCommand cmd, String sql, params SQLiteParameter[] sqliteParameter)
        {
            cmd.CommandText = sql;
            if (sqliteParameter != null)
                cmd.Parameters.AddRange(sqliteParameter);
            try
            {
                int result = cmd.ExecuteNonQuery();
                return result;
            }
            catch (Exception exception)
            {
                Console.WriteLine("DBManager Exception : Msg={0} : {1} ", exception.Message, exception);
#if false
                    throw new Exception("执行语句出错");
#else
                ExceptionUtils.ShowMessage("执行语句出错", exception);
                return 0;
#endif
            }
        }

        public static async Task<int> ExecuteCommandAsync(String sql, params SQLiteParameter[] sqliteParameter)
        {
            int result = 0;
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(sql, Connection);
                if (sqliteParameter != null)
                    cmd.Parameters.AddRange(sqliteParameter);
                result = await cmd.ExecuteNonQueryAsync();
                cmd.Dispose();
            }
            catch (Exception exception)
            {
                Console.WriteLine("DBManager Exception : Msg={0} : {1} ", exception.Message, exception);
#if false
                    throw new Exception("执行语句出错");
#else
                ExceptionUtils.ShowMessage("执行语句出错", exception);
#endif
            }
            finally
            {
                Connection.Close();
                Connection = null;
            }

            return result;
        }
        #endregion
    }
}
