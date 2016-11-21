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
using System.Windows.Shapes;
using System.Windows.Threading;
using WeChat.iKu.Emoji;

namespace WeChat.iKu.WPF.Modules.Main.View
{
    /// <summary>
    /// MainUC.xaml 的交互逻辑
    /// </summary>
    public partial class MainUC : Window
    {
        private WindowState _lastWindowState = WindowState.Normal;

        public MainUC()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 双击最大化或还原
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                ShowOrhide();
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();//左键拖动
        }

        /// <summary>
        /// 最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_min_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// 正常大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_normal_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
            ShowOrhide();
        }

        /// <summary>
        /// 最大化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_max_Click(object sender, RoutedEventArgs e)
        {
            WindowState = System.Windows.WindowState.Maximized;
            ShowOrhide();
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_close_Click(object sender, RoutedEventArgs e)
        {
            WindowState = System.Windows.WindowState.Minimized;
            ShowInTaskbar = false;
        }

        private void NotificationAreaIcon_MouseClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                Open();
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// 显示或隐藏按钮
        /// </summary>
        private void ShowOrhide()
        {
            switch (WindowState)
            {
                case WindowState.Maximized:
                    btn_max.Visibility = System.Windows.Visibility.Collapsed;
                    btn_normal.Visibility = System.Windows.Visibility.Visible;
                    break;
                case WindowState.Minimized:
                    break;
                case WindowState.Normal:
                    btn_normal.Visibility = System.Windows.Visibility.Collapsed;
                    btn_max.Visibility = System.Windows.Visibility.Visible;
                    break;
                default:
                    break;
            }
        }
        
        /// <summary>
        /// 状态变更
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStateChanged(EventArgs e)
        {
            this._lastWindowState = WindowState == WindowState.Minimized ? _lastWindowState : WindowState;
        }

        private void Open()
        {
            WindowState = _lastWindowState;
            ShowInTaskbar = true;
            Show();
        }
    }
}
