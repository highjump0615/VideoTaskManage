using System;
using System.Data;
using System.Data.SQLite;
using VideoSearch.Database;

namespace VideoSearch.Model
{
    public class VideoData : DataItemBase
    {
        public static VideoData AppVideoData = null;

        public VideoData()
            : base()
        {            
            if (AppVideoData == null)
                AppVideoData = this;

            SetLevel(0);

            ItemNamePrefix = "Event";

            ItemsTable = EventTable.Table;
        }
     }
}
