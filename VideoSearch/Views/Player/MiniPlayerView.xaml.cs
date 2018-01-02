using System.Windows.Controls;
using System.Windows;
using System;

namespace VideoSearch.Views.Player
{
    /// <summary>
    /// Interaction logic for MiniPlayerView.xaml
    /// </summary>
    public partial class MiniPlayerView : UserControl
    {
        public MiniPlayerView()
        {
            InitializeComponent();

            PlayerPanel.Visibility = Visibility.Hidden;

            PlayButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
        }

        private void OnPause(object sender, System.Windows.RoutedEventArgs e)
        {
            MediaPlayer.Pause();

            PlayButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
        }

        private void OnPlay(object sender, System.Windows.RoutedEventArgs e)
        {
            if (PlayerPanel.Visibility == Visibility.Hidden)
                PlayerPanel.Visibility = Visibility.Visible;

            MediaPlayer.Play();

            PlayButton.IsEnabled = false;
            PauseButton.IsEnabled = true;
        }

        private void OnEnded(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Position = TimeSpan.Zero;

            if (PlayerPanel.Visibility == Visibility.Visible)
                PlayerPanel.Visibility = Visibility.Hidden;

            PlayButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
        }
    }
}
