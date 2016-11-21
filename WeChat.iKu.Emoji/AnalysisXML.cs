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
    public class AnalysisXML
    {
        private List<emojiEntity> _emojiList = new List<emojiEntity>();
        /// <summary>
        /// emoji集合
        /// </summary>
        public List<emojiEntity> EmojiList {
            get { return _emojiList; }
            set { _emojiList = value; }
        }

        /// <summary>
        /// 解析XML
        /// </summary>
        public void AnayXML()
        {
            XmlDocument xmlDoc = new XmlDocument();
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream("WeChat.iKu.Emoji.Emoji.xml");//文件需为嵌入的资源
            xmlDoc.Load(stream);
            XmlNode root = xmlDoc.SelectSingleNode("array");
            XmlNodeList nodeList = root.ChildNodes;
            //循环列表，获得相应的内容
            foreach (XmlNode xn in nodeList)
            {
                XmlElement xe = (XmlElement)xn;
                XmlNodeList subList = xe.ChildNodes;
                emojiEntity entity = new emojiEntity();
                foreach (XmlNode xmlNode in subList)
                {
                    if (xmlNode.Name == "key")
                        entity.Key = xmlNode.InnerText;
                    if (xmlNode.Name == "array")
                    {
                        XmlElement lastXe = (XmlElement)xmlNode;
                        foreach (XmlNode lastNode in lastXe)
                        {
                            if (lastNode.Name == "e")
                                entity.EmojiCode.Add(lastNode.InnerText, GetEmoji(lastNode.InnerText));
                        }
                    }
                }
                EmojiList.Add(entity);
            }
        }

        /// <summary>
        /// 返回Emoji
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private BitmapImage GetEmoji(string name)
        {
            BitmapImage bitmap = new BitmapImage();
            string imgUrl = "/WeChat.iKu.Emoji;Component/Image/" + "emoji_" + name + ".png";
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imgUrl, UriKind.Relative);
            bitmap.EndInit();
            return bitmap;
        }
    }
}
