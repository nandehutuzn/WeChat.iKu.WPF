using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using WeChat.iKu.HTTP;
using WeChat.iKu.WPF.Modules.Main.Model;
using System.Collections.ObjectModel;
using WeChat.iKu.Tools.Heplers;
using System.Windows.Threading;
using WeChat.iKu.Emoji;

namespace WeChat.iKu.WPF.Modules.Main.ViewModel
{
    public class MainVM : ViewModelBase
    {
        private WeChatService wcs = new WeChatService();
        System.Timers.Timer timer = new System.Timers.Timer();

        /// <summary>
        /// 开启聊天事件
        /// </summary>
        public event Action<WeChatUser> StartChat;

        public MainVM()
        {
            timer.Interval = 2000;
            timer.Elapsed += timer_Elapsed;
            Init();
        }

        private WeChatUser _me;
        /// <summary>
        /// 当前登录微信用户
        /// </summary>
        public WeChatUser Me {
            get { return _me; }
            set {
                _me = value;
                RaisePropertyChanged("Me");
            }
        }

        private WeChatUser _friendUser;
        /// <summary>
        /// 聊天好友
        /// </summary>
        public WeChatUser FriendUser {
            get { return _friendUser; }
            set {
                if (value != _friendUser)
                {
                    if (_friendUser != null)
                    {
                        _friendUser.MsgRecved -= _friendUser_MsgRecved;
                        _friendUser.MsgSent -= _friendUser_MsgSent;
                    }

                    _friendUser = value;
                    if (_friendUser != null)
                    {
                        _friendUser.MsgRecved += _friendUser_MsgRecved;
                        _friendUser.MsgSent += _friendUser_MsgSent;
                        IEnumerable<KeyValuePair<Guid, WeChatMsg>> dic = _friendUser.RecvedMsg.Concat(_friendUser.SentMsg);
                        foreach (KeyValuePair<Guid, WeChatMsg> p in dic)
                        {
                            if (p.Value.From == _friendUser.UserName)
                                ShowReceiveMsg(p.Value);
                            else
                                ShowSendMsg(p.Value);
                            p.Value.Readed = true;
                            _friendUser.UnReadCount = 0;
                        }
                    }
                }
                RaisePropertyChanged("FriendUser");
            }
        }

        private WeChatUser _friendInfo;
        /// <summary>
        /// 好友信息
        /// </summary>
        public WeChatUser FriendInfo {
            get { return _friendInfo; }
            set {
                _friendInfo = value;
                RaisePropertyChanged("FriendInfo");
            }
        }

        private List<object> _contact_all = new List<object>();
        /// <summary>
        /// 通讯录
        /// </summary>
        public List<object> Contact_all {
            get { return _contact_all; }
            set {
                _contact_all = value;
                RaisePropertyChanged("Contact_all");
            }
        }

        private ObservableCollection<object> _contact_latest = new ObservableCollection<object>();
        /// <summary>
        /// 最近联系人
        /// </summary>
        public ObservableCollection<object> Contact_latest {
            get { return _contact_latest; }
            set {
                _contact_latest = value;
                RaisePropertyChanged("Contact_latest");
            }
        }

        private object _select_Conntact_latest = new object();
        /// <summary>
        /// 聊天列表选中
        /// </summary>
        public object Select_Contact_latest {
            get { return _select_Conntact_latest; }
            set {
                _select_Conntact_latest = value;
                RaisePropertyChanged("Select_Contact_latest");
            }
        }

        private object _select_Contact_all = new object();
        /// <summary>
        /// 通讯录选中
        /// </summary>
        public object Select_Contact_all {
            get { return _select_Contact_all; }
            set {
                _select_Contact_all = value;
                RaisePropertyChanged("Select_Contact_all");
            }
        }

        private string _userName = string.Empty;
        /// <summary>
        /// 用于在顶部显示用户名
        /// </summary>
        public string UserName {
            get { return _userName; }
            set {
                _userName = value;
                RaisePropertyChanged("UserName");
            }
        }

        private ObservableCollection<ChatMsg> _chatList = new ObservableCollection<ChatMsg>();
        /// <summary>
        /// 聊天列表
        /// </summary>
        public ObservableCollection<ChatMsg> ChatList {
            get { return _chatList; }
            set {
                _chatList = value;
                RaisePropertyChanged("ChatList");
            }
        }

        private string _sendMessage;
        //发送消息内容
        public string SendMessage {
            get { return _sendMessage; }
            set {
                _sendMessage = value;
                RaisePropertyChanged("SendMessage");
            }
        }

        private Visibility _tootip_Visibility = Visibility.Collapsed;
        /// <summary>
        /// 是否显示提示
        /// </summary>
        public Visibility Tootip_Visibility{
            get { return _tootip_Visibility; }
            set {
                _tootip_Visibility = value;
                RaisePropertyChanged("Tootip_Visibility");
            }
        }

        private bool _isChecked = true;
        /// <summary>
        /// 是否被选中
        /// </summary>
        public bool IsChecked {
            get { return _isChecked; }
            set {
                _isChecked = value;
                RaisePropertyChanged("IsChecked");
            }
        }

        private Visibility _emoji_Visibility = Visibility.Collapsed;
        /// <summary>
        /// Emoji显隐
        /// </summary>
        public Visibility Emoji_Visibility {
            get { return _emoji_Visibility; }
            set {
                _emoji_Visibility = value;
                RaisePropertyChanged("Emoji_Visibility");
            }
        }

        private bool _emoji_Popup = false;
        /// <summary>
        /// Popup是否弹出
        /// </summary>
        public bool Emoji_Popup {
            get { return _emoji_Popup; }
            set {
                _emoji_Popup = value;
                RaisePropertyChanged("Emoji_Popup");
            }
        }

        /// <summary>
        /// 倒计时隐藏按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Tootip_Visibility = Visibility.Collapsed;
            timer.Stop();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            JObject init_result = wcs.WeChatInit();
            List<object> contact_all = new List<object>();
            if (init_result != null)
            {
                _me = new WeChatUser();
                _me.UserName = init_result["User"]["UserName"].ToString();
                _me.City = "";
                _me.HeadImgUrl = init_result["User"]["HeadImgUrl"].ToString();
                _me.NickName = init_result["User"]["NickName"].ToString();
                _me.Province = "";
                _me.PyQuanPin = init_result["User"]["PYQuanPin"].ToString();
                _me.RemarkName = init_result["User"]["RemarkName"].ToString();
                _me.RemarkPYQuanPin = init_result["User"]["RemarkPYQuanPin"].ToString();
                _me.Sex = init_result["User"]["Sex"].ToString();
                _me.Signature = init_result["User"]["Signature"].ToString();
                _me.Icon = GetIcon(wcs, _me.UserName);
                //部分好友名单
                foreach (JObject contact in init_result["ContactList"])
                {
                    WeChatUser user = new WeChatUser();
                    user.UserName = contact["UserName"].ToString();
                    user.City = contact["City"].ToString();
                    user.HeadImgUrl = contact["HeadImgUrl"].ToString();
                    user.NickName = contact["NickName"].ToString();
                    user.Province = contact["Province"].ToString();
                    user.PyQuanPin = contact["PYQuanPin"].ToString();
                    user.RemarkName = contact["RemarkName"].ToString();
                    user.RemarkPYQuanPin = contact["RemarkPYQuanPin"].ToString();
                    user.Sex = contact["Sex"].ToString();
                    user.Signature = contact["Signature"].ToString();
                    user.Icon = GetIcon(wcs, user.UserName);
                    user.SnsFlag = contact["SnsFlag"].ToString();
                    user.KeyWord = contact["KeyWord"].ToString();

                    _contact_latest.Add(user);
                }
            }
            //通讯录
            JObject contact_result = wcs.GetContact();
            if (contact_result != null)
            {//完整好友名单
                foreach (JObject contact in contact_result["MemberList"])
                {
                    WeChatUser user = new WeChatUser();
                    user.UserName = contact["UserName"].ToString();
                    user.City = contact["City"].ToString();
                    user.HeadImgUrl = contact["HeadImgUrl"].ToString();
                    user.NickName = contact["NickName"].ToString();
                    user.Province = contact["Province"].ToString();
                    user.PyQuanPin = contact["PYQuanPin"].ToString();
                    user.RemarkName = contact["RemarkName"].ToString();
                    user.RemarkPYQuanPin = contact["RemarkPYQuanPin"].ToString();
                    user.Sex = contact["Sex"].ToString();
                    user.Signature = contact["Signature"].ToString();
                    user.Icon = GetIcon(wcs, user.UserName);
                    user.SnsFlag = contact["SnsFlag"].ToString();
                    user.KeyWord = contact["KeyWord"].ToString();
                    user.StartChar = GetStartChar(user);

                    contact_all.Add(user);
                }

                IOrderedEnumerable<object> list_all = contact_all.OrderBy(p =>
                    (p as WeChatUser).StartChar).ThenBy(p =>
                        (p as WeChatUser).NickName);

                WeChatUser wcu;
                string start_char;
                foreach (object o in list_all)
                {
                    wcu = o as WeChatUser;
                    start_char = wcu.StartChar;
                    if (!_contact_all.Contains(start_char.ToUpper()))
                        _contact_all.Add(start_char.ToUpper());
                    _contact_all.Add(o);
                }
            }
        }

        /// <summary>
        /// 获取头像
        /// </summary>
        /// <param name="wcs"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        private ImageSource GetIcon(WeChatService wcs, string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return null;
            ImageSource icon;
            if (userName.Contains("@@"))//讨论组
                icon = wcs.GetIcon(userName, StaticUrl.Url_GetHeadImg);
            else
                icon = wcs.GetIcon(userName);

            return icon;
        }

        /// <summary>
        /// 获取分组的头
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string GetStartChar(WeChatUser user)
        {
            string start_char;
            if (user.KeyWord == "gh_" && user.SnsFlag.Equals("0") || user.KeyWord == "cmb")
                start_char = "公众号";//user.KeyWord =="cmb"是招商银行信用卡
            else if (user.UserName.Contains("@@") && user.SnsFlag.Equals("0"))
                start_char = "群聊";
            else
                start_char = string.IsNullOrEmpty(user.ShowPinYin) ?
                    string.Empty : user.ShowPinYin.Substring(0, 1);

            return start_char;
        }

        private RelayCommand _loadedCommand;
        /// <summary>
        /// 载入
        /// </summary>
        public RelayCommand LoadedCommand{
            get {
                return _loadedCommand ?? (_loadedCommand = new RelayCommand(() =>
                    {
                        Task.Run(() =>
                            {
                                string sync_flag = "";
                                JObject sync_result;
                                while (true)
                                {
                                    //同步检查
                                    sync_flag = wcs.WeChatSyncCheck();
                                    if (sync_flag != null) //这里应该判断sync_flag中Selector的值
                                    {
                                        sync_result = wcs.WeChatSync();
                                        if (sync_result != null)
                                        {
                                            if (sync_result["AddMsgCount"] != null && sync_result["AddMsgCount"].ToString() != "0")
                                            {
                                                foreach (JObject m in sync_result["AddMsgList"])
                                                {
                                                    string from = m["FromUserName"].ToString();
                                                    string to = m["ToUserName"].ToString();
                                                    string content = m["Content"].ToString();
                                                    string type = m["MsgType"].ToString();

                                                    WeChatMsg msg = new WeChatMsg();
                                                    msg.From = from;
                                                    msg.Msg = type == "1" ? content : "请在其他设备上查看消息";//只接受文本消息
                                                    msg.Readed = false;
                                                    msg.Time = DateTime.Now;
                                                    msg.To = to;
                                                    msg.Type = int.Parse(type);
                                                    if (msg.Type == 51) //屏蔽系统数据
                                                        continue;

                                                    Application.Current.Dispatcher.BeginInvoke((Action)delegate()
                                                    {
                                                        WeChatUser user;
                                                        bool exist_latest_contact = false;
                                                        foreach (object u in Contact_latest)
                                                        {
                                                            user = u as WeChatUser;
                                                            if (user != null)
                                                            {
                                                                if (user.UserName == msg.From && msg.To == _me.UserName)
                                                                { //接收别人消息
                                                                    Contact_latest.Remove(user);
                                                                    user.UnReadCount = user.GetUnreadMsg() == null ? 0 : user.GetUnreadMsg().Count;
                                                                    List<WeChatMsg> unReadList = user.GetUnreadMsg();
                                                                    WeChatMsg latestMsg = user.GetLatestMsg();
                                                                    if (unReadList != null)
                                                                    {
                                                                        user.LastTime = unReadList[unReadList.Count - 1].Time.ToShortTimeString();
                                                                        user.LastMsg = unReadList[unReadList.Count - 1].Msg.ToString();
                                                                        user.LastMsg = user.LastMsg.Length <= 10 ? user.LastMsg : user.LastMsg.Substring(0, 10) + "...";
                                                                    }
                                                                    else//最新消息
                                                                    {
                                                                        if (latestMsg != null)
                                                                        {
                                                                            user.LastTime = latestMsg.Time.ToShortTimeString();
                                                                            user.LastMsg = latestMsg.Msg.ToString();
                                                                            user.LastMsg = user.LastMsg.Length <= 10 ? user.LastMsg : user.LastMsg.Substring(0, 10) + "...";
                                                                        }
                                                                    }

                                                                    Contact_latest.Insert(0, user);
                                                                    exist_latest_contact = true;
                                                                    user.ReceivedMsg(msg);
                                                                    break;
                                                                }
                                                                else if (user.UserName == msg.To && msg.From == _me.UserName)
                                                                { //同步自己在其他设备上发送的消息
                                                                    Contact_latest.Remove(user);
                                                                    Contact_latest.Insert(0, user);
                                                                    exist_latest_contact = true;
                                                                    user.SendMsg(msg, true);
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        if (!exist_latest_contact)
                                                        {
                                                            foreach (object o in Contact_all)
                                                            {
                                                                WeChatUser friend = o as WeChatUser;
                                                                if (friend != null && friend.UserName == msg.From && msg.To == _me.UserName)
                                                                {
                                                                    Contact_latest.Insert(0, friend);
                                                                    friend.ReceivedMsg(msg);
                                                                    break;
                                                                }
                                                                if (friend != null && friend.UserName == msg.To && msg.From == _me.UserName)
                                                                {
                                                                    Contact_latest.Insert(0, friend);
                                                                    friend.SendMsg(msg, true);
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    });
                                                }
                                            }
                                        }
                                    }
                                    Thread.Sleep(20);
                                }
                            });
                    }));
            }
        }

        private RelayCommand _chatCommand;
        /// <summary>
        /// 聊天列表的选中事件
        /// </summary>
        public RelayCommand ChatCommand{
            get {
                return _chatCommand ?? (_chatCommand = new RelayCommand(() =>
                    {
                        WeChatUser user = Select_Contact_latest as WeChatUser;
                        if (user != null)
                        {
                            UserName = user.ShowName;
                            ChatList.Clear();
                            FriendUser = user;
                        }
                    }));
            }
        }

        private RelayCommand _friendCommand;
        /// <summary>
        /// 通讯录选中事件
        /// </summary>
        public RelayCommand FriendCommad {
            get {
                return _friendCommand ?? (_friendCommand = new RelayCommand(() =>
                    {
                        if (Select_Contact_all is WeChatUser)
                            FriendInfo = Select_Contact_all as WeChatUser;
                    }));
            }
        }

        private RelayCommand _friendSendCommand;
        /// <summary>
        /// 用户信息页面的发送消息按钮
        /// </summary>
        public RelayCommand FriendSendCommand {
            get{
                return _friendSendCommand ?? (_friendSendCommand = new RelayCommand(() =>
                    {
                        Contact_latest.Remove(Select_Contact_all);
                        Contact_latest.Insert(0, Select_Contact_all);
                    }));
            }
        }

        private RelayCommand _sendCommand;
        /// <summary>
        /// 发送消息
        /// </summary>
        public RelayCommand SendCommand {
            get {
                return _sendCommand ?? (_sendCommand = new RelayCommand(() =>
                    {
                        if (!string.IsNullOrEmpty(SendMessage))
                        {
                            WeChatMsg msg = new WeChatMsg();
                            msg.From = _me.UserName;
                            msg.Readed = false;
                            msg.To = _friendUser.UserName;
                            msg.Type = 1;
                            msg.Msg = SendMessage;
                            msg.Time = DateTime.Now;
                            _friendUser.SendMsg(msg, false);
                            SendMessage = string.Empty;
                        }
                        else
                        {
                            Tootip_Visibility = Visibility.Visible;
                            timer.Start();
                        }
                    }));
            }
        }

        /// <summary>
        /// 发送消息完成
        /// </summary>
        /// <param name="msg"></param>
        private void _friendUser_MsgSent(WeChatMsg msg)
        {
            ShowSendMsg(msg);
        }

        /// <summary>
        /// 收到新消息
        /// </summary>
        /// <param name="msg"></param>
        private void _friendUser_MsgRecved(WeChatMsg msg)
        {
            ShowReceiveMsg(msg);
        }

        /// <summary>
        /// 显示发出的消息
        /// </summary>
        /// <param name="msg"></param>
        private void ShowSendMsg(WeChatMsg msg)
        {
            ChatMsg chatmsg = new ChatMsg();
            chatmsg.Image = _me.Icon;
            chatmsg.Message = msg.Msg;
            chatmsg.FlowDir = FlowDirection.RightToLeft;
            chatmsg.TbColor = (Brush)new BrushConverter().ConvertFromString("#FF98E165");
            ChatList.Add(chatmsg);
        }

        /// <summary>
        /// 显示收到的信息
        /// </summary>
        /// <param name="msg"></param>
        private void ShowReceiveMsg(WeChatMsg msg)
        {
            ChatMsg chatmsg = new ChatMsg();
            chatmsg.Message = msg.Msg;
            chatmsg.FlowDir = FlowDirection.LeftToRight;
            chatmsg.TbColor = Brushes.White;
            Contact_all.ForEach(o =>
                {
                    WeChatUser user = o as WeChatUser;
                    if (user != null)
                    {
                        if (user.UserName == msg.From)
                        {
                            chatmsg.Image = user.Icon;
                            return;
                        }
                    }
                });
            ChatList.Add(chatmsg);
        }

        private RelayCommand<EmojiTabControlUC> _emojiCommand;
        /// <summary>
        /// Emoji按钮事件
        /// </summary>
        public RelayCommand<EmojiTabControlUC> EmojiCommand {
            get {
                return _emojiCommand ?? (_emojiCommand = new RelayCommand<EmojiTabControlUC>(p => _emoji_Popup = true));
            }
        }
    }
}
