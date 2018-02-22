using System.Windows.Controls;
using System.Windows;
using System;
using System.Windows.Forms.Integration;
using vlcPlayerLib;

namespace VideoSearch.Views.Player
{
    /// <summary>
    /// Interaction logic for MiniPlayerView.xaml
    /// </summary>
    public partial class MiniPlayerView : UserControl
    {
        private vlcPlayer _vlcPlayer = null;

        public MiniPlayerView()
        {
            InitializeComponent();

            _vlcPlayer = new vlcPlayer();
            _vlcPlayer.SetIntiTimeInfo(false);
            _vlcPlayer.SetControlPanelTimer(false);
            _vlcPlayer.SetManualMarkMode(true);

            _vlcPlayer.PlayerStopped += OnMovieStopped;

            PlayerPanel.Visibility = Visibility.Hidden;

            PlayButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
        }

        ~MiniPlayerView()
        {
            _vlcPlayer.Stop();
            _vlcPlayer = null;
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            _vlcPlayer.SetVideoInfo(MovieSource.Text, true);

            PlayerPanel.Child = _vlcPlayer;
        }

        private void OnPause(object sender, System.Windows.RoutedEventArgs e)
        {
            _vlcPlayer.Pause();

            PlayButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
        }

        private void OnPlay(object sender, System.Windows.RoutedEventArgs e)
        {
            if (PlayerPanel.Visibility == Visibility.Hidden)
                PlayerPanel.Visibility = Visibility.Visible;

            _vlcPlayer.Play();

            PlayButton.IsEnabled = false;
            PauseButton.IsEnabled = true;
        }


        #region Media Hanlder
        private void OnMovieStopped(object sender, EventArgs e)
        {
            _vlcPlayer.SetPlayerPositionForOuterControl(0);

            if (PlayerPanel.Visibility == Visibility.Visible)
                PlayerPanel.Visibility = Visibility.Hidden;

            PlayButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
        }
        #endregion
    }
}
