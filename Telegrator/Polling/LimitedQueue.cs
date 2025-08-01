using System.Collections.Concurrent;

namespace Telegrator.Polling
{
    public class LimitedQueue<T>
    {
        private readonly int? _maximum;
        private readonly ConcurrentQueue<T> _queue = [];
        private readonly SemaphoreSlim _semaphore = null!;

        public LimitedQueue(int? maximum)
        {
            _maximum = maximum;
            if (maximum != null)
            {
                int value = maximum.Value;
                _semaphore = new SemaphoreSlim(value, value);
            }
        }

        public async Task Enqueue(T item, CancellationToken cancellationToken)
        {
            if (_maximum.HasValue)
                await _semaphore.WaitAsync(cancellationToken);

            _queue.Enqueue(item);
        }

        public bool Dequeue(out T result)
        {
            if (_queue.TryDequeue(out result))
            {
                if (_maximum.HasValue)
                    _semaphore?.Release(1);

                return true;
            }

            return false;
        }
    }
}
