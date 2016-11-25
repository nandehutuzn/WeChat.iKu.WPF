using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;

namespace ZN.Dotnet.Tools
{
    public  static class Logger
    {
        /// <summary>
        /// 当日日志路径
        /// </summary>
        private static string _logFile = string.Empty;

        static Logger()
        {
            CreateLogFolder();
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
            if (!File.Exists(_logFile))
                File.CreateText(_logFile);
        }

        public static void Test() 
        {
            using (StreamWriter sW = new StreamWriter(_logFile, true))
            {
                sW.WriteLine("测试");
            }
        }
    }
}
