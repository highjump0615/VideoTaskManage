using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using VideoSearch.ViewModel;
using VideoSearch.Views.PlayView;
using vlcPlayerLib;

namespace VideoSearch.Views
{
    public partial class PanelViewTaskCompressView : PlayerViewBase
    {
        private long _curPos = 0;

        public PanelViewTaskCompressView()
        {
            InitializeComponent();

            Unloaded += OnUnLoad;
        }

        public void OnUnLoad(object sender, RoutedEventArgs e)
        {
            Unloaded -= OnUnLoad;
            OnStop(sender, e);
        }

        #region Utility
        public void StopMovie()
        {
            if (_vlcPlayer.IsPlaying())
                _vlcPlayer.Stop();
        }


        protected override void ShowPlayer(bool isShow)
        {
            base.ShowPlayer(isShow);

            //PlayerPanel.Visibility = isShow ? Visibility.Visible : Visibility.Hidden;

            PlayButton.IsEnabled = !isShow;
            PauseButton.IsEnabled = isShow;
            StopButton.IsEnabled = isShow;

            GotoBeginButton.IsEnabled = isShow;
            GotoEndButton.IsEnabled = isShow;
            SpeedUpButton.IsEnabled = isShow;
            SpeedDownButton.IsEnabled = isShow;
        }

        #endregion

        #region Buttons Handler
        private void OnPause(object sender, RoutedEventArgs e)
        {
            _vlcPlayer.Pause();

            PlayButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
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
                // 检查视频文件是否存在
                if (!File.Exists(vm.taskItem.CompressedPlayPath))
                {
                    // 显示提示
                    Globals.Instance.MainVM.ShowWorkMask("浓缩视频不存在", false);

                    DisablePlayer();
                }
                else
                {
                    // 重新加载需要延迟
                    await Task.Delay(100);

                    _vlcPlayer.SetVideoInfo(vm.taskItem.CompressedPlayPath, true);

                    //
                    // 时间轴加载标注信息
                    //
                    axEventBarClearEvent();

                    for (int i = 0; i < vm.taskItem.objInfos.Count; i++)
                    {
                        if (vm.taskItem.objInfos[i].sumVideoStartTimeStamp != vm.taskItem.objInfos[i].sumVideoEndTimeStamp)
                        {
                            axEventBarAddEvent((i + 1).ToString(), 
                                "", 
                                (vm.taskItem.objInfos[i].sumVideoStartTimeStamp).ToString(), 
                                (vm.taskItem.objInfos[i].sumVideoEndTimeStamp).ToString());
                        }
                    }
                    

                    // 浓缩物体显示
                    List<string> listPath = new List<string>();
                    if (!String.IsNullOrEmpty(vm.taskItem.CompresseInfoXmlPath))
                    {
                        listPath.Add(vm.taskItem.CompresseInfoXmlPath);
                    }
                    _vlcPlayer.SetWuShiBiaoInfo(true, listPath);

                    OnPlay(this, null);
                }
            }
            else {
                DisablePlayer();
            }

            Globals.Instance.ShowWaitCursor(false);
        }

        private void DisablePlayer()
        {
            OnStop(this, null);
            PlayButton.IsEnabled = false;
        }

        protected new void onDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            base.onDataContextChanged(sender, e);

            if (e.NewValue is PanelViewTaskCompressModel vm)
            {
                vm.View = this;

                var task = vm.InitTaskResult();
            }
        }

        /// <summary>
        /// 播放器初始化
        /// </summary>
        public override void InitPlayer()
        {
            // 初始化播放器
            ClearPlayer();

            base.InitPlayer();

            PlayerPanel.Child = _vlcPlayer;
            TrackBarPanel.Child = _trackBar;

            controlEffect.initEffect(this);

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
        }

        #endregion

        private void ChkTarget_Checked(object sender, RoutedEventArgs e)
        {
            setPlayerTargetShow(true);
        }

        private void ChkTarget_Unchecked(object sender, RoutedEventArgs e)
        {
            setPlayerTargetShow(false);
        }

        /// <summary>
        /// 显示/隐藏物标
        /// </summary>
        /// <param name="show"></param>
        private void setPlayerTargetShow(bool show)
        {
            if (_vlcPlayer != null)
            {
                _vlcPlayer.SetWubiaoShow(show);
            }
        }

        private void ChkTime_Unchecked(object sender, RoutedEventArgs e)
        {
            setPlayerTimeShow(false);
        }

        private void ChkTime_Checked(object sender, RoutedEventArgs e)
        {
            setPlayerTimeShow(true);
        }

        /// <summary>
        /// 显示/隐藏时标
        /// </summary>
        /// <param name="show"></param>
        private void setPlayerTimeShow(bool show)
        {
            if (_vlcPlayer != null)
            {
                _vlcPlayer.SetShibiaoShow(show);
            }
        }
    }
}
