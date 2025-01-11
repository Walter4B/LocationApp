using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationApp.Services.Services
{
    public class RequestQueueService
    {
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new();

        public async Task<T> EnqueueAsync<T>(string key, Func<Task<T>> action)
        {
            var semaphore = _semaphores.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

            try
            {
                await semaphore.WaitAsync();
                return await action();
            }
            finally
            {
                semaphore.Release();

                // Reset semaphore after use
                if (semaphore.CurrentCount == 1)
                {
                    _semaphores.TryRemove(key, out _);
                }
            }
        }
    }
}
