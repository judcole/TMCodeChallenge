namespace SampledStreamCommon
{
    /// <summary>
    /// Class to store statistics for the sampled stream
    /// </summary>
    public class SampledStreamStats
    {
        // Size of the list for the top Hashtags
        public const int TopHashtagsSize = 10;

        // Average daily number of tweets received
        public ulong DailyTweets { get; private set; }

        // Average hourly number of tweets received
        public ulong HourlyTweets { get; private set; }

        // Date and time of statistics
        public DateTime LastUpdated { get; private set; }

        // Extra status information
        public string? Status { get; set; }

        // Top 10 Hashtag counts
        public ulong[] TopHashtagCounts { get; private set; } = new ulong[TopHashtagsSize];

        // Top 10 Hashtags
        public string[] TopHashtags { get; private set; } = new string[TopHashtagsSize];

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
        public SampledStreamStats()
        {
            // Set the last updated date and time
            LastUpdated = DateTime.UtcNow;
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
    }
}