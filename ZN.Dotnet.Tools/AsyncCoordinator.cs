using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ZN.Dotnet.Tools
{
    internal sealed class AsyncCoordinator
    {
        private int _opCount = 1;//AllBegun内部调用JustEnded来递减它
        private int _statusReported = 0;//0=false, 1=true
        private Action<CoordinationStatus> _callback;
        private Timer _timer;

        /// <summary>
        /// 该方法必须在发起一个操作之前调用
        /// </summary>
        /// <param name="opsToAdd"></param>
        public void AboutToBegin(int opsToAdd = 1)
        {
            Interlocked.Add(ref _opCount, opsToAdd);
        }

        /// <summary>
        /// 该方法必须在处理好一个操作的结果之后调用
        /// </summary>
        public void JustEnded()
        {
            if (Interlocked.Decrement(ref _opCount) == 0)
                ReportStatus(CoordinationStatus.AllDone);
        }

        /// <summary>
        /// 该方法必须在发起所以操作之后调用
        /// </summary>
        /// <param name="callback"></param>
        public void AllBegun(Action<CoordinationStatus> callback)
        {
            int timeout = Timeout.Infinite;
            _callback = callback;
            if (timeout != Timeout.Infinite)
                _timer = new Timer(TimeExprider, null, timeout, Timeout.Infinite);
            JustEnded();
        }

        private void TimeExprider(object o) { ReportStatus(CoordinationStatus.TimeOut); }
        public void Cancel() { ReportStatus(CoordinationStatus.Cancel); }

        private void ReportStatus(CoordinationStatus status)
        {//如果报告从未报告过，就报告它，否则忽略它
            if (Interlocked.Exchange(ref _statusReported, 1) == 0)
                _callback(status);
        }
    }
}
