using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using vlcPlayerLib;

namespace VideoSearch.Views.PlayView
{
    public class PlayerViewBase : UserControlBase
    {
        protected vlcPlayer _vlcPlayer = null;
        protected AxEventTrackBarXLib.AxEventTrackBarX _trackBar;

        private long mlDuration;

        /// <summary>
        /// 应用播放效果
        /// </summary>
        /// <param name="index"></param>
        public void playWithEffect(int index)
        {
            if (_vlcPlayer == null)
            {
                return;
            }

            switch (index)
            {
                // 去雾
                case 0:
                    _vlcPlayer.SetDefog();
                    break;

                // 对比度增强
                case 1:
                    _vlcPlayer.SetContrast();
                    break;

                // 直方图均衡
                case 2:
                    _vlcPlayer.SetHist();
                    break;

                // 锐化
                case 3:
                    _vlcPlayer.SetSharp();
                    break;

                // 平滑
                case 4:
                    _vlcPlayer.SetSmooth();
                    break;

                // 关闭
                case 5:
                    _vlcPlayer.ClearFilter();
                    break;
            }

        }

        public virtual void InitPlayer()
        {
            if (_vlcPlayer == null)
            {
                _vlcPlayer = new vlcPlayer();
                _vlcPlayer.SetIntiTimeInfo(false);
                _vlcPlayer.SetControlPanelTimer(false);

                _vlcPlayer.VideoDurationChanged += OnMovieDurationChanged;
                _vlcPlayer.VideoPositionChanged += OnMoviePosChanged;
                _vlcPlayer.PlayerStopped += OnMovieStopped;

                // init track bar
                _trackBar = new AxEventTrackBarXLib.AxEventTrackBarX();
                _trackBar.FirePlayTicChange += new AxEventTrackBarXLib._DEventTrackBarXEvents_FirePlayTicChangeEventHandler(OnTrackBarFirePlayTicChange);
            }            
        }

        protected void initTrackBar(long duration = -1)
        {
            if (_trackBar == null)
            {
                return;
            }

            if (duration > 0)
            {
                mlDuration = duration;
            }

            _trackBar.SetFrameCount(UInt32.Parse(mlDuration.ToString()));
        }

        private void OnTrackBarFirePlayTicChange(object sender, AxEventTrackBarXLib._DEventTrackBarXEvents_FirePlayTicChangeEvent e)
        {
            _vlcPlayer.SetPlayerPositionForOuterControl(e.nPlayTicPosition);
        }

        private void OnMovieDurationChanged(object sender, long duration)
        {
            //TimeMarker.Duration = TimeSpan.FromMilliseconds(duration);

            //DurationSlider.IsEnabled = true;
            //DurationSlider.SmallChange = 500;
            //DurationSlider.LargeChange = 5000;
            //DurationSlider.Minimum = 0;
            //DurationSlider.Maximum = duration;

            // track bar
            initTrackBar(duration);
        }

        protected void OnMoviePosChanged(object sender, long pos)
        {
            _trackBar.SetPlayPosition(UInt32.Parse(pos.ToString()));

            //DurationSlider.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new UpdateDurationDelegate(UpdateDuration), pos);
        }

        protected void OnStop(object sender, RoutedEventArgs e)
        {
            if (_vlcPlayer != null)
                _vlcPlayer.Stop();

            //DurationSlider.Value = 0;
            //DurationSlider.IsEnabled = false;

            ShowPlayer(false);
        }

        protected void OnMovieStopped(object sender, EventArgs e)
        {
            //DurationSlider.Value = DurationSlider.Maximum;
            OnStop(sender, null);
        }

        protected virtual void ShowPlayer(bool isShow)
        {
        }

        /// <summary>
        /// 截图按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void onButScreenshot(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = "";
                if (_vlcPlayer == null)
                {
                    return;
                }
                name = "视频截图" + DateTime.Now.ToString("yyyyMMddhhmmss");
                System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog
                {
                    Description = "选择截屏图片存放位置"
                };

                var result = fbd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK ||
                    result == System.Windows.Forms.DialogResult.Yes)
                {
                    string path = fbd.SelectedPath;
                    _vlcPlayer.SnapShot(path + "\\" + name + ".bmp");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 放大镜按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void onButMagnifier(object sender, RoutedEventArgs e)
        {
            if (_vlcPlayer == null)
            {
                return;
            }

            //结束放大
            if (_vlcPlayer.IsLocalEnlarged())
            {
                _vlcPlayer.SetLocalEnlarged(false);
            }
            //开始放大
            else
            {
                _vlcPlayer.SetLocalEnlarged(true);
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Console.WriteLine("MovieTaskViewMainView --- OnSizeChanged");

            initTrackBar();
        }
    }
}
