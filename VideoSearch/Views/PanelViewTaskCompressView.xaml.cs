using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
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

            _vlcPlayer = new vlcPlayer();
            _vlcPlayer.SetIntiTimeInfo(false);
            _vlcPlayer.SetControlPanelTimer(false);
            _vlcPlayer.SetManualMarkMode(true);

            _vlcPlayer.VideoDurationChanged += OnMovieDurationChanged;
            _vlcPlayer.VideoPositionChanged += OnMoviePosChanged;
            _vlcPlayer.PlayerStopped += OnMovieStopped;

            OnStop(this, null);

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

            if (sender != null && e != null)
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
            String moviePath = MovieSource.Text;
            if (moviePath != null && moviePath.Length > 0)
            {
                _vlcPlayer.SetVideoInfo(moviePath, true);

                PlayerPanel.Child = _vlcPlayer;

                OnPlay(sender, e);
            }
            else
            {
                OnStop(this, null);
                PlayButton.IsEnabled = false;
            }
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
