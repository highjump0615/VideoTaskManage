using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using VideoSearch.Model;

namespace VideoSearch.Database
{
    public class TableManager
    {

        private String _tableName = null;
        public TableManager(String tableName)
        {
            _tableName = tableName;
       }

        #region Virtual Function (Load, DataItemWithRow, Add, Update, Remove)
        public virtual void Load(DataItemBase parent)
        {

        }

        public virtual DataItemBase DataItemWithRow(DataRow row)
        {
            return null;
        }


        public virtual int Add(DataItemBase newItem)
        {
            return 0;
        }

        public virtual int Update(DataItemBase newItem)
        {
            return 0;
        }
        #endregion

        #region Internal (Load, Remove)
        public void Load(DataItemBase parent, String sql, SQLiteParameter[] parameters = null)
        {
            if (parent == null || _tableName == null || sql == null)
                return;

            DataTable dt = DBManager.GetDataTable(sql, parameters);
            foreach (DataRow row in dt.Rows)
            {
                DataItemBase item = DataItemWithRow(row);

                if (item != null)
                    parent.AddItem(item, false);
            }
        }

        public int Remove(DataItemBase item)
        {
            int result = 0;
            SQLiteCommand cmd = null;
            try
            {
                cmd = DBManager.GetCommand();
                String sql = String.Format("delete from {0} where ID = @ID  ", _tableName);
                result = DBManager.ExecuteCommand(cmd, sql, new SQLiteParameter("@ID", item.ID));
                DBManager.Commit(cmd);
                return result;
            }
            catch (Exception exception)
            {
                DBManager.Rollback(cmd);

#if false
                throw exception;
#else
                MessageBox.Show(exception.Message, "Remove fail!");
                return 0;
#endif
            }
        }
#endregion
    }
}
