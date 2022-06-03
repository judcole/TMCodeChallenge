namespace SampledStreamCommon
{
    /// <summary>
    /// Class to store statistics for the sampled stream
    /// </summary>
    public class SampledStreamStats
    {
        // Average daily number of tweets received
        public ulong DailyTweets { get; private set; }

        // Average hourly number of tweets received
        public ulong HourlyTweets { get; private set; }

        // Date and time of statistics
        public DateTime LastUpdated { get; private set; }

        // Extra status information
        public string? Status { get; set; }

        // List of top Hashtag counts
        public ulong[] TopHashtagCounts { get; }

        // List of top 10 hashtags
        public string?[] TopHashtags { get; }

        // Size of the list for the top Hashtags
        public int TopHashtagsSize { get; }
        // Total number of hashtags received
        public ulong TotalHashtags { get; private set; }

        // Total number of tweets received
        public ulong TotalTweets { get; private set; }

        // Number of Tweets waiting to be processed in incoming queue
        public int TweetQueueCount { get; private set; }

        // Object to use for simple locking when updating complex fields
        private readonly object _lockObject = new();

        /// <summary>
        /// Construct the SampledStreamStats instance with the current date and time
        /// </summary>
        /// <param name="topHashtagsSize">Size of the list for the top Hashtags</param>
        public SampledStreamStats(int topHashtagsSize)
        {
            // Set the last updated date and time
            LastUpdated = DateTime.UtcNow;

            // Create the top hashtags list
            TopHashtagsSize = topHashtagsSize;
            TopHashtagCounts = new ulong[TopHashtagsSize];
            TopHashtags = new string[TopHashtagsSize];
    }

    /// <summary>
    /// Set new values for the basic fields (concurrent safe)
    /// </summary>
    /// <param name="totalHashtags"></param>
    /// <param name="totalTweets"></param>
    /// <param name="tweetQueueCount"></param>
    public void SetBasicFields(ulong totalHashtags, ulong totalTweets, int tweetQueueCount)
        {
            // Play safe and lock the instance while we update it
            lock (_lockObject)
            {
                TotalHashtags = totalHashtags;
                TotalTweets = totalTweets;
                TweetQueueCount = tweetQueueCount;
            }
        }

        /// <summary>
        /// Calculate and set all calculated fields (concurrent safe)
        /// </summary>
        /// <param name="startTime">The application start time for evaluating the elapsed time</param>
        public void SetCalculatedFields(DateTime startTime)
        {
            // Play safe and lock the instance while we update it
            lock (_lockObject)
            {
                // Update the last updated date and time
                LastUpdated = DateTime.UtcNow;

                // Calculate and set the daily tweet rate with a check for negative durations
                var elapsedTime = LastUpdated.Subtract(startTime);
                var elapsedDays = Math.Max(1, Math.Ceiling(elapsedTime.TotalDays));
                DailyTweets = (ulong)(Math.Ceiling((double)TotalTweets) / elapsedDays);

                // Calculate and set the hourly tweet rate with a check for negative durations
                var elapsedHours = Math.Max(1, Math.Ceiling(elapsedTime.TotalHours));
                HourlyTweets = (ulong)(Math.Ceiling((double)TotalTweets) / elapsedHours);
            }
        }

        /// <summary>
        /// Update the list of top hashtags with a specified hashtag and count
        /// </summary>
        /// <param name="hashtag">Hashtag to add</param>
        /// <param name="count">Count of occurences of the hashtag</param>
        public void UpdateTopHashtags(string hashtag, ulong count)
        {
            // Index of the current hashtag
            int index;

            // Play safe and lock the instance while we update it
            lock (_lockObject)
            {
                // Find if it is already in the list and if so delete it
                for (index = 0; index < TopHashtagsSize; index++)
                    if (TopHashtags[index] == hashtag)
                    {
                        // Found it so shuffle the rest up
                        for (int i = index; i < TopHashtagsSize - 1; i++)
                        {
                            // Shuffle the lower hashtags up a slot
                            TopHashtags[i] = TopHashtags[i + 1];
                            TopHashtagCounts[i] = TopHashtagCounts[i + 1];
                        }
                        // Clear the last slot to make sure it can be overwritten
                        TopHashtags[TopHashtagsSize - 1] = null;
                        TopHashtagCounts[TopHashtagsSize - 1] = 0;

                        // Done with this part
                        break;
                    }

                // Find if and where the hashtag qualifies to be in the list
                for (index = 0; (index < TopHashtagsSize) && (TopHashtagCounts[index] > count); index++) ;

                if (index < TopHashtagsSize)
                {
                    // Found its slot so we need to shuffle the rest down to make room
                    for (int i = TopHashtagsSize - 2; i >= index; i--)
                    {
                        // Shuffle the hashtag down a slot
                        TopHashtags[i + 1] = TopHashtags[i];
                        TopHashtagCounts[i + 1] = TopHashtagCounts[i];
                    }

                    // Set the new hashtag value and count in its correct slot
                    TopHashtags[index] = hashtag;
                    TopHashtagCounts[index] = count;
                }
            }
        }
    }
}