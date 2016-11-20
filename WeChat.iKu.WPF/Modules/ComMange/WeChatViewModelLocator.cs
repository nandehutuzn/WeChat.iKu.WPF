using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeChat.iKu.WPF.Modules.Login.ViewModel;
using WeChat.iKu.WPF.Modules.Main.ViewModel;

namespace WeChat.iKu.WPF.Modules.ComMange
{
    class WeChatViewModelLocator
    {
        private static readonly object SyncObject = new object();

        private static WeChatViewModelLocator _instance;
        /// <summary>
        /// ViewModel实例
        /// </summary>
        public static WeChatViewModelLocator Instance{
            get {
                if (_instance == null)
                {
                    lock (SyncObject)
                    {
                        if (_instance == null)
                            _instance = new WeChatViewModelLocator();
                    }
                }
                return _instance;
            }
            set { _instance = value; }
        }

        private LoginVM _loginViewModel;
        /// <summary>
        /// 登录
        /// </summary>
        public LoginVM LoginViewModel {
            get { return _loginViewModel ?? (_loginViewModel = new LoginVM()); }
        }

        private MainVM _mainViewModel;
        /// <summary>
        /// 主页面
        /// </summary>
        public MainVM MainViewModel{
            get { return _mainViewModel ?? (_mainViewModel = new MainVM()); }
        }
    }
}
