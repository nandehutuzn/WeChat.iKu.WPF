using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WeChat.iKu.Tools.Heplers;

namespace WeChat.iKu.HTTP
{
    public class LoginService
    {
        public static string Pass_Ticket = "";
        public static string SKey = "";
        private static string session_id = null;

        /// <summary>
        /// 获取登录的二维码
        /// </summary>
        /// <returns></returns>
        public ImageSource GetQRCode()
        {
            //请求UUID
            byte[] bytes = BaseService.Request(StaticUrl.Url_GetUUID + TimeHelper.GetTimeStamp(), MethodEnum.GET);
            //得到session
            session_id = Encoding.UTF8.GetString(bytes).Split(new string[] { "\"" }, StringSplitOptions.None)[1];
            //请求二维码
            bytes = BaseService.Request(StaticUrl.Url_GetQrCode + session_id, MethodEnum.GET);
            return ImageHelper.MemoryToImageSource(new MemoryStream(bytes));
        }

        /// <summary>
        /// 登录扫描检查
        /// </summary>
        /// <returns></returns>
        public object LoginCheck()
        {
            if (session_id == null)
                return null;
            //查看是否扫码登录了
            byte[] bytes = BaseService.Request(StaticUrl.Url_WaitLogin + session_id + "&tip=0&r=" + TimeHelper.GetTimeStamp_TakeBack() + "&_=" + TimeHelper.GetTimeStamp(),
                 MethodEnum.GET);
            string login_result = Encoding.UTF8.GetString(bytes);

            if (login_result.Contains("=" + StaticCode.LoginCode.code_LoginSuccess))
            {
                string login_redirect_url = login_result.Split(new string[] { "\"" }, StringSplitOptions.None)[1];
                return login_redirect_url;
            }
            else if (login_result.Contains("=" + StaticCode.LoginCode.code_LoginWait))
            {
                string base64_image = login_result.Split(new string[] { "\'" }, StringSplitOptions.None)[1].Split(',')[1];
                byte[] base64_image_bytes = Convert.FromBase64String(base64_image);
                MemoryStream memoryStream = new MemoryStream(base64_image_bytes);
                //转成图片
                return ImageHelper.MemoryToImageSource(memoryStream);
            }
            else if (login_result.Contains("=" + StaticCode.LoginCode.code_LoginTimeOut))
            {//登录超时  应设置 session_id = null
                return 408;
            }
            else
                return null;
        }

        /// <summary>
        /// 获取sid uid 结果放在cookie中
        /// </summary>
        /// <param name="login_redirect"></param>
        public void GetSidUid(string login_redirect)
        {
            byte[] bytes = BaseService.Request(login_redirect + StaticUrl.Url_redirect_ext, MethodEnum.GET);
            string pass_ticket = Encoding.UTF8.GetString(bytes);
            Pass_Ticket = pass_ticket.Split(new string[] { "pass_ticket" }, StringSplitOptions.None)[1].TrimStart('>').TrimEnd('<', '/');
            SKey = pass_ticket.Split(new string[] { "skey" }, StringSplitOptions.None)[1].TrimStart('>').TrimEnd('<', '/');
        }
    }
}
