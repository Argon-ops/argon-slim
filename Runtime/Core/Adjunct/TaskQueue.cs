using UnityEngine;
using UnityEditor;
using System.Threading;
using System.Threading.Tasks;

namespace DuksGames.Argon.Core
{
    public class TaskQueue
    {
        SemaphoreSlim _semaphore;
        public TaskQueue()
        {
            _semaphore = new SemaphoreSlim(1);
        }

        public async Task<T> Enqueue<T>(System.Func<Task<T>> taskGenerator)
        {
            await _semaphore.WaitAsync();
            try
            {
                return await taskGenerator();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task Enqueue(System.Func<Task> taskGenerator)
        {
            await _semaphore.WaitAsync();
            try
            {
                await taskGenerator();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}