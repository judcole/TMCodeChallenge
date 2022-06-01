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
        public DateTime LastUpdated { get; set; }

        // Extra status information
        public string? Status { get; set; }

        // Top 10 Hashtag counts
        public ulong[] TopHashtagCounts { get; set; } = new ulong[TopHashtagsSize];

        // Top 10 Hashtags
        public string[] TopHashtags { get; set; } = new string[TopHashtagsSize];

        // Total number of tweets received
        public ulong TotalTweets { get; set; }

        // Number of Tweets waiting to be processed in incoming queue
        public ulong TweetQueueCount { get; set; }

        /// <summary>
        /// Construct the SampledStreamStats instance defaulting to the current date and time
        /// </summary>
        public SampledStreamStats() : this(DateTime.UtcNow) { }

        /// <summary>
        /// Construct the SampledStreamStats instance with a specified date and time
        /// </summary>
        /// <param name="lastUpdated">Date and time of last update</param>
        public SampledStreamStats(DateTime lastUpdated)
        {
            // Use the provided last updated date and time
            LastUpdated = lastUpdated;
        }

        /// <summary>
        /// Calculate and set all calculated fields
        /// </summary>
        /// <param name="startTime">The application start time for evaluating the elapsed time</param>
        public void SetCalculatedFields(DateTime startTime)
        {
            // Calculate and set the daily tweet rate
            var elapsedTime = LastUpdated.Subtract(startTime);
            var elapsedDays = Math.Ceiling(elapsedTime.TotalDays);
            DailyTweets = (ulong)(Math.Ceiling((double)TotalTweets) / elapsedDays);

            // Calculate and set the hourly tweet rate
            var elapsedHours = Math.Ceiling(elapsedTime.TotalHours);
            HourlyTweets = (ulong)(Math.Ceiling((double)TotalTweets) / elapsedHours);
        }
    }
}