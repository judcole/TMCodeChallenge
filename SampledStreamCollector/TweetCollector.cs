using System.ComponentModel;
using SampledStreamCommon;

// Todo: Replace implementation with a .NET Queue Service https://docs.microsoft.com/en-us/dotnet/core/extensions/queue-service

namespace SampledStreamCollector
{
    /// <summary>
    /// Class to collect tweet data from the Twitter Sampled Stream API
    /// </summary>
    public class TweetCollector : BackgroundService
    {
        private const string BearerTokenEnvironmentString = "STREAM_BEARER_TOKEN";
        private const string BearerTokenMissingMessage = $"To access the Twitter API please set the {BearerTokenEnvironmentString} environment variable";

        private static readonly string? s_bearerToken = Environment.GetEnvironmentVariable(BearerTokenEnvironmentString);

        // Application logger
        private readonly ILogger<TweetCollector> _logger;

        // Shared total statistics
        private readonly SampledStreamStats _stats;

        // Queue of tweets to process
        private readonly IBackgroundQueue<Tweet> _tweetQueue;

        /// <summary>
        /// Construct the collector instance
        /// </summary>
        /// <param name="queue">Queue of tweets to process</param>
        /// <param name="stats">Shared total statistics</param>
        /// <param name="logger">Application log file</param>
        public TweetCollector(IBackgroundQueue<Tweet> queue, SampledStreamStats stats, ILogger<TweetCollector> logger)
        {
            // Save the parameters
            _logger = logger;
            _stats = stats;
            _tweetQueue = queue;
        }
        /// <summary>
        /// The background processor of queued Tweets
        /// </summary>
        /// <param name="stoppingToken">Token used to check for a forced stop</param>
        /// <returns>An async task for the background processor</returns>
        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            _logger.LogInformation("{Type} is now running in the background", nameof(BackgroundWorker));

            int messageSeconds = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Get the current time for various calculatins
                    var time = DateTimeOffset.UtcNow;
                    var seconds = time.Second;
                    
                    if ((seconds % 10 == 0) && (seconds != messageSeconds))
                    {
                        // Log a message every 10 seconds or so and remember we have done so for this second
                        _logger.LogInformation("Worker running at: {time} with {count} tweets queued",
                            time, _tweetQueue.GetCount());
                        messageSeconds = seconds;
                    }

                    if (_tweetQueue.GetCount() > 0)
                    {
                        // Get the next tweet to process
                        var tweet = _tweetQueue.Dequeue();

                        if (tweet is not null)
                        {
                            _logger.LogInformation("Processing tweet at: {time}", time);

                            // Process the tweet
                            ProcessTweet(tweet);
                        }
                    }
                    else
                    {
                        // There are no more tweets to process so wait a little while
                        await Task.Delay(500, stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical("An error occurred when processing tweets: Exception: {@Exception}", ex);
                    _stats.Status = $"An error occurred when processing tweets: Exception: {ex}";
                }
            }

            _logger.LogInformation("{Type} is now shutting down", nameof(BackgroundWorker));
        }

        /// <summary>
        /// Asynchronously execute the body of the background service
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns>An async task for the background service</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Await right away so Host Startup can continue.
            await Task.Delay(10, stoppingToken).ConfigureAwait(false);

            if (s_bearerToken is null)
            {
                // No bearer token so log it and indicate it in the statistics data
                _logger.LogCritical(BearerTokenMissingMessage);
                _stats.Status = BearerTokenMissingMessage;
            }
            else
            {
                // Kick off the background processing task
                await BackgroundProcessing(stoppingToken);
            }
        }

        /// <summary>
        /// Process an incoming tweet
        /// </summary>
        /// <param name="tweet">The tweet</param>
        /// <returns>True for success</returns>
        private bool ProcessTweet(Tweet tweet)
        {
            bool success = true;

            _stats.SetBasicFields(_stats.TotalHashtags + 2, _stats.TotalTweets + 1, _tweetQueue.GetCount());

            _logger.LogInformation("Processing tweet number {count}", _stats.TotalTweets);

            return success;
        }

        /// <summary>
        /// Triggered when the background processing should stop
        /// </summary>
        /// <param name="cancellationToken">Token used to indicate a forced stop</param>
        /// <returns>An async task for the </returns>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogCritical(
                "{Type} is stopping due to a host shutdown so queued tweets will no longer be processed",
                nameof(BackgroundWorker)
            );

            return base.StopAsync(cancellationToken);
        }
    }
}
