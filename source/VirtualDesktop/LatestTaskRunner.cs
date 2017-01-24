using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsDesktop
{
    internal class LatestTaskRunner
    {
        private Task _latestTask;

        public LatestTaskRunner()
        {
        }

        public Task Set(Func<Task> taskGenerator, Task waitForThisTaskFirst)
        {
            lock (this)
            {
                var setTask = taskGenerator();
                this._latestTask = setTask;

                return Task.Factory.StartNew(() =>
                {
                    waitForThisTaskFirst.Wait();

                    if (this._latestTask == setTask)
                    {
                        this._latestTask.Start();
                        this._latestTask.Wait();
                    }
                });
            }
        }
    }
}
