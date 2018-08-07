using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;
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

        public virtual DataItemBase DataItemWithRow(DataRow row, DataItemBase parent, String pfxField = "")
        {
            return null;
        }

        public virtual async Task<int> Add(DataItemBase newItem)
        {
            await Task.FromResult(0);
            return 0;
        }

        public virtual async Task<int> Update(DataItemBase newItem)
        {
            await Task.FromResult(0);
            return 0;
        }

        #endregion

        #region Internal (Load, Remove)
        public async void LoadAsync(DataItemBase parent, String sql, SQLiteParameter[] parameters = null)
        {
            if (parent == null || _tableName == null || sql == null)
                return;

            DataTable dt = DBManager.GetDataTable(sql, parameters);
            foreach (DataRow row in dt.Rows)
            {
                DataItemBase item = DataItemWithRow(row, parent);

                if (item != null)
                    await parent.AddItemAsync(item, false);
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
