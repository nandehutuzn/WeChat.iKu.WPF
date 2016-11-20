using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Data;
using WeChat.iKu.WPF.Modules.Main.Model;

namespace WeChat.iKu.WPF.Conver
{
    class ObjectConvertColor:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is WeChatUser)
            {
                return (Brush)new BrushConverter().ConvertFromString("#FFEAEAEA");
            }
            return (Brush)new BrushConverter().ConvertFromString("#FFE0E0E0");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
