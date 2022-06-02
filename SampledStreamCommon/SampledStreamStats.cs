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

        // Total number of hashtags received
        public ulong TotalHashtags { get; set; }

        // Total number of tweets received
        public ulong TotalTweets { get; set; }

        // Number of Tweets waiting to be processed in incoming queue
        public ulong TweetQueueCount { get; set; }

        /// <summary>
        /// Construct the SampledStreamStats instance with the current date and time
        /// </summary>
        public SampledStreamStats()
        {
            // Set the last updated date and time
            LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        /// Calculate and set all calculated fields
        /// </summary>
        /// <param name="startTime">The application start time for evaluating the elapsed time</param>
        public void SetCalculatedFields(DateTime startTime)
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