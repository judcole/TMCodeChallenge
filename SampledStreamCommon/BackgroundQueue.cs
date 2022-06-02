using System.Collections.Concurrent;

namespace SampledStreamCommon
{
    /// <summary>
    /// Class for background queues
    /// </summary>
    /// <typeparam name="T">The type of each queue entry</typeparam>
    public class BackgroundQueue<T> : IBackgroundQueue<T> where T : class
    {
        /// <summary>
        /// The item queue
        /// </summary>
        private readonly ConcurrentQueue<T> _items = new();

        /// <summary>
        /// Get the number of items currently in the queue
        /// </summary>
        public int GetCount()
        { 
            return _items.Count;
        }

        /// <summary>
        /// Schedule an item that needs to be processed
        /// </summary>
        /// <param name="item">Item to be processed</param>
        public void Enqueue(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            _items.Enqueue(item);
        }

        /// <summary>
        /// Try to remove and return the item at the beginning of the queue
        /// </summary>
        /// <returns>An item if found otherwise null</returns>
        public T? Dequeue()
        {
            var success = _items.TryDequeue(out var workItem);

            return success ? workItem : null;
        }
    }
}
