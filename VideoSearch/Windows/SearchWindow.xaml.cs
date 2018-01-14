using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using VideoSearch.Model;
using VideoSearch.SearchListView;

namespace VideoSearch.Windows
{
    public delegate void ClearResultDelegate();
    public delegate void AppendResultDelegate(FormattedText fmtText);
    public delegate void PlaceHolderVisibleChangDelegate(bool isVisible);
    public delegate void GetSearchTextDelegate();

    public partial class SearchWindow : Window
    {
        #region Property

        private Timer _searchTimer = null;
        private String _searchText = "";

        private SearchFilterType _searchType = SearchFilterType.UnInited;
        public SearchFilterType SearchType
        {
            get { return _searchType; }
            set
            {
                if(_searchType != value)
                {
                    _searchType = value;
                    UpdateSearchFilter();
                }
            }
        }
        #endregion

        #region Constructor & Init
        public SearchWindow()
        {
            InitializeComponent();

            //Owner = MainWindow.VideoSearchMainWindow;

            SearchType = SearchFilterType.SearchAll;
        }

        #endregion

        #region Window Handler

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SearchService.VideoSearch.SearchStarted += SearchStarted;
            SearchService.VideoSearch.SearchCompleted += SearchCompleted;
            SearchService.VideoSearch.FoundNewResult += FoundNewResult;
            SearchService.VideoSearch.MaxWidth = 498;

            Binding searchBinding = new Binding();
            searchBinding.Source = SearchResult.Result;
            searchBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            SearchResultList.SetBinding(ListView.ItemsSourceProperty, searchBinding);

            textPlaceHolder.Visibility = Visibility.Visible;
            SearchBox.Text = "";
            SearchBox.TextChanged += OnSearchTextChange;
            Height = 110 + mainFrame.ShadowSize * 2;
            Top -= 180;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            SearchResultList.ItemsSource = null;

            SearchService.VideoSearch.SearchStarted -= SearchStarted;
            SearchService.VideoSearch.SearchCompleted -= SearchCompleted;
            SearchService.VideoSearch.FoundNewResult -= FoundNewResult;

            ResetSearchTimer();
        }

        private void OnDeactivated(object sender, EventArgs e)
        {
            Hide();

            SearchService.VideoSearch.StopSearch();
        }

        #endregion

        #region Interface Handler

        private void OnSearchFilter(object sender, RoutedEventArgs e)
        {
            if (sender == btSearchAll)
                SearchType = SearchFilterType.SearchAll;
            else if (sender == btSearchEvent)
                SearchType = SearchFilterType.SearchEvent;
            else
                SearchType = SearchFilterType.SearchCamera;
        }

        private void OnCancelSearch(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
        }

        protected void OnSearchTextChange(object sender, TextChangedEventArgs e)
        {
            SearchService.VideoSearch.SearchText = SearchBox.Text;

            ChangPlaceHolderVisible(false);

            if (SearchBox.Text.Length > 0)
            {
                textPlaceHolder.Visibility = Visibility.Hidden;
                btnCancel.Visibility = Visibility.Visible;

                ChangeWidowHeight(532 + mainFrame.ShadowSize * 2);

                StartSearchTimer();
            }
            else
            {
                textPlaceHolder.Visibility = Visibility.Visible;
                btnCancel.Visibility = Visibility.Hidden;

                ChangeWidowHeight(110 + mainFrame.ShadowSize * 2);

                SearchService.VideoSearch.StopSearch();
            }
        }

        #endregion

        #region Utility

        protected void StartSearchTimer()
        {
            ResetSearchTimer();
            _searchText = SearchBox.Text;
            TimerCallback searchCallback = StartSearch;
            _searchTimer = new Timer(searchCallback, null, 1000, 10000);
        }

        protected void ResetSearchTimer()
        {
            if (_searchTimer != null)
            {
                _searchTimer.Dispose();
                _searchTimer = null;
            }
        }

        protected void StartSearch(Object stateInfo)
        {
            SearchBox.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new GetSearchTextDelegate(UpdateSearchText));

            SearchService.VideoSearch.SearchText = _searchText;
            SearchService.VideoSearch.StartSearch();

            ResetSearchTimer();
        }

        protected void UpdateSearchFilter()
        {
            if (SearchType == SearchFilterType.SearchAll)
            {
                SearchAllSelector.Visibility = Visibility.Visible;
                SearchEventSelector.Visibility = Visibility.Hidden;
                SearchCameraSelector.Visibility = Visibility.Hidden;
            }
            else if (SearchType == SearchFilterType.SearchEvent)
            {
                SearchAllSelector.Visibility = Visibility.Hidden;
                SearchEventSelector.Visibility = Visibility.Visible;
                SearchCameraSelector.Visibility = Visibility.Hidden;
            }
            else
            {
                SearchAllSelector.Visibility = Visibility.Hidden;
                SearchEventSelector.Visibility = Visibility.Hidden;
                SearchCameraSelector.Visibility = Visibility.Visible;
            }
            SearchService.VideoSearch.Filter = SearchType;
            SearchService.VideoSearch.StartSearch();
        }

        private void ChangeWidowHeight(double newHeight, double duration = 0.2)
        {
            if (!IsLoaded)
                return;

            DoubleAnimation changeHeightAnimation = new DoubleAnimation();
            changeHeightAnimation.From = Height;
            changeHeightAnimation.To = newHeight;
            changeHeightAnimation.Duration = new Duration(TimeSpan.FromSeconds(duration));
            changeHeightAnimation.AutoReverse = false;

            BeginAnimation(HeightProperty, changeHeightAnimation);
        }
        #endregion

        #region Handler
        private void OnSelectedItem(object sender, SelectionChangedEventArgs e)
        {
        }
        #endregion

        #region Delegate
        private void SearchStarted()
        {
            SearchResultList.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new ClearResultDelegate(ClearResult));
        }

        private void SearchCompleted(int count)
        {
            if(count == 0)
                PlaceHolder.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new PlaceHolderVisibleChangDelegate(ChangPlaceHolderVisible), true);
        }

        private void FoundNewResult(FormattedText result)
        {
            SearchResultList.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new AppendResultDelegate(AppendResult), result);
            PlaceHolder.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new PlaceHolderVisibleChangDelegate(ChangPlaceHolderVisible), false);
        }

        private void ClearResult()
        {
            SearchResult.ClearResult();
        }

        private void AppendResult(FormattedText result)
        {
            SearchResult.AddResult(result);
        }

        private void ChangPlaceHolderVisible(bool isVisible)
        {
            if(PlaceHolder != null)
                PlaceHolder.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
        }

        private void UpdateSearchText()
        {
            if (SearchBox != null)
                _searchText = SearchBox.Text;
        }

        #endregion

        private void OnListDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left && SearchResultList.SelectedIndex >= 0)
            {
                object hitItem = SearchResultList.InputHitTest(e.GetPosition(SearchResultList));

                if(hitItem != null && hitItem.GetType() == typeof(SearchListViewItemContent))
                {
                    Hide();
                    SearchService.VideoSearch.StopSearch();

                    SearchService.VideoSearch.SelectResult(SearchResultList.SelectedIndex);
                }
            }
        }
    }
}
