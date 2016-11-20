using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight;
using WeChat.iKu.HTTP;
using SIO=System.IO;
using WeChat.iKu.Tools.Heplers;

namespace WeChat.iKu.WPF.Modules.Login.ViewModel
{
    public class LoginVM :ViewModelBase
    {
        LoginService ls = new LoginService();
        private CancellationTokenSource _cst = new CancellationTokenSource();
        private Task _task;
        //Thread thread;

        public LoginVM()
        {
            QRCodeImageSource = InitImageSource();
            QRCodeLabelText = "欢迎使用 WeChat.iKu by 男的糊涂...";
        }

        /// <summary>
        /// 获取二维码
        /// </summary>
        private void GetQRCode()
        {
            //QRCodeImageSource = ls.GetQRCode();  //同步方式获取二维码导致页面卡死
            //启用一个新线程来循环登录
            //thread = new System.Threading.Thread(LoopLoginCheck);
            //thread.IsBackground = true;
            //thread.Start();
            _task = Task.Run(() => LoopLoginCheck(_cst.Token));

        }

        /// <summary>
        /// 循环检测是否登录成功
        /// </summary>
        private void LoopLoginCheck(CancellationToken token)
        {
            QRCodeImageSource = ls.GetQRCode();//因为该属性和界面显示图片已经绑定了，所以可以放在任务线程获取二维码
            QRCodeLabelText = "请使用微信扫一扫以登录";
            object login_result = null;
            while (true)
            {
                login_result = ls.LoginCheck();
                if (login_result is ImageSource)
                {//已扫描，未手机端还未确定登录
                    HeadImageSource = login_result as ImageSource;
                    Messenger.Default.Send<object>(null, "ShowLoginInfoUC");
                }
                else if (login_result is string)
                { //完成登录
                    ls.GetSidUid(login_result as string); //访问登录跳转URL
                    Messenger.Default.Send<object>(null, "HideLoginUC");
                    //thread.Abort();
                    break;  //跳出循环即可结束该任务
                    
                }
                else if (login_result is int)
                { //超时
                    Messenger.Default.Send<object>(null, "ShowQRCodeUC");//返回二维码页面
                }
                Thread.Sleep(20);
            }
        }

        private RelayCommand _loadCommand;
        /// <summary>
        /// 载入事件
        /// </summary>
        public RelayCommand LoadCommand {
            get {
                return _loadCommand ?? (_loadCommand = new RelayCommand(
                    () => GetQRCode()));
            }
        }

        private RelayCommand _returnQRCodeCommand;
        /// <summary>
        /// 点击返回二维码
        /// </summary>
        public RelayCommand ReturnQRCodeCommand{
            get {
                return _returnQRCodeCommand ?? (_returnQRCodeCommand = new RelayCommand(
                    () => Messenger.Default.Send<object>(null, "ShowQRCodeUC")));
            }
        }
        private ImageSource InitImageSource()
        {
            string imageFile = SIO.Path.Combine(Environment.CurrentDirectory, "wait.png");
            if (SIO.File.Exists(imageFile))
            {
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.UriSource = new Uri(imageFile);
                bi.EndInit();
                return bi;
            }
            return null;
        }

        private ImageSource _qrCodeImageSource;
        /// <summary>
        /// 二维码图片
        /// </summary>
        public ImageSource QRCodeImageSource {
            get { return _qrCodeImageSource; }
            set { 
                _qrCodeImageSource = value;
                RaisePropertyChanged("QRCodeImageSource");
            }
        }

        private ImageSource _headImageSource;
        /// <summary>
        /// 头像图片
        /// </summary>
        public ImageSource HeadImageSource{
            get { return _headImageSource; }
            set { 
                _headImageSource = value;
                RaisePropertyChanged("HeadImageSource");
            }
        }

        private string _qrCodeLabelText;
        /// <summary>
        /// 二维码下面textblock位置显示文字
        /// </summary>
        public string QRCodeLabelText{
            get { return _qrCodeLabelText; }
            set {
                _qrCodeLabelText = value;
                RaisePropertyChanged("QRCodeLabelText");
            }
        }
    }
}
