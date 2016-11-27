using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace ZN.Dotnet.Tools
{
    public class Logger
    {
        /// <summary>
        /// 当日日志路径
        /// </summary>
        private static string _logFile = string.Empty;
        private static int _resourceInUse;  //0=false, 1=true
        private static Logger _instance;
        private static object _syncObj = new object();
        private Logger() { }
        public static Logger Instance {
            get {
                if (_instance == null)
                {
                    lock (_syncObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new Logger();
                            CreateLogFolder();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 创建日志文件夹、文件
        /// </summary>
        private static void CreateLogFolder()
        {
            string curDir = Environment.CurrentDirectory;
            string dir = Path.Combine(curDir, "Log");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            string year = DateTime.Now.Year.ToString();
            string yearDir = Path.Combine(dir, year);
            if (!Directory.Exists(yearDir))
                Directory.CreateDirectory(yearDir);

            string month = DateTime.Now.Month.ToString();
            string monthDir = Path.Combine(yearDir, month);
            if (!Directory.Exists(monthDir))
                Directory.CreateDirectory(monthDir);

            string day = DateTime.Now.ToString("yyyy-MM-dd");
            _logFile = Path.Combine(monthDir, string.Format("{0}.txt", day));
        }

        private void EnterWrite(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                string logMsg = string.Format("{0}:{1}:{2}:{3}  {4}", DateTime.Now.Hour.ToString(),
                    DateTime.Now.Minute.ToString(), DateTime.Now.Second.ToString(), DateTime.Now.Millisecond.ToString(), message);
                while (true)
                {
                    if (Interlocked.Exchange(ref _resourceInUse, 1) == 0)
                    {
                        using (StreamWriter sWrite = new StreamWriter(_logFile, true))
                        {
                            sWrite.WriteLine(logMsg);
                        }
                        return;
                    }
                }
            }
        }

        private void Leave()
        {
            Volatile.Write(ref _resourceInUse, 0);//将资源标记为未使用
        }

        public  void Exception(string exceptionMsg)
        {
            EnterWrite(exceptionMsg);
            Leave();
        }

        public void Exception(Exception ex)
        {
            Exception(string.Format("Exception Message: {0} \r\n Position: {1}", ex.Message, ex.StackTrace));
        }
    }
}
