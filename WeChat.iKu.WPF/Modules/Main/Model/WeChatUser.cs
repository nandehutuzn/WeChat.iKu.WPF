using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WeChat.iKu.HTTP;
using GalaSoft.MvvmLight;

namespace WeChat.iKu.WPF.Modules.Main.Model
{
    public class WeChatUser : ViewModelBase
    {
        /// <summary>
        /// 消息发送事件
        /// </summary>
        public event Action<WeChatMsg> MsgSent;

        /// <summary>
        /// 消息接收事件
        /// </summary>
        public event Action<WeChatMsg> MsgRecved;

        private string _userName;
        private string _nickName="";
        private string _headImgUrl;
        private string _remarkName;
        private string _sex;
        private string _signature;
        private string _city;
        private string _province;
        private string _pyQuanPin;
        private string _remarkPYQuanPin;
        private string _contactFlag;
        private string _snsFlag;
        private string _keyWord;
        private string _startChar;
        private string _lastTime;
        private string _lastMsg;
        private int _unReadCount = 0;
        private ImageSource _icon;
        private Dictionary<Guid, WeChatMsg> _sentMsg = new Dictionary<Guid, WeChatMsg>();
        private Dictionary<Guid, WeChatMsg> _recvedMsg = new Dictionary<Guid, WeChatMsg>();

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserName {
            get { return _userName; }
            set { _userName = value; }
        }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName {
            get { return _nickName; }
            set { _nickName = value; }
        }

        /// <summary>
        /// 头像URL
        /// </summary>
        public string HeadImgUrl {
            get { return _headImgUrl; }
            set { _headImgUrl = value; }
        }

        /// <summary>
        /// 备注名
        /// </summary>
        public string RemarkName {
            get { return _remarkName; }
            set { _remarkName = value; }
        }

        /// <summary>
        /// 性别0 - （未设置 公众号 保密)，1 - 男， 2 - 女
        /// </summary>
        public string Sex {
            get { return _sex; }
            set { _sex = value; }
        }

        /// <summary>
        /// 签名
        /// </summary>
        public string Signature {
            get { return _signature; }
            set { _signature = value; }
        }

        /// <summary>
        /// 城市
        /// </summary>
        public string City {
            get { return _city; }
            set { _city = value; }
        }

        /// <summary>
        /// 省份
        /// </summary>
        public string Province {
            get { return _province; }
            set { _province = value; }
        }

        /// <summary>
        /// 昵称全拼
        /// </summary>
        public string PyQuanPin {
            get { return _pyQuanPin; }
            set { _pyQuanPin = value; }
        }

        /// <summary>
        /// 备注名全拼
        /// </summary>
        public string RemarkPYQuanPin {
            get { return _remarkPYQuanPin; }
            set { _remarkPYQuanPin = value; }
        }

        /// <summary>
        /// 没发现关键性备注
        /// </summary>
        public string ContactFlag {
            get { return _contactFlag; }
            set { _contactFlag = value; }
        }

        /// <summary>
        /// 0：是公众号或者群聊，其他值是好友 （也有特色情况）
        /// </summary>
        public string SnsFlag {
            get { return _snsFlag; }
            set { _snsFlag = value; }
        }

        /// <summary>
        /// 公众号
        /// </summary>
        public string KeyWord {
            get { return _keyWord; }
            set { _keyWord = value; }
        }

        /// <summary>
        /// 分组的头个字符
        /// </summary>
        public string StartChar {
            get { return _startChar; }
            set { _startChar = value; }
        }

        /// <summary>
        /// 最后消息时间
        /// </summary>
        public string LastTime {
            get { return _lastTime; }
            set { 
                _lastTime = value;
                RaisePropertyChanged("LastTime");
            }
        }

        public string LastMsg {
            get { return _lastMsg; }
            set {
                _lastMsg = value;
                RaisePropertyChanged("LastMsg");
            }
        }

        /// <summary>
        /// 未读条数
        /// </summary>
        public int UnReadCount {
            get { return _unReadCount; }
            set {
                _unReadCount = value;
                RaisePropertyChanged("UnReadCount");
            }
        }

        /// <summary>
        /// 头像
        /// </summary>
        public ImageSource Icon {
            get { return _icon; }
            set { _icon = value; }
        }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string ShowName {
            get { return string.IsNullOrEmpty(_remarkName) ? _nickName : _remarkName; }
        }

        /// <summary>
        /// 显示的拼音全拼
        /// </summary>
        public string ShowPinYin {
            get { return string.IsNullOrEmpty(_remarkPYQuanPin) ? _pyQuanPin : _remarkPYQuanPin; }
        }

        /// <summary>
        /// 发送给对方的消息
        /// </summary>
        public Dictionary<Guid, WeChatMsg> SentMsg{
            get { return _sentMsg; }
        }

        /// <summary>
        /// 收到对方的消息
        /// </summary>
        public Dictionary<Guid, WeChatMsg> RecvedMsg {
            get { return _recvedMsg; }
        }

        /// <summary>
        /// 触发消息接收事件
        /// </summary>
        /// <param name="msg"></param>
        private void OnMsgRecved(WeChatMsg msg)
        {
            if (MsgRecved != null)
                MsgRecved(msg);
        }

        /// <summary>
        /// 触发消息发送事件
        /// </summary>
        /// <param name="msg"></param>
        private void OnMsgSent(WeChatMsg  msg)
        {
            if (MsgSent != null)
                MsgSent(msg);
        }

        /// <summary>
        /// 接收来自该用户的消息
        /// </summary>
        /// <param name="msg"></param>
        public void ReceivedMsg(WeChatMsg msg)
        {
            _recvedMsg.Add(Guid.NewGuid(), msg);
            OnMsgRecved(msg); ;
        }

        /// <summary>
        /// 向该用户发送消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="showOnly"></param>
        public void SendMsg(WeChatMsg msg, bool showOnly)
        {
            if (!showOnly)
            {
                WeChatService wcs = new WeChatService();
                wcs.SendMsg(msg.Msg, msg.From, msg.To, msg.Type);
            }
            _sentMsg.Add(Guid.NewGuid(), msg);
            OnMsgSent(msg);
        }

        /// <summary>
        /// 获取该用户发送的未读消息
        /// </summary>
        /// <returns></returns>
        public List<WeChatMsg> GetUnreadMsg()
        {
            var query= _recvedMsg.Values.Where(o => !o.Readed);
            return query.Count() > 0 ? query.ToList() : null;
        }

        /// <summary>
        /// 获取最近的一条消息
        /// </summary>
        /// <returns></returns>
        public WeChatMsg GetLatestMsg()
        {
            WeChatMsg msg = null;
            if (_sentMsg.Count > 0 && _recvedMsg.Count > 0)
                msg = _sentMsg.Last().Value.Time > _recvedMsg.Last().Value.Time ? _sentMsg.Last().Value : _recvedMsg.Last().Value;
            else if (_sentMsg.Count > 0)
                msg = _sentMsg.Last().Value;
            else if (_recvedMsg.Count > 0)
                msg = _recvedMsg.Last().Value;

            return msg;
        }
    }
}
