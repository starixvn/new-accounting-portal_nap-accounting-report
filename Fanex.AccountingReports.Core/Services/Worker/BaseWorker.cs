namespace Fanex.AccountingReports.Core.Services.Impl.Worker
{
    using System;
    using System.Threading;
    using Fanex.Logging;

    public class BaseWorker
    {
        private Thread _workerThread;

        public BaseWorker(string threadName)
        {
            _workerThread = new Thread(Run);
            ThreadName = threadName;
        }

        protected string ThreadName { get; set; }

        protected bool IsStop { get; set; }

        public void Start()
        {
            if (_workerThread != null)
            {
                _workerThread.Start();
                IsStop = false;
            }
            else
            {
                Logger.Log.Error(string.Format("Thread {0} was stoped!", ThreadName), new Exception());
            }
        }

        public void Stop()
        {
            IsStop = true;
        }

        protected virtual void Run()
        {
        }
    }
}
