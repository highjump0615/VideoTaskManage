using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace VideoSearch.Views
{
    public delegate void UpdateDurationDelegate();

    public partial class MovieTaskViewMainView : UserControl
    {
        private Timer _monitor = null;
        private String _source = null;

        public MovieTaskViewMainView()
        {
            InitializeComponent();

            OnStop(this, null);
        }

        #region Utility
        public void StopMovie()
        {
            if (MediaPlayer.HasVideo)
                MediaPlayer.Stop();
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
            MediaPlayer.Pause();

            PlayButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
        }

        private void OnStop(object sender, RoutedEventArgs e)
        {
            MediaPlayer.SpeedRatio = 1.0;
            MediaPlayer.Position = TimeSpan.Zero;
            MediaPlayer.Close();

            ShowPlayer(false);
        }

        private void OnPlay(object sender, RoutedEventArgs e)
        {
            ShowPlayer(true);

            MediaPlayer.Play();
        }

        private void OnGotoBegin(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.HasVideo)
            {
                MediaPlayer.Position = TimeSpan.Zero;
            }
        }

        private void OnGotoEnd(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.HasVideo)
            {
                MediaPlayer.Position = new TimeSpan(MediaPlayer.NaturalDuration.TimeSpan.Ticks - 1);
            }
        }

        private void OnSpeedUp(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.HasVideo)
            {
                MediaPlayer.SpeedRatio = MediaPlayer.SpeedRatio * 2;
                OnPlay(sender, e);
            }
        }

        private void OnSpeedDown(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.HasVideo)
            {
                MediaPlayer.SpeedRatio = MediaPlayer.SpeedRatio / 2;
                OnPlay(sender, e);
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
            Console.WriteLine("=========== load +++ == : {0}", MediaPlayer.Source);

            if (MediaPlayer.Source != null)
                OnPlay(null, null);
            else
            {
                OnStop(this, null);
                PlayButton.IsEnabled = false;
            }                
        }

        private void OnUnLoaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("=========== unload ==");
            DurationSlider.IsEnabled = false;

//            ResetPlaybackMonitor();
        }


        private void OnOpend(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("=========== OnOpend ==");
            if (MediaPlayer.Source == null)
            {
                OnStop(this, null);
                PlayButton.IsEnabled = false;
                return;
            }

            String newSource = MediaPlayer.Source.ToString();

            if(_source != newSource)
            {
                _source = newSource;

                OnStop(this, null);
                OnPlay(this, null);

                Console.WriteLine("=========== eeeadfa  OnOpend ==");
                return;
            }


            TimeSpan duration = MediaPlayer.NaturalDuration.TimeSpan;
            TimeMarker.Duration = duration;

            DurationSlider.IsEnabled = true;

            DurationSlider.SmallChange = new TimeSpan(0, 0, 0, 1).Ticks;
            DurationSlider.LargeChange = DurationSlider.SmallChange;
            DurationSlider.Minimum = 0;
            DurationSlider.Maximum = duration.Ticks;

            StartPlaybackMonitor();

        }

        private void OnEnded(object sender, RoutedEventArgs e)
        {
            DurationSlider.Value = DurationSlider.Maximum;
        }
        #endregion

        #region Playback Monitor Timer
        protected void StartPlaybackMonitor()
        {
            ResetPlaybackMonitor();
            TimerCallback searchCallback = UpdateMediaPos;
            _monitor = new Timer(searchCallback, null, 0, 1);
        }

        protected void ResetPlaybackMonitor()
        {
            if (_monitor != null)
            {
                _monitor.Dispose();
                _monitor = null;
            }
        }

        protected void UpdateMediaPos(Object stateInfo)
        {
            DurationSlider.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new UpdateDurationDelegate(UpdateDuration));
        }

        protected void UpdateDuration()
        {
            DurationSlider.Value = MediaPlayer.Position.Ticks;
        }
        #endregion

        private void OnDurationChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(MediaPlayer.HasVideo)
            {
                long diff = Math.Abs((long)DurationSlider.Value - MediaPlayer.Position.Ticks);
                if (diff > DurationSlider.LargeChange)
                    OnPause(null, null);
                MediaPlayer.Position = new TimeSpan((long)DurationSlider.Value);
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
