using System;
using System.Windows;
using System.Windows.Input;
using vlcPlayerLib;

namespace VideoSearch.Windows
{
    public partial class PlayerWindow : Window
    {
        #region static acces
        private static PlayerWindow _player = null;
        private vlcPlayer _vlcPlayer = null;

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
            }
        }

        #endregion

        #region Constructor & Others...

        public PlayerWindow()
        {
            InitializeComponent();

            Owner = MainWindow.VideoSearchMainWindow;

            _vlcPlayer = new vlcPlayer();
            _vlcPlayer.SetIntiTimeInfo(false);
            _vlcPlayer.SetControlPanelTimer(false);
            _vlcPlayer.SetManualMarkMode(true);

            _vlcPlayer.PlayerStopped += OnMovieStopped;

            OnStop(null, null);
        }

        public void StopMovie()
        {
            if (_vlcPlayer.IsPlaying())
                _vlcPlayer.Stop();

            Close();
        }
        
        private void OnLoaded(object sender, EventArgs e)
        {
            _vlcPlayer.SetVideoInfo(_moviePath, true);

            PlayerPanel.Child = _vlcPlayer;
        }

        private void OnClosed(object sender, EventArgs e)
        {
            _player = null;
        }

        private void ShowPlayer(bool isShow)
        {
            PlayerPanel.Visibility = isShow ? Visibility.Visible : Visibility.Hidden;
        }


        private void OnMouseLeftDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
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
            _vlcPlayer.Stop();

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
            if(sender != null && e != null)
            {
                ShowPlayer(true);
                _vlcPlayer.Play();
            }


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
        private void OnMovieStopped(object sender, EventArgs e)
        {
            OnStop(sender, null);
        }

        #endregion
    }
}
