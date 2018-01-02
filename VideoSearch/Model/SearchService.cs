using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace VideoSearch.Model
{
    public enum SearchFilterType
    {
        UnInited,
        SearchAll,
        SearchEvent,
        SearchCamera
    }

    public delegate void SearchStartedHandelr();
    public delegate void SearchCompletedHandelr(int count);
    public delegate void FoundNewResultHandler(FormattedText result);

    public class SearchService
    {
        #region Gloabal Data
        static SearchService _videoSearch = null;

        public static SearchService VideoSearch
        {
            get
            {
                if (_videoSearch == null)
                    _videoSearch = new SearchService();

                return _videoSearch;
            }
        }
        #endregion

        #region Constructor
        public SearchService()
        {
            _maxWidth = 1920;
            _fontSize = 14;
            _fontFamily = "Arial";
            _textBrush = Brushes.White;
            _highliteTextBrush = new SolidColorBrush(Color.FromRgb(86, 255, 166));

            Filter = SearchFilterType.UnInited;

            _dataItems = new List<DataItemBase>();
        }

        public SearchService(double maxWidth = 1920, double fontSize = 14, String fontFamily = "Arial")
            : this()
        {
            _maxWidth = maxWidth;
            _fontSize = fontSize;
            _fontFamily = fontFamily;
         }
        #endregion

        #region Member Values
        private SolidColorBrush _textBrush = null;
        private SolidColorBrush _highliteTextBrush = null;
        private double _fontSize = 0;
        private double _maxWidth = 0;
        private String _fontFamily = null;

        private SearchFilterType _filter = SearchFilterType.UnInited;
        private String _searchText = null;
        private List<DataItemBase> _dataItems = null;

        private Thread _searchThread = null;

        public SearchStartedHandelr SearchStarted = null;
        public SearchCompletedHandelr SearchCompleted = null;
        public FoundNewResultHandler FoundNewResult = null;

        #endregion

        #region Property

        public double MaxWidth
        {
            set { _maxWidth = value; }
        }

        public double FontSize
        {
            set { _fontSize = value; }
        }

        public String FontFamily
        {
            set { _fontFamily = value; }
        }

        public SearchFilterType Filter
        {
            get { return _filter; }
            set
            {
                if (_filter != value)
                {
                    _filter = value;
                }
            }
        }

        public String SearchText
        {
            get { return _searchText; }
            set
            {
                if(_searchText != value)
                {
                    _searchText = value;
                }
            }
        }

        #endregion

        #region Method
        public void SelectResult(int index)
        {
            if (index < 0 || index >= _dataItems.Count)
                return;

            DataItemBase item = _dataItems[index];

            if(item != null)
            {
                DataItemBase parent = item.Parent;

                if (parent != null)
                    parent.IsSelected = true;

                item.IsChecked = true;
            }
        }

        public void StartSearch()
        {
            StopSearch();

            _dataItems.Clear();

            SearchStarted?.Invoke();

            _searchThread = new Thread(new ThreadStart(Search));
            _searchThread.Start();
        }

        public void StopSearch()
        {
            if(_searchThread != null)
            {
                if (_searchThread.ThreadState == ThreadState.Running)
                    _searchThread.Abort();

                _searchThread = null;
                SearchCompleted?.Invoke(_dataItems.Count);
            }
        }

        protected void AddResult(FormattedText result, DataItemBase item)
        {
            if (_dataItems.Contains(item))
                return;

            _dataItems.Add(item);

            FoundNewResult?.Invoke(result);
        }

        protected void Search()
        {
            String searchText = SearchText;

            bool isEventSearch = false;
            bool isCameraSearch = false;

            if (Filter == SearchFilterType.SearchAll)
            {
                isEventSearch = true;
                isCameraSearch = true;
            }
            else if (Filter == SearchFilterType.SearchEvent)
                isEventSearch = true;
            else if (Filter == SearchFilterType.SearchCamera)
                isCameraSearch = true;

            foreach (DataItemBase eventItem in VideoData.AppVideoData.Children)
            {
                if(isEventSearch)
                    SearchOnItem(searchText, eventItem);
                if(isCameraSearch)
                {
                    foreach (DataItemBase cameraItem in eventItem.Children)
                        SearchOnItem(searchText, cameraItem);
                }
            }

            StopSearch();
       }

        protected void SearchOnItem(String searchText, DataItemBase item)
        {
            Thread.Sleep(0);

            if (item.ContainsText(searchText))
                AddSearchText(searchText, item);
        }

        protected void AddSearchText(String searchText, DataItemBase item)
        {
            String strSearch = item.GetDisplaySearchText();
            int realIndex = 0, startIndex = 0, nCount = searchText.Length;

            FormattedText fmtText = new FormattedText(strSearch,
                                            CultureInfo.GetCultureInfo("en-us"),
                                            FlowDirection.LeftToRight,
                                            new Typeface(_fontFamily),
                                            _fontSize,
                                            _textBrush);

            fmtText.MaxLineCount = 1;
            fmtText.MaxTextWidth = _maxWidth;
            fmtText.Trimming = TextTrimming.CharacterEllipsis;

            do
            {
                Thread.Sleep(0);
                startIndex = strSearch.IndexOf(searchText);

                if(startIndex >= 0)
                {
                    strSearch = strSearch.Remove(0, startIndex + nCount);
                    realIndex += startIndex;
                    fmtText.SetForegroundBrush(_highliteTextBrush, realIndex, nCount);
                    realIndex += nCount;
                }

            } while (startIndex >= 0);

            AddResult(fmtText, item);
        }

        #endregion
    }
}
