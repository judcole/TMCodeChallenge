namespace SampledStreamCollector
{
    // Class to store statistics for the sampled stream
    public class SampledStreamStats
    {
        // Date and time of statistics
        public DateTime Date { get; set; }

        // Total number of tweets received
        public ulong TotalTweets { get; set; }

        // Extra status information
        public string? Status { get; set; }
    }
}