using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using VideoSearch.ViewModel;
using vlcPlayerLib;

namespace VideoSearch.Views
{
    public partial class PanelViewTaskCompressView : UserControl
    {
        private vlcPlayer _vlcPlayer = null;
        private long _curPos = 0;

        public PanelViewTaskCompressView()
        {
            InitializeComponent();

            Unloaded += OnUnLoad;

        }

        public void OnUnLoad(object sender, RoutedEventArgs e)
        {
            Unloaded -= OnUnLoad;
            _vlcPlayer.Stop();
        }

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

            if (!isShow)
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

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ////string moviepath = moviesource.text;
            ////if (moviepath != null && moviepath.length > 0)
            ////{
            ////    var task = initplayerasync();
            ////    //_vlcplayer.setvideoinfo(moviepath, true);

            ////    //// 浓缩物体显示
            ////    ////list<string> listpath = new list<string>();
            ////    ////listpath.add(@"e:\work\project\video\test\1.xml");
            ////    ////vlcplayer1.setwushibiaoinfo(true, listpath);

            ////    //playerpanel.child = _vlcplayer;

            ////    //onplay(sender, e);
            ////}
            ////else
            ////{
            ////    onstop(this, null);
            ////    playbutton.isenabled = false;
            ////}
        }

        /// <summary>
        /// 初始化视频信息
        /// </summary>
        /// <returns></returns>
        private async Task InitPlayerAsync()
        {
            // 显示加载中提示
            Globals.Instance.ShowWaitCursor(true);

            var vm = (PanelViewTaskCompressModel)this.DataContext;
            if (!String.IsNullOrEmpty(vm.taskItem.CompressedPlayPath))
            {

                // 重新加载需要延迟
                await Task.Delay(20);

                _vlcPlayer.SetVideoInfo(vm.taskItem.CompressedPlayPath, true);
                OnPlay(this, null);
            }
            else {
                OnStop(this, null);
                PlayButton.IsEnabled = false;
            }

            Globals.Instance.ShowWaitCursor(false);
        }

        protected void onDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is PanelViewTaskCompressModel)
            {
                var vm = (PanelViewTaskCompressModel)e.NewValue;
                vm.View = this;

                var task = vm.InitTaskResult();
            }
        }

        /// <summary>
        /// 播放器初始化
        /// </summary>
        public void InitPlayer()
        {
            // 初始化播放器
            ClearPlayer();

            if (_vlcPlayer == null)
            {
                _vlcPlayer = new vlcPlayer();
                _vlcPlayer.SetIntiTimeInfo(false);
                _vlcPlayer.SetControlPanelTimer(false);

                _vlcPlayer.VideoDurationChanged += OnMovieDurationChanged;
                _vlcPlayer.VideoPositionChanged += OnMoviePosChanged;
                _vlcPlayer.PlayerStopped += OnMovieStopped;

                PlayerPanel.Child = _vlcPlayer;
            }

            var taskInit = InitPlayerAsync();
        }

        /// <summary>
        /// 清除播放器
        /// </summary>
        public void ClearPlayer()
        {
            OnStop(this, null);
        }

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
            if (_vlcPlayer.VideoTime > 0)
            {
                if (_vlcPlayer.VideoTime > 0)
                {
                    if (DurationSlider.Value != _curPos)
                        _vlcPlayer.SetPlayerPositionForOuterControl((long)DurationSlider.Value);
                }
            }
        }

        private void OnClear(object sender, RoutedEventArgs e)
        {

        }

        private void OnSave(object sender, RoutedEventArgs e)
        {

        }
    }
}
