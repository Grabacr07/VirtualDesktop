using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsDesktop
{
    internal class SequentialTaskQueue
    {
        private readonly SemaphoreSlim _semaphore;
        public SequentialTaskQueue()
        {
            this._semaphore = new SemaphoreSlim(1);
        }

        public async Task<T> Enqueue<T>(Func<Task<T>> taskGenerator)
        {
            await this._semaphore.WaitAsync();
            try
            {
                return await taskGenerator();
            }
            finally
            {
                this._semaphore.Release();
            }
        }

        public async Task Enqueue(Func<Task> taskGenerator)
        {
            await this._semaphore.WaitAsync();
            try
            {
                await taskGenerator();
            }
            finally
            {
                this._semaphore.Release();
            }
        }
    }
}
