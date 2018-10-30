namespace Fanex.AccountingReports.Core.Services.Impl.Worker
{
    using System;
    using System.Threading;
    using Fanex.Logging;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
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

        /// <summary>
        /// Start Worker.
        /// </summary>
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

        /// <summary>
        /// Set Stop Worker.
        /// </summary>
        public void Stop()
        {
            IsStop = true;
        }

        /// <summary>
        /// Run main function.
        /// </summary>
        protected virtual void Run()
        {
        }
    }
}
