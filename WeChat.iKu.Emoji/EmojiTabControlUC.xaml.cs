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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WeChat.iKu.Emoji
{
    /// <summary>
    /// EmojiTabControlUC.xaml 的交互逻辑
    /// </summary>
    public partial class EmojiTabControlUC : UserControl
    {
        private static ObservableCollection<emojiEntity> _emojiList =
            new ObservableCollection<emojiEntity>();

        private KeyValuePair<string, BitmapImage> _selectEmoji =
            new KeyValuePair<string, BitmapImage>();
        /// <summary>
        /// emoji集合
        /// </summary>
        public static ObservableCollection<emojiEntity> EmojiList{
            get { return _emojiList; }
            set { _emojiList = value; }
        }

        /// <summary>
        /// 选中项
        /// </summary>
        public KeyValuePair<string, BitmapImage> SelectEmoji {
            get { return _selectEmoji; }
            set { _selectEmoji = value; }
        }
        public EmojiTabControlUC()
        {
            InitializeComponent();
            if (EmojiList.Count > 0)
                return;
            AnalysisXML anlyxml = new AnalysisXML();
            anlyxml.AnayXML();
            EmojiList = new ObservableCollection<emojiEntity>(anlyxml.EmojiList);
        }

        /// <summary>
        /// 点击选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show(e.Source.GetType().Name + e.OriginalSource.GetType().Name + SelectEmoji.Key);
            e.Handled = true;
        }
    }
}
