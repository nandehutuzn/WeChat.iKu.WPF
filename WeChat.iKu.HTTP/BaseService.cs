using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Collections;

namespace WeChat.iKu.HTTP
{
    /// <summary>
    /// 访问http服务器类
    /// </summary>
    public class BaseService
    {
        /// <summary>
        /// 访问服务器时的cookies
        /// </summary>
        public static CookieContainer CookiesContainer;

        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)"; 

        /// <summary>
        /// 向服务器发送Request
        /// </summary>
        /// <param name="url">字符串</param>
        /// <param name="method">Get或Post</param>
        /// <param name="body">Post时必须传值</param>
        /// <returns></returns>
        public static byte[] Request(string url, MethodEnum method, string body = "")
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = DefaultUserAgent;
                request.Method = method.ToString();
                if (method == MethodEnum.POST)//post设置body
                {
                    byte[] request_body = Encoding.UTF8.GetBytes(body);
                    request.ContentLength = request_body.Length;

                    Stream request_stream = request.GetRequestStream();
                    request_stream.Write(request_body, 0, request_body.Length);
                }
                if (CookiesContainer == null)
                    CookiesContainer = new CookieContainer();
                request.CookieContainer = CookiesContainer;

                return Response(request);
            }
            catch {
                throw;
            }
        }

        /// <summary>
        /// 返回Response数据
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static byte[] Response(HttpWebRequest request)
        {
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream response_stream = response.GetResponseStream();

                int count = (int)response.ContentLength;
                int offset = 0;
                byte[] buf = new byte[count];
                while (count > 0)
                {
                    int n = response_stream.Read(buf, offset, count);
                    if (n == 0)
                        break;
                    count -= n;
                    offset += n;
                }
                return buf;
            }
            catch {
                throw;
            }
        }

        /// <summary>
        /// 获取所有的Cookie
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        private static List<Cookie> GetAllCookies(CookieContainer cc)
        {
            List<Cookie> lstCookies = new List<Cookie>();
            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                 System.Reflection.BindingFlags.NonPublic |
                  System.Reflection.BindingFlags.GetField |
                   System.Reflection.BindingFlags.Instance,
                   null, cc, new object[] { });

            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                     System.Reflection.BindingFlags.NonPublic |
                      System.Reflection.BindingFlags.GetField |
                       System.Reflection.BindingFlags.Instance,
                       null, pathList, new object[] { });

                foreach (CookieCollection colCoolies in lstCookieCol.Values)
                    foreach (Cookie c in colCoolies)
                        lstCookies.Add(c);
            }

            return lstCookies;
        }

        /// <summary>
        /// 获取指定 cookie
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Cookie GetCookie(string name)
        {
            return GetAllCookies(CookiesContainer).First(o => o.Name == name);
        }
    }
}
