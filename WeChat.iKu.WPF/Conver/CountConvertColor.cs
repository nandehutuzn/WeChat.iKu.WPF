using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Data;

namespace WeChat.iKu.WPF.Conver
{
    /// <summary>
    /// 其他显示未读信息
    /// </summary>
    class CountConvertColor:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if ((int)value > 0)
                {
                    Color color = Color.FromRgb(192, 0, 0);
                    return (Brush)new SolidColorBrush(color);
                }
            }
            Color col = Color.FromRgb(70, 56, 69);
            return (Brush)new SolidColorBrush(col);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
