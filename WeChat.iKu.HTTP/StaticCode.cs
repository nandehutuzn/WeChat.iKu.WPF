using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.iKu.HTTP
{
    public class StaticCode
    {
        public static class LoginCode
        {
            /// <summary>
            /// 确认登录
            /// </summary>
            public static string code_LoginSuccess = "200";

            /// <summary>
            /// 扫描成功
            /// </summary>
            public static string code_LoginWait = "201";

            /// <summary>
            /// 登录超时
            /// </summary>
            public static string code_LoginTimeOut = "408";
        }
    }
}
