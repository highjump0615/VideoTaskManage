﻿using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using VideoSearch.Utils;
using VideoSearch.ViewModel;
using vlcPlayerLib;

namespace VideoSearch.Views
{
    public delegate void UpdateDurationDelegate(long pos);

    public partial class MovieTaskViewMainView : UserControlBase
    {
        private vlcPlayer _vlcPlayer = null;
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
            if (_vlcPlayer == null)
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
            }

            var taskInit = InitPlayerAsync();

            Reset();
        }

        private async Task InitPlayerAsync()
        {
            await Task.Delay(10);

            var vm = (MovieTaskViewMainModel)this.DataContext;

            if (!String.IsNullOrEmpty(vm.MoviePath))
            {
                _vlcPlayer.SetVideoInfo(vm.MoviePath, true);
                _markUtils = new ManualMarkUtils(_vlcPlayer, vm.MovieID);

                OnPlay(this, null);
            }
            else
            {
                OnStop(this, null);
                PlayButton.IsEnabled = false;
            }
        }

        protected new void onDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            base.onDataContextChanged(sender, e);

            // 初始化播放器
            ClearPlayer();
            InitPlayer();
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

        private void OnClear(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            if(!IsEditedDetail())
            {
                if (MessageBox.Show("未保存编辑内容，是否继续?", "保存", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    Reset();

                return;
            }

            _markUtils.SaveManualMark();
            Reset();
        }

        private bool IsEditedDetail()
        {
            System.Windows.Media.Brush transBr = System.Windows.Media.Brushes.Transparent;

            if(CboTargetType.SelectedIndex == 0)
            {
                if (PantsColor.Background != transBr)
                    return true;

                if (CboPantsKind.SelectedIndex > -1)
                    return true;

                if (TxtOtherHumanSpec.Text.Length > 0)
                    return true;

                if (CoatColor.Background != transBr)
                    return true;

                if (TxtCoatKind.Text.Length > 0)
                    return true;

                if (ChkHasPack.IsChecked == true ||
                    ChkHasCap.IsChecked == true ||
                    ChkHasGlass.IsChecked == true)
                    return true;

                if (TxtName.Text.Length > 0)
                    return true;
            }
            else if (CboTargetType.SelectedIndex == 1)
            {
                if (TxtCarNumber.Text.Length > 0)
                    return true;

                if (CarColor.Background != transBr)
                    return true;

                if (TxtMemberCount.Text.Length > 0)
                    return true;

                if (TxtDriver.Text.Length > 0)
                    return true;

                if (TxtCarModel.Text.Length > 0)
                    return true;

                if (TxtOtherCarSpec.Text.Length > 0)
                    return true;
            }

            if (TxtDetail.Text.Length > 0)
                return true;

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
