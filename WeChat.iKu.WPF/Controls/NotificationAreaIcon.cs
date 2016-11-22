using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows;

namespace WeChat.iKu.WPF.Controls
{
    [ContentProperty("Text")]
    [DefaultEvent("MouseDoubleClick")]
    public class NotificationAreaIcon : FrameworkElement
    {
        /// <summary>
        /// 系统托盘
        /// </summary>
        NotifyIcon notifyIcon;

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(NotificationAreaIcon));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(NotificationAreaIcon));

        public static readonly DependencyProperty MenuItemsProperty =
            DependencyProperty.Register("MenuItems", typeof(List<MenuItem>), typeof(NotificationAreaIcon), new PropertyMetadata(new List<MenuItem>()));

        public static readonly RoutedEvent MouseClickEvent = EventManager.RegisterRoutedEvent(
            "MouseClick", RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(NotificationAreaIcon));

        public static readonly RoutedEvent MouseDoubleClickEvent = EventManager.RegisterRoutedEvent(
            "MouseDoubleClick", RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(NotificationAreaIcon));

        /// <summary>
        /// 图标
        /// </summary>
        public ImageSource Icon {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// 文本
        /// </summary>
        public string Text {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// 菜单项
        /// </summary>
        public List<MenuItem> MenuItems {
            get { return (List<MenuItem>)GetValue(MenuItemsProperty); }
            set { SetValue(MenuItemsProperty, value); }
        }

        /// <summary>
        /// 鼠标单击事件
        /// </summary>
        public event MouseButtonEventHandler MouseClick {
            add { AddHandler(MouseClickEvent, value); }
            remove { RemoveHandler(MouseClickEvent, value); }
        }

        /// <summary>
        /// 鼠标双击事件
        /// </summary>
        public event MouseButtonEventHandler MouseDoubleClick {
            add { AddHandler(MouseDoubleClickEvent, value); }
            remove { RemoveHandler(MouseDoubleClickEvent, value); }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            notifyIcon = new NotifyIcon();
            notifyIcon.Text = Text;
            if (!DesignerProperties.GetIsInDesignMode(this))
                notifyIcon.Icon = FromImageSource(Icon);
            notifyIcon.Visible = FromVisibility(Visibility);
            if (MenuItems != null && MenuItems.Count > 0)
                notifyIcon.ContextMenu = new ContextMenu(MenuItems.ToArray());

            notifyIcon.MouseDown += notifyIcon_MouseDown;
            notifyIcon.MouseUp += notifyIcon_MouseUp;
            notifyIcon.MouseClick += notifyIcon_MouseClick;
            notifyIcon.MouseDoubleClick += notifyIcon_MouseDoubleClick;
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        /// <summary>
        /// 鼠标路由事件
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="e"></param>
        private void OnRaiseEvent(RoutedEvent handler, MouseButtonEventArgs e)
        {
            e.RoutedEvent = handler;
            RaiseEvent(e);
        }

        void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            notifyIcon.Dispose();
        }

        /// <summary>
        /// 双击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void notifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            OnRaiseEvent(MouseDoubleClickEvent, new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, ToMouseButton(e.Button)));
        }

        /// <summary>
        /// 单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void notifyIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            OnRaiseEvent(MouseClickEvent, new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, ToMouseButton(e.Button)));
        }

        /// <summary>
        /// 鼠标抬起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void notifyIcon_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            OnRaiseEvent(MouseUpEvent, new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, ToMouseButton(e.Button)));
        }

        /// <summary>
        /// 鼠标按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void notifyIcon_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            OnRaiseEvent(MouseDownEvent, new MouseButtonEventArgs(InputManager.Current.PrimaryMouseDevice, 0, ToMouseButton(e.Button)));
        }

        /// <summary>
        /// 显示小图标
        /// </summary>
        /// <param name="visibility"></param>
        /// <returns></returns>
        private bool FromVisibility(Visibility visibility)
        {
            return visibility == Visibility.Visible;
        }

        /// <summary>
        /// 将ImageSource转换成icon
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        private Icon FromImageSource(ImageSource icon)
        {
            if (icon == null)
                return null;
            Uri iconUri = new Uri(icon.ToString());
            return new Icon(System.Windows.Application.GetResourceStream(iconUri).Stream);
        }

        /// <summary>
        /// 是鼠标哪个键按下的
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        private MouseButton ToMouseButton(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    return MouseButton.Left;
                case MouseButtons.Middle:
                    return MouseButton.Middle;
                case MouseButtons.Right:
                    return MouseButton.Right;
                case MouseButtons.XButton1:
                    return MouseButton.XButton1;
                case MouseButtons.XButton2:
                    return MouseButton.XButton2;
            }
            throw new InvalidOperationException();
        }
    }
}
