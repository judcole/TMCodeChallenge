namespace SampledStreamCommon
{
    // Class to store statistics for the sampled stream
    public class SampledStreamStats
    {
        // Size of the list for the top Hashtags
        public const int TopHashtagsSize = 10;

        // Average daily number of tweets received
        public ulong DailyTweets { get; set; }

        // Average hourly number of tweets received
        public ulong HourlyTweets { get; set; }

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
    }
}