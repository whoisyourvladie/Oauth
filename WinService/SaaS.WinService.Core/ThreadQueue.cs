using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SaaS.WinService.Core
{
    public class ThreadPool
    {
        internal class ThreadItem
        {
            private Delegate _task;
            private object[] _params;

            public ThreadItem(Delegate task, object[] @params)
            {
                _task = task;
                _params = @params;
            }

            internal void Invoke()
            {
                if (!object.Equals(_task, null))
                    _task.DynamicInvoke(_params);
            }
        }

        internal class ThreadCollection : List<ThreadItem>
        {

        }

        internal class ThreadQueue : Queue<ThreadItem>
        {

        }

        private readonly ThreadCollection _threads = new ThreadCollection();
        private readonly ThreadQueue _queue = new ThreadQueue();


        private readonly object addlock = new object();
        private readonly object checklock = new object();
        public event EventHandler Completed;
        private bool RaiseCompleteEventIfQueueEmpty = false;

        private int ThreadsMaxCount;

        public ThreadPool(int threadsMaxCount)
        {
            ThreadsMaxCount = threadsMaxCount;
        }
        public void Start()
        {
            CheckQueue();
        }
        public void AddTask(Delegate task, object[] @params)
        {
            lock (addlock)
            {
                ThreadItem item = new ThreadItem(task, @params);

                if (IsFull)
                    _queue.Enqueue(item);
                else
                    StartThread(item);
            }
        }

        private void StartThread(ThreadItem task)
        {
            _threads.Add(task);
            BackgroundWorker thread = new BackgroundWorker();

            thread.DoWork += delegate
            {
                task.Invoke();
            };
            thread.RunWorkerCompleted += delegate
            {
                _threads.Remove(task);

                CheckQueue();

                if (_queue.Count == 0 && _threads.Count == 0 && RaiseCompleteEventIfQueueEmpty)
                    OnCompleted();

            };
            thread.RunWorkerAsync();
        }
        private void CheckQueue()
        {
            lock (checklock)
            {
                while (_queue.Count > 0 && _threads.Count < ThreadsMaxCount)
                    StartThread(_queue.Dequeue());

                if (_queue.Count == 0 && _threads.Count == 0 && RaiseCompleteEventIfQueueEmpty)
                    OnCompleted();
            }
        }
        protected void OnCompleted()
        {
            if (Completed != null)
                Completed(this, null);
        }
        public bool IsFull
        {
            get { return _threads.Count == ThreadsMaxCount; }
        }
    }
}
