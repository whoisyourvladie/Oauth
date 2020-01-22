using NLog;
using System;
using System.Timers;

namespace SaaS.WinService.Core
{
    public abstract class BaseScheduler
    {
        protected delegate void EmptyEventHandler();
        protected delegate void ErrorEventHandler(Exception exc);

        protected ThreadPool _queue = new ThreadPool(1);
        protected Logger _logger = LogManager.GetCurrentClassLogger();

        protected void Do(Timer timer, EmptyEventHandler handler)
        {
            timer.Stop();
            System.Threading.ThreadPool.QueueUserWorkItem(state =>
            {
                object[] data = (object[])state;
                try
                {
                    handler();
                }
                catch (Exception exc)
                {
                    _logger.Error(exc);
                }
                finally
                {
                    ((Timer)data[0]).Start();
                }
            }, new object[] { timer });
        }

        public abstract void Start();
    }
}
