using System;
using System.Windows;

namespace VideoSearch.Windows
{
    public partial class PlayerWindow : Window
    {
        #region static acces
        private static PlayerWindow _player = null;

        public static PlayerWindow Player
        {
            get
            {
                if(_player == null)
                {
                    _player = new PlayerWindow();
                }

                return _player;
            }
        }

        public static void PlayMovie(String name, String path)
        {
            CloseMovie();

            Player.MovieTitle = name;
            Player.MoviePath = path;

            Player.ShowDialog();
        }

        public static void CloseMovie()
        {
            if (_player != null)
                _player.StopMovie();
        }

        #endregion

        #region Property

        private bool _isSkip = false;

        private String _movieTitle = "";
        public String MovieTitle
        {
            get { return _movieTitle; }
            set
            {
                _movieTitle = value;
                MediaTitle.Text = _movieTitle;
            }
        }

        private String _moviePath = "";
        public String MoviePath
        {
            get { return _moviePath; }
            set
            {
                _moviePath = value;
                MediaPlayer.Source = new Uri(_moviePath);
            }
        }

        #endregion

        #region Constructor & Others...

        public PlayerWindow()
        {
            InitializeComponent();

            Owner = MainWindow.VideoSearchMainWindow;

            OnStop(null, null);
        }

        public void StopMovie()
        {
            if (MediaPlayer.HasVideo)
                MediaPlayer.Stop();

            Close();
        }

        private void OnClosed(object sender, EventArgs e)
        {
            _player = null;
        }

        private void ShowPlayer(bool isShow)
        {
            PlayerPanel.Visibility = isShow ? Visibility.Visible : Visibility.Hidden;
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
            MediaPlayer.Close();

            if(_moviePath.Length > 0)
                MediaPlayer.Source = new Uri(_moviePath);

            ShowPlayer(false);

            PlayButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
            StopButton.IsEnabled = false;

            GotoBeginButton.IsEnabled = false;
            GotoEndButton.IsEnabled = false;
            SpeedUpButton.IsEnabled = false;
            SpeedDownButton.IsEnabled = false;
        }

        private void OnPlay(object sender, RoutedEventArgs e)
        {
            ShowPlayer(true);

            MediaPlayer.Play();

            PlayButton.IsEnabled = false;
            PauseButton.IsEnabled = true;
            StopButton.IsEnabled = true;

            GotoBeginButton.IsEnabled = true;
            GotoEndButton.IsEnabled = true;
            SpeedUpButton.IsEnabled = true;
            SpeedDownButton.IsEnabled = true;
        }

        private void OnGotoBegin(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.HasVideo)
            {
                MediaPlayer.Position = TimeSpan.Zero;
                OnPlay(sender, e);
            }
        }

        private void OnGotoEnd(object sender, RoutedEventArgs e)
        {
            if(MediaPlayer.HasVideo)
            {
                _isSkip = true;

                MediaPlayer.Position = new TimeSpan(MediaPlayer.NaturalDuration.TimeSpan.Ticks - 1);
                OnPlay(sender, e);
            }
        }

        private void OnSpeedUp(object sender, RoutedEventArgs e)
        {
            if(MediaPlayer.HasVideo)
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
        
        private void OnEnded(object sender, RoutedEventArgs e)
        {
            if (_isSkip)
            {
                _isSkip = false;
            }
            else
            {
                OnStop(sender, e);
            }
        }
        #endregion
    }
}
