using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.iKu.Tools.Heplers
{
    public class TimeHelper
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        /// <summary>
        /// 时间戳取反
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp_TakeBack()
        {
            return ~GetTimeStamp();
        }
    }
}
