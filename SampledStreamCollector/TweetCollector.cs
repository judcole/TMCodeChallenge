using System.ComponentModel;
using SampledStreamCommon;

namespace SampledStreamCollector
{
    /// <summary>
    /// Class to collect tweet data from the Twitter Sampled Stream API
    /// </summary>
    public class TweetCollector : BackgroundService
    {
        private const string BearerTokenEnvironmentString = "STREAM_BEARER_TOKEN";
        private static readonly string? s_bearerToken = Environment.GetEnvironmentVariable(BearerTokenEnvironmentString);

        private readonly IBackgroundQueue<Tweet> _tweetQueue;

        // Application logger
        private readonly ILogger<TweetCollector> _logger;

        /// <summary>
        /// Construct the collector instance
        /// </summary>
        /// <param name="logger"></param>
        public TweetCollector(IBackgroundQueue<Tweet> queue, ILogger<TweetCollector> logger)
        {
            // Save the tweet queue and the application logger
            _tweetQueue = queue;
            _logger = logger;

            if (s_bearerToken is null)
            {
                // No bearer token so give up now
                //throw new Exception($"Please make sure you set the {BearerTokenEnvironmentString} environment variable");
            }
        }

        /// <summary>
        /// Asynchrnously execute the body of the background service
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time} with {count} queued", DateTimeOffset.UtcNow, _tweetQueue.GetCount());

                // Wait a while
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
