using System.Collections;
using System.Collections.Concurrent;

namespace Telegrator.Polling
{
    public class LimitedDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IDisposable
    {
        private readonly int? _maximum;
        private readonly SemaphoreSlim _semaphore = null!;
        private readonly ConcurrentDictionary<TKey, TValue> _dict = [];

        public LimitedDictionary(int? maximum)
        {
            _maximum = maximum;
            if (maximum != null)
            {
                int value = maximum.Value;
                _semaphore = new SemaphoreSlim(value, value);
            }
        }

        public async Task<bool> Add(TKey key, TValue value, CancellationToken cancellationToken)
        {
            if (_semaphore != null)
                await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            return _dict.TryAdd(key, value);
        }

        public bool Remove(TKey key, out TValue result)
        {
            if (_dict.TryRemove(key, out result))
            {
                _semaphore?.Release(1);
                return true;
            }

            return false;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dict.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _dict.GetEnumerator();

        /// <inheritdoc/>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _semaphore.Dispose();
        }
    }
}
