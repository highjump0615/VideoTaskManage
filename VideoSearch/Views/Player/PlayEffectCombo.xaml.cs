using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VideoSearch.Views.PlayView;

namespace VideoSearch.Views.Player
{
    /// <summary>
    /// PlayEffectCombo.xaml 的交互逻辑
    /// </summary>
    public partial class PlayEffectCombo : UserControl
    {
        private PlayerViewBase _viewParent;

        public PlayEffectCombo()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化播放
        /// </summary>
        public void initEffect(PlayerViewBase view)
        {
            cmbEffect.SelectedIndex = -1;

            // 隐藏标题
            txtComboLabel.Visibility = Visibility.Visible;
            _viewParent = view;
        }

        /// <summary>
        /// 效果选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbEffect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmbEffect = sender as ComboBox;

            if (cmbEffect.SelectedIndex >= 0)
            {
                // 隐藏标题
                txtComboLabel.Visibility = Visibility.Collapsed;

                // 应用效果
                _viewParent.playWithEffect(cmbEffect.SelectedIndex);
            }
        }
    }
}
