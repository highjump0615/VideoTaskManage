using System;
using System.Collections.ObjectModel;

namespace VideoSearch.SkinControl.Thumbnail
{
    public class Thumbnail
    {
        #region Property
        private String _thumbnailPath = null;
        public String ThumbnailPath
        {
            get { return _thumbnailPath; }
            set
            {
                if(_thumbnailPath != value)
                {
                    _thumbnailPath = value;
                }
            }
        }

        private String _thumbTitle = null;
        public String ThumbTitle
        {
            get { return _thumbTitle; }
            set
            {
                if (_thumbTitle != value)
                {
                    _thumbTitle = value;
                }
            }
        }
        #endregion

        public Thumbnail(String thumbnailPath, String thumbnailTitle)
        {
            ThumbnailPath = thumbnailPath;
            ThumbTitle = thumbnailTitle;
        }
    }

    public class ThumbnailList : ObservableCollection<Thumbnail>
    {
        public ThumbnailList()
        {
            for(int i=0; i<8; i++)
            {
                Thumbnail thumb = new Thumbnail("", String.Format("{0}", i));
                Add(thumb);
            }
        }
    }
}
