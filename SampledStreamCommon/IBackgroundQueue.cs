namespace SampledStreamCommon
{
    /// <summary>
    /// Interface for background queues
    /// </summary>
    /// <typeparam name="T">The type of each queue entry</typeparam>
    public interface IBackgroundQueue<T>
    {
        /// <summary>
        /// Get the number of items currently in the queue
        /// </summary>
        int GetCount();

        /// <summary>
        /// Schedule an item that needs to be processed.
        /// </summary>
        /// <param name="item">Item to be processed</param>
        void Enqueue(T item);

        /// <summary>
        /// Try to remove and return the item at the beginning of the queue.
        /// </summary>
        /// <returns>An item if found otherwise null</returns>
        T? Dequeue();
    }
}
