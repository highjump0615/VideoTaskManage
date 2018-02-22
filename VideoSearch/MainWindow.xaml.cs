using System;
using System.Windows;
using VideoSearch.Windows;
using VideoSearch.Model;
using VideoSearch.ViewModel;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using VideoSearch.SkinControl;
using System.Windows.Input;
using System.Windows.Shell;

namespace VideoSearch
{
    public partial class MainWindow : Window
    {
        private int _level = 0;
        private DataItemBase _selectedItem = new DataItemBase();

        private SearchWindow m_searchWindow = null;

        private ObservableCollection<Control> _groupList = null;

        private static MainWindow _mainWindow = null;
        public static MainWindow VideoSearchMainWindow
        {
            get { return _mainWindow; }
        }

        public MainWindow()
        {
            InitializeComponent();
            _mainWindow = this;
        }
        
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            InitControls();
 
            SelectRoot();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            if (m_searchWindow != null)
            {
                m_searchWindow.Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            VideoData.AppVideoData.Dispose();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            Rect bounds = new Rect(0, 0, ActualWidth, 25);

            if(bounds.Contains(e.GetPosition(this)))
                DragMove();
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            Rect bounds = new Rect(0, 0, ActualWidth, 25);

            if (bounds.Contains(e.GetPosition(this)))
                OnRestore(this, null);
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            if (WindowState == WindowState.Maximized)
                btRestore.IsAltState = true;
            else if (WindowState == WindowState.Normal)
                btRestore.IsAltState = false;
        }
        #region SystemButton
        /////////////////////////////////////////////////////////
        // Process system buttons
        /////////////////////////////////////////////////////////
        private void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnRestore(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                Style = (Style)(this.Resources["ShadowNormalWindowStyle"]);
                WindowState = WindowState.Normal;
            }
            else
            {
                Style = (Style)(this.Resources["ShadowMaximizedWindowStyle"]);
                WindowState = WindowState.Maximized;
            }
        }

        private void OnMinimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        #endregion

        /////////////////////////////////////////////////////////
        // Process command from TreeView
        /////////////////////////////////////////////////////////

        private void UpdateContentsWithSelectedItem(DataItemBase selectedItem)
        {
            if (_selectedItem != selectedItem)
            {
                _selectedItem = selectedItem;

                int level = _selectedItem == null ? 0 : _selectedItem.Level;
                if (level == 0)
                    workView.Content = new EventViewModel(VideoData.AppVideoData);
                else if (level == 1)
                    workView.Content = new CameraViewModel(_selectedItem);
                else if (level == 2)
                    workView.Content = new MovieViewModel(_selectedItem);
                else if (level == 3)
                    workView.Content = new MovieTaskViewModel(_selectedItem);
                else if (level == 4)
                    workView.Content = new PanelViewModel(_selectedItem);
            }
        }

        private void OnTreeItemSelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DataItemBase selectedItem = (DataItemBase)treeView.SelectedItem;

            if (selectedItem != null)
                SelectTabWithLevel(selectedItem.Level);
            else
                SelectRoot();

            UpdateContentsWithSelectedItem(selectedItem);
        }

        /////////////////////////////////////////////////////////
        // utility fuctions...
        /////////////////////////////////////////////////////////
        
        private void InitControls()
        {
            _groupList = new ObservableCollection<Control>();

            _groupList.Add(EventGroup);
            _groupList.Add(CameraGroup);
            _groupList.Add(MovieGroup);
            _groupList.Add(MovieTaskGroup);
            _groupList.Add(PanelGroup);

            EventGroup.Children.Add(ToolbarNewEvent);
            EventGroup.Children.Add(ToolbarDeleteEvent);
            EventGroup.Children.Add(ToolbarSearchEvent);

            CameraGroup.Children.Add(ToolbarNewCamera);
            CameraGroup.Children.Add(ToolbarDeleteCamera);
            CameraGroup.Children.Add(ToolbarShowCameraMap);
            CameraGroup.Children.Add(ToolbarShowCameraList);

            MovieGroup.Children.Add(ToolbarMovieImport);
            MovieGroup.Children.Add(ToolbarMovieDelete);
            MovieGroup.Children.Add(ToolbarMovieFind);
            MovieGroup.Children.Add(ToolbarMoviePlay);
            MovieGroup.Children.Add(ToolbarMovieShowList);

            MovieTaskGroup.Children.Add(ToolbarMovieTaskSearch);
            MovieTaskGroup.Children.Add(ToolbarMovieTaskSummary);
            MovieTaskGroup.Children.Add(ToolbarMovieTaskCompress);
            MovieTaskGroup.Children.Add(ToolbarMovieTaskFind);
            MovieTaskGroup.Children.Add(ToolbarMovieTaskChargeList);
            MovieTaskGroup.Children.Add(ToolbarMovieTaskFindAndPlay);

            PanelGroup.Children.Add(ToolbarPanelPreview);
            PanelGroup.Children.Add(ToolbarPanelShowPath);
            PanelGroup.Children.Add(ToolbarPanelExport);
        }

        private void SelectTabWithLevel(int level)
        {
            for (int i = 0; i < 5; i++)
            {
                SkinButtonGroup control = (SkinButtonGroup)_groupList[i];
                if (i <= level)
                    control.IsEnabled = true;
                else
                    control.IsEnabled = false;

                if (i == level)
                    control.IsSelected = true;
                else
                    control.IsSelected = false;
            }

            Level = level+1;
        }

        protected void SelectRoot()
        {
            for (int i = 0; i < 5; i++)
            {
                SkinButtonGroup control = (SkinButtonGroup)_groupList[i];

                if (i == 0)
                {
                    control.IsEnabled = true;
                    control.IsSelected = true;
                }
                else
                {
                    control.IsEnabled = false;
                    control.IsSelected = false;
                }
            }

            Level = 1;

            UpdateContentsWithSelectedItem(null);
        }

        protected bool SelectParentItem(int level, Object item)
        {
            if (level == 0)
            {
                UpdateContentsWithSelectedItem(null);
                return false;
            }

            DataItemBase dataItem = (DataItemBase)item;

            if(dataItem != null)
            {
                if (dataItem.Level == level)
                {
                    dataItem.IsSelected = true;
                    UpdateContentsWithSelectedItem(dataItem);

                    return true;
                }
                return SelectParentItem(level, dataItem.Parent);
            }

            return false;
        }

        protected void OnTabChanged(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                Decimal senderTag = Decimal.Parse(String.Concat(((FrameworkElement)sender).Tag));

                int nTag = Decimal.ToInt32(senderTag);

                DataItemBase selectedItem = (DataItemBase)treeView.SelectedItem;
                if (!SelectParentItem(nTag - 1, selectedItem))
                {
                    if(selectedItem != null)
                        selectedItem.IsSelected = false;
                    SelectRoot();
                }
            }
        }

        public int Level
        {
            get { return _level; }
            set
            {
                if(_level != value)
                {
                    _level = value;
                }
            }
        }

        protected void ShowSearchWindow()
        {
            if (m_searchWindow == null)
            {
                m_searchWindow = new SearchWindow();
                m_searchWindow.Closed += OnSearchWindowClosed;
            }

            m_searchWindow.Show();
        }

        private void OnSearchWindowClosed(object sender, EventArgs e)
        {
            m_searchWindow = null;
        }
        /////////////////////////////////////////////////////////
        // Process command from Toolbar1
        /////////////////////////////////////////////////////////

        private void OnNewEvent(object sender, RoutedEventArgs e)
        {
            OnTabChanged(sender, e);

            Object viewContents = workView.Content;
            if (viewContents.GetType() == typeof(EventViewModel))
                ((EventViewModel)viewContents).AddNewItem();
        }

        private void OnDeleteEvent(object sender, RoutedEventArgs e)
        {
            OnTabChanged(sender, e);

            Object viewContents = workView.Content;
            if (viewContents.GetType() == typeof(EventViewModel))
                ((EventViewModel)viewContents).DeleteSelectedItems();
        }

        private void OnSearchEvent(object sender, RoutedEventArgs e)
        {
            OnTabChanged(sender, e);

            ShowSearchWindow();
        }

        /////////////////////////////////////////////////////////
        // Process command from Toolbar2
        /////////////////////////////////////////////////////////

        private void OnNewCamera(object sender, RoutedEventArgs e)
        {
            OnTabChanged(sender, e);

            Object viewContents = workView.Content;
            if (viewContents.GetType() == typeof(CameraViewModel))
                ((CameraViewModel)viewContents).AddNewItem();
        }

        private void OnDeleteCamera(object sender, RoutedEventArgs e)
        {
            OnTabChanged(sender, e);

            Object viewContents = workView.Content; ;
            if (viewContents.GetType() == typeof(CameraViewModel))
                ((CameraViewModel)viewContents).DeleteSelectedItems();
        }

        private void OnShowCameraMap(object sender, RoutedEventArgs e)
        {
            OnTabChanged(sender, e);

            Object viewContents = workView.Content; ;
            if (viewContents.GetType() == typeof(CameraViewModel))
            {
                ((CameraViewModel)viewContents).DeleteSelectedItems();
                ((CameraViewModel)viewContents).ShowCameraMap();
            }
        }

        private void OnShowCameraList(object sender, RoutedEventArgs e)
        {
            OnTabChanged(sender, e);

            Object viewContents = workView.Content; ;
            if (viewContents.GetType() == typeof(CameraViewModel))
            {
                ((CameraViewModel)viewContents).ShowCameraDetailList();
            }
        }

        /////////////////////////////////////////////////////////
        // Process command from Toolbar3
        /////////////////////////////////////////////////////////

        private void OnMovieImport(object sender, RoutedEventArgs e)
        {
            OnTabChanged(sender, e);

            Object viewContents = workView.Content; ;
            if (viewContents.GetType() == typeof(MovieViewModel))
                ((MovieViewModel)viewContents).ImportMovie();
        }

        private void OnMovieDelete(object sender, RoutedEventArgs e)
        {
            OnTabChanged(sender, e);

            Object viewContents = workView.Content; ;
            if (viewContents.GetType() == typeof(MovieViewModel))
            {
                ((MovieViewModel)viewContents).DeleteSelectedMovies();
                ((MovieViewModel)viewContents).ShowMovieList();
            }
        }

        private void OnMovieFind(object sender, RoutedEventArgs e)
        {
            OnTabChanged(sender, e);

            ShowSearchWindow();
        }

        private void OnMoviePlay(object sender, RoutedEventArgs e)
        {
            OnTabChanged(sender, e);

            Object viewContents = workView.Content; ;
            if (viewContents.GetType() == typeof(MovieViewModel))
                ((MovieViewModel)viewContents).PlaySelectedMovies();
        }

        private void OnMovieShowList(object sender, RoutedEventArgs e)
        {
            OnTabChanged(sender, e);

            Object viewContents = workView.Content; ;
            if (viewContents.GetType() == typeof(MovieViewModel))
                ((MovieViewModel)viewContents).ShowMovieList();
        }

        private void OnMovieAutioAnalysis(object sender, RoutedEventArgs e)
        {
            OnTabChanged(sender, e);

            Object viewContents = workView.Content; ;
            if (viewContents.GetType() == typeof(MovieViewModel))
                ((MovieViewModel)viewContents).ShowMovieAnalysis();
        }

        /////////////////////////////////////////////////////////
        // Process command from Toolbar4
        /////////////////////////////////////////////////////////

        private void OnMovieTaskSearch(object sender, RoutedEventArgs e)
        {
            OnTabChanged(sender, e);

            Object viewContents = workView.Content; ;
            if (viewContents.GetType() == typeof(MovieTaskViewModel))
            {
                ((MovieTaskViewModel)viewContents).MovieSearch();
            }
        }

        private void OnMovieTaskOutline(object sender, RoutedEventArgs e)
        {
            OnTabChanged(sender, e);

            Object viewContents = workView.Content; ;
            if (viewContents.GetType() == typeof(MovieTaskViewModel))
            {
                ((MovieTaskViewModel)viewContents).MovieOutline();
            }
        }

        private void OnMovieTaskCompress(object sender, RoutedEventArgs e)
        {
            OnTabChanged(sender, e);

            Object viewContents = workView.Content; ;
            if (viewContents.GetType() == typeof(MovieTaskViewModel))
            {
                ((MovieTaskViewModel)viewContents).MovieCompress();
            }
        }

        private void OnMovieTaskFind(object sender, RoutedEventArgs e)
        {
            OnTabChanged(sender, e);

            ShowSearchWindow();
        }

        private void OnMovieTaskChargeList(object sender, RoutedEventArgs e)
        {
            OnTabChanged(sender, e);

            Object viewContents = workView.Content; ;
            if (viewContents.GetType() == typeof(MovieTaskViewModel))
            {
                ((MovieTaskViewModel)viewContents).ShowMovieChargeList();
            }
        }

        private void OnMovieTaskFindAndPlay(object sender, RoutedEventArgs e)
        {
            OnTabChanged(sender, e);

            Object viewContents = workView.Content; ;
            if (viewContents.GetType() == typeof(MovieTaskViewModel))
            {
                ((MovieTaskViewModel)viewContents).MovieFindAndPlay();
            }
        }

        /////////////////////////////////////////////////////////
        // Process command from Toolbar5
        /////////////////////////////////////////////////////////

        private void OnPanelPreview(object sender, RoutedEventArgs e)
        {
            OnTabChanged(sender, e);

            Object viewContents = workView.Content;
            if (viewContents.GetType() == typeof(PanelViewModel))
                ((PanelViewModel)viewContents).ShowResult();
        }

        private void OnPanelShowPath(object sender, RoutedEventArgs e)
        {
            OnTabChanged(sender, e);

            Object viewContents = workView.Content;
            if (viewContents.GetType() == typeof(PanelViewModel))
                ((PanelViewModel)viewContents).ShowPath();
        }

        private void OnPanelExport(object sender, RoutedEventArgs e)
        {
            OnTabChanged(sender, e);

            Object viewContents = workView.Content;
            if (viewContents.GetType() == typeof(PanelViewModel))
                ((PanelViewModel)viewContents).Export();
        }

        /////////////////////////////////////////////////////////
        // Process command from Toolbar6
        /////////////////////////////////////////////////////////

        private void OnShowHelp(object sender, RoutedEventArgs e)
        {
            HelpWindow helpWindow = new HelpWindow();

            helpWindow.Owner = this;
            helpWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            helpWindow.ShowDialog();
        }
    }
}
