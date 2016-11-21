using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.Xml;

namespace WeChat.iKu.Emoji
{
    public class emojiEntity
    {
        private string _key;
        private BitmapImage _keyImg;
        private Dictionary<string, BitmapImage> _emojiCode = new Dictionary<string, BitmapImage>();

        /// <summary>
        /// 分组
        /// </summary>
        public string Key {
            get { return _key; }
            set { _key = value; }
        }

        /// <summary>
        /// emoji编码
        /// </summary>
        public Dictionary<string, BitmapImage> EmojiCode {
            get { return _emojiCode; }
            set { _emojiCode = value; }
        }

        /// <summary>
        /// 分组图像
        /// </summary>
        public BitmapImage KeyImg {
            get { return _keyImg = EmojiCode.Values.First(); }
        }
    }
}
