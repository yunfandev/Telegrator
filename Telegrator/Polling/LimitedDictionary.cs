using System.Collections;
using System.Collections.Concurrent;

namespace Telegrator.Polling
{
    /// <summary>
    /// Represents a dictionayr with limited number of slots, if trying to overflow, will block calling thread until one of slots will be free
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class LimitedDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IDisposable
    {
        private readonly int? _maximum;
        private readonly SemaphoreSlim _semaphore = null!;
        private readonly ConcurrentDictionary<TKey, TValue> _dict = [];

        /// <summary>
        /// Initializes new instance of <see cref="LimitedDictionary{TKey, TValue}"/>
        /// </summary>
        /// <param name="maximum"></param>
        public LimitedDictionary(int? maximum)
        {
            _maximum = maximum;
            if (maximum != null)
            {
                int value = maximum.Value;
                _semaphore = new SemaphoreSlim(value, value);
            }
        }

        /// <summary>
        /// Tries to add new element to dictioanry.
        /// If all slots are occupied, blocks calling thread.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> Add(TKey key, TValue value, CancellationToken cancellationToken)
        {
            if (_semaphore != null)
                await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            return _dict.TryAdd(key, value);
        }

        /// <summary>
        /// Tries to remove element from dictionay.
        /// Frees slot on success.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool Remove(TKey key, out TValue result)
        {
            if (_dict.TryRemove(key, out result))
            {
                _semaphore?.Release(1);
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dict.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => _dict.GetEnumerator();

        /// <inheritdoc/>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _semaphore.Dispose();
        }
    }
}
