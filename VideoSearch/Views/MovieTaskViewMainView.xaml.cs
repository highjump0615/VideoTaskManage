using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using VideoSearch.Model;
using VideoSearch.Utils;
using VideoSearch.ViewModel;
using VideoSearch.Views.PlayView;
using vlcPlayerLib;

namespace VideoSearch.Views
{
    public delegate void UpdateDurationDelegate(long pos);

    public partial class MovieTaskViewMainView : PlayerViewBase
    {
        private ManualMarkUtils _markUtils = null;
        private long _curPos = 0;

        public MovieTaskViewMainView()
        {
            InitializeComponent();

            OnStop(this, null);

            Unloaded += OnUnLoad;
        }

        #region Delegate
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// 播放器初始化
        /// </summary>
        public void InitPlayer()
        {
            _vlcPlayer = new vlcPlayer();
            _vlcPlayer.SetIntiTimeInfo(false);
            _vlcPlayer.SetControlPanelTimer(false);
            _vlcPlayer.SetManualMarkMode(true);

            _vlcPlayer.VideoDurationChanged += OnMovieDurationChanged;
            _vlcPlayer.VideoPositionChanged += OnMoviePosChanged;
            _vlcPlayer.PlayerStopped += OnMovieStopped;
            _vlcPlayer.ManualMarkAdded += ManualMarkAdded;

            PlayerPanel.Child = _vlcPlayer;

            var taskInit = InitPlayerAsync();

            Reset();
        }

        private async Task InitPlayerAsync()
        {
            // 显示加载中提示
            Globals.Instance.ShowWaitCursor(true);

            // 重新加载需要延迟
            await Task.Delay(20);

            var vm = (MovieTaskViewMainModel)this.DataContext;

            // 是否已获取
            if (!vm.movieItem.IsFetched())
            {
                await vm.movieItem.InitFromServer(true);
            }

            if (!String.IsNullOrEmpty(vm.movieItem.PlayPath))
            {
                _vlcPlayer.SetVideoInfo(vm.movieItem.PlayPath, true);
                _markUtils = new ManualMarkUtils(_vlcPlayer, vm.movieItem.VideoId);

                OnPlay(this, null);
            }
            else
            {
                OnStop(this, null);
                PlayButton.IsEnabled = false;
            }

            Globals.Instance.ShowWaitCursor(false);
        }

        protected new void onDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            base.onDataContextChanged(sender, e);

            // 初始化播放器
            ClearPlayer();

            if (e.NewValue is MovieTaskViewMainModel)
            {
                // 初始化
                InitPlayer();
                controlEffect.initEffect(this);
            }            
        }

        public void OnUnLoad(object sender, RoutedEventArgs e)
        {
            Unloaded -= OnUnLoad;

            ClearPlayer();
            _vlcPlayer = null;
        }

        /// <summary>
        /// 清除播放器
        /// </summary>
        public void ClearPlayer()
        {
            OnStop(this, null);
        }
        #endregion

        #region VLC
        public void ManualMarkAdded(Object sender, Rectangle manualMarkRect, long frame, Bitmap bitmap)
        {
            if(_markUtils.AddManualMark(manualMarkRect, frame))
            {
                ObservableCollection<ClipInfo> items = (ObservableCollection<ClipInfo>)PathDataGrid.ItemsSource;

                ClipInfo clip = new ClipInfo(frame, manualMarkRect.X, manualMarkRect.Y, manualMarkRect.Right, manualMarkRect.Bottom);
                items.Add(clip);

                btnSave.IsEnabled = true;
                btnClear.IsEnabled = true;
            }
        }

        #endregion

        #region Utility
        public void StopMovie()
        {
            if (_vlcPlayer.IsPlaying())
                _vlcPlayer.Stop();
        }


        private void ShowPlayer(bool isShow)
        {
            PlayerPanel.Visibility = isShow ? Visibility.Visible : Visibility.Hidden;

            PlayButton.IsEnabled = !isShow;
            PauseButton.IsEnabled = isShow;
            StopButton.IsEnabled = isShow;

            GotoBeginButton.IsEnabled = isShow;
            GotoEndButton.IsEnabled = isShow;
            SpeedUpButton.IsEnabled = isShow;
            SpeedDownButton.IsEnabled = isShow;

            DurationSlider.IsEnabled = isShow;

            if(!isShow)
            {
                DurationSlider.Minimum = 0;
                DurationSlider.Maximum = 1;
                DurationSlider.Value = 0;
            }
        }

        #endregion

        #region Buttons Handler
        private void OnPause(object sender, RoutedEventArgs e)
        {
            _vlcPlayer.Pause();

            PlayButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
        }

        private void OnStop(object sender, RoutedEventArgs e)
        {
            if (_vlcPlayer != null)
                _vlcPlayer.Stop();

            DurationSlider.Value = 0;
            DurationSlider.IsEnabled = false;

            ShowPlayer(false);
        }

        private void OnPlay(object sender, RoutedEventArgs e)
        {
            ShowPlayer(true);

            if (sender != null)
            {
                _vlcPlayer.Play();
            }
        }

        private void OnGotoBegin(object sender, RoutedEventArgs e)
        {
            if (_vlcPlayer.VideoTime > 0)
            {
                _vlcPlayer.SetPlayerPositionForOuterControl(0);
                 OnPlay(null, null);
            }
        }

        private void OnGotoEnd(object sender, RoutedEventArgs e)
        {
            if (_vlcPlayer.VideoTime > 0)
            {
                _vlcPlayer.SetPlayerPositionForOuterControl(_vlcPlayer.VideoTime);
                 OnPlay(null, null);
            }
        }

        private void OnSpeedUp(object sender, RoutedEventArgs e)
        {
            if (_vlcPlayer.VideoTime > 0)
            {
                _vlcPlayer.Accelarete();
                OnPlay(null, null);
            }
        }

        private void OnSpeedDown(object sender, RoutedEventArgs e)
        {
            if (_vlcPlayer.VideoTime > 0)
            {
                _vlcPlayer.Slow();
                OnPlay(null, null);
            }
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            StopMovie();
        }
        #endregion

        #region Media Hanlder

        protected void UpdateDuration(long pos)
        {
            _curPos = pos;
            DurationSlider.Value = pos;
        }

        private void OnMoviePosChanged(object sender, long pos)
        {
            DurationSlider.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new UpdateDurationDelegate(UpdateDuration), pos);
        }

        private void OnMovieDurationChanged(object sender, long duration)
        {
            TimeMarker.Duration = new TimeSpan(duration);

            DurationSlider.IsEnabled = true;
            DurationSlider.SmallChange = 500;
            DurationSlider.LargeChange = 5000;
            DurationSlider.Minimum = 0;
            DurationSlider.Maximum = duration;
        }

        private void OnMovieStopped(object sender, EventArgs e)
        {
            DurationSlider.Value = DurationSlider.Maximum;
            OnStop(sender, null);
        }
        #endregion

        private void OnDurationChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_vlcPlayer != null && _vlcPlayer.VideoTime > 0)
            {
                if(DurationSlider.Value != _curPos)
                    _vlcPlayer.SetPlayerPositionForOuterControl((long)DurationSlider.Value);
            }
        }

        /// <summary>
        /// 清空标注
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClear(object sender, RoutedEventArgs e)
        {
            if (!IsEditedDetail())
            {
                if (MessageBox.Show("未保存编辑内容，是否继续?", "清空", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    Reset();

                return;
            }
            else
            {
                Reset();
            }
        }

        /// <summary>
        /// 保存标注
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSave(object sender, RoutedEventArgs e)
        {
            _markUtils.SaveManualMark();

            //
            // 收集标注相关信息
            //
            DetailInfo markInfo = new DetailInfo();
            try
            {                
                // 轨迹说明
                markInfo.desc = TxtDetail.Text;
                // 关键词
                markInfo.keyword = TxtMainKey.Text;
                // 目标类型
                markInfo.type = CboTargetType.SelectedIndex;

                // 裤子颜色
                // 裤子种类
                markInfo.pantsKind = CboPantsKind.Text;
                // 其它体貌特征
                markInfo.otherHumanSpec = TxtOtherHumanSpec.Text;
                // 上衣颜色
                // 上衣种类
                markInfo.coatKind = TxtCoatKind.Text;
                // 是否背包
                markInfo.hasPack = Convert.ToInt32(ChkHasPack.IsChecked);
                // 是否戴帽子
                markInfo.hasCap = Convert.ToInt32(ChkHasCap.IsChecked);
                // 是否戴眼镜
                markInfo.hasGlass = Convert.ToInt32(ChkHasGlass.IsChecked);
                // 姓名
                markInfo.name = TxtName.Text;

                // 车牌
                markInfo.carNumber = TxtCarNumber.Text;
                // 车身颜色
                // 乘客数量
                markInfo.memberCount = Convert.ToInt32(TxtMemberCount.Text);
                // 驾驶人
                markInfo.driver = TxtDriver.Text;
                // 品牌型号
                markInfo.carModel = TxtCarModel.Text;
                // 其他车身特征
                markInfo.otherCarSpec = TxtOtherCarSpec.Text;
            }
            catch (Exception)
            {
            }


            // 保存到数据库
            var vm = (MovieTaskViewMainModel)this.DataContext;
            vm.saveMarkInfo(_markUtils.getMarkAt(0), markInfo);



            Reset();
        }

        private bool IsEditedDetail()
        {
            System.Windows.Media.Brush transBr = System.Windows.Media.Brushes.Transparent;

            // 目标类型：人
            if(CboTargetType.SelectedIndex == 0)
            {
                // 裤子颜色
                if (PantsColor.Background != transBr)
                    return true;

                // 裤子种类
                if (CboPantsKind.SelectedIndex > -1)
                    return true;

                // 其它体貌特征
                if (TxtOtherHumanSpec.Text.Length > 0)
                    return true;

                // 上衣颜色
                if (CoatColor.Background != transBr)
                    return true;

                // 上衣种类
                if (TxtCoatKind.Text.Length > 0)
                    return true;

                // 是否背包
                if (ChkHasPack.IsChecked == true ||
                    ChkHasCap.IsChecked == true ||
                    ChkHasGlass.IsChecked == true)
                    return true;

                // 姓名
                if (TxtName.Text.Length > 0)
                    return true;
            }
            // 目标类型：车
            else if (CboTargetType.SelectedIndex == 1)
            {
                // 车牌
                if (TxtCarNumber.Text.Length > 0)
                    return true;

                // 车身颜色
                if (CarColor.Background != transBr)
                    return true;

                // 乘客数量
                if (TxtMemberCount.Text.Length > 0)
                    return true;

                // 驾驶人
                if (TxtDriver.Text.Length > 0)
                    return true;

                // 品牌型号
                if (TxtCarModel.Text.Length > 0)
                    return true;

                // 其他车身特征
                if (TxtOtherCarSpec.Text.Length > 0)
                    return true;
            }

            // 轨迹说明
            if (TxtDetail.Text.Length > 0)
                return true;

            // 关键词
            if (TxtMainKey.Text.Length > 0)
                return true;

            return false;
        }

        private void Reset()
        {
            ObservableCollection<ClipInfo> items = (ObservableCollection<ClipInfo>)PathDataGrid.ItemsSource;
            if (items != null)
            {
                items.Clear();
            }

            if (_markUtils != null)
            {
                _markUtils.ResetManualMark();
            }

            btnSave.IsEnabled = false;
            btnClear.IsEnabled = false;

            //
            // 清空标注信息详情
            //
            System.Windows.Media.Brush transBr = System.Windows.Media.Brushes.Transparent;
            TxtDetail.Text = "";
            TxtMainKey.Text = "";
            CboTargetType.SelectedIndex = -1;
            PantsColor.Background = transBr;
            CboPantsKind.SelectedIndex = -1;
            TxtOtherHumanSpec.Text = "";
            CoatColor.Background = transBr;
            TxtCoatKind.Text = "";
            ChkHasPack.IsChecked = false;
            ChkHasCap.IsChecked = false;
            ChkHasGlass.IsChecked = false;
            TxtName.Text = "";

            TxtCarNumber.Text = "";
            CarColor.Background = transBr;
            TxtMemberCount.Text = "";
            TxtDriver.Text = "";
            TxtCarModel.Text = "";
            TxtOtherCarSpec.Text = "";

            GridManInfo.Visibility = Visibility.Collapsed;
            GridCarInfo.Visibility = Visibility.Collapsed;

            if (_vlcPlayer != null)
            {
                _vlcPlayer.ClearAllManualMark();
            }
        }

        private void OnSelectColor(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.ColorDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // need to convert from the ColorDialog GDI colorspace to the WPF colorspace
                var wpfColor = System.Windows.Media.Color.FromArgb(dialog.Color.A, dialog.Color.R, dialog.Color.G, dialog.Color.B);

                var brush = new SolidColorBrush(wpfColor);

                if (sender == PantsColorBtn)
                    this.PantsColor.Background = brush;
                else if (sender == CoatColorBtn)
                    this.CoatColor.Background = brush;
                else if (sender == CarColorBtn)
                    this.CarColor.Background = brush;
            }
        }

        private void OnTargetTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            GridManInfo.Visibility = Visibility.Collapsed;
            GridCarInfo.Visibility = Visibility.Collapsed;

            if (CboTargetType.SelectedIndex == 0)
            {
                GridManInfo.Visibility = Visibility.Visible;
            }
            else if (CboTargetType.SelectedIndex == 1)
            {
                GridCarInfo.Visibility = Visibility.Visible;
            }
        }
    }

}
