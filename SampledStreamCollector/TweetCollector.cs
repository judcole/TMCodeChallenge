using System.ComponentModel;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.RegularExpressions;
using SampledStreamCommon;

namespace SampledStreamCollector
{
    /// <summary>
    /// Class to collect tweet data from the Twitter Sampled Stream API
    /// </summary>
    /// 
    /// Todo: Replace implementation with a .NET Queue Service https://docs.microsoft.com/en-us/dotnet/core/extensions/queue-service
    public class TweetCollector : BackgroundService
    {
        // Regular expression object to match hashtags
        public static readonly Regex HashtagRegex = new(HashtagPattern, RegexOptions.IgnoreCase);

        // Name of bearer token environment variable
        private const string BearerTokenEnvironmentString = "STREAM_BEARER_TOKEN";

        // Message to display and send to the API if the token is missing
        private const string BearerTokenMissingMessage = $"To access the Twitter API please set the {BearerTokenEnvironmentString} environment variable";

        // Todo: This should ideally match international characters as well
        // Regular expression pattern to match hashtags
        private const string HashtagPattern = @"\B#\w*[a-zA-Z]+\w*";

        // URL of Twitter stream API
        private const string TwitterApiUrl = "https://api.twitter.com/2/tweets/sample/stream";

        // Twitter stream API authentication bearer token (read from the environment)
        private readonly string? _bearerToken = Environment.GetEnvironmentVariable(BearerTokenEnvironmentString);

        // Dictionary of all Hashtags and their counts
        private readonly Dictionary<string, ulong> _hashtagDictionary= new();

        // HTTP client for accessing the Twitter API
        private readonly HttpClient _httpClient = new();

        // Options and settings for the JSON Serializer
        private readonly JsonSerializerOptions _jsonOptions = new();

        // Application logger
        private readonly ILogger<TweetCollector> _logger;

        // Shared total statistics
        private readonly SampledStreamStats _stats;

        // Queue of tweet blocks to process
        private readonly IBackgroundQueue<TweetBlock> _tweetQueue;

        // Cancellation token source to manage cancellation of the Twitter API read task
        private CancellationTokenSource? _tweetStreamCancellationTokenSource;

        // Stream reader for accessing the Twitter API
        private StreamReader? _tweetStreamReader;

        /// <summary>
        /// Construct the collector instance
        /// </summary>
        /// <param name="queue">Queue of tweets to process</param>
        /// <param name="stats">Shared total statistics</param>
        /// <param name="logger">Application log file</param>
        public TweetCollector(IBackgroundQueue<TweetBlock> queue, SampledStreamStats stats, ILogger<TweetCollector> logger)
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
                        // Log a message every 10 seconds or so
                        _logger.LogInformation("Worker running at: {time} with {count} tweets queued",
                            time, _tweetQueue.GetCount());

                        // Remember we have done so for this second so we don't log twice
                        messageSeconds = seconds;
                    }

                    if (_tweetQueue.GetCount() > 0)
                    {
                        // Get the next tweet block to process
                        var tweetBlock = _tweetQueue.Dequeue();

                        if (tweetBlock is not null)
                        {
                            _logger.LogInformation("Processing tweet block at: {time}", time);

                            // Process the tweet block
                            ProcessTweetBlockFromQueue(tweetBlock);
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
                    _logger.LogCritical("An error occurred when reading tweets: Exception: {@Exception}", ex);
                    _stats.Status = $"An error occurred when reading tweets: Exception: {ex}";
                }
            }

            _logger.LogInformation("{Type} is now shutting down", nameof(BackgroundWorker));
        }

        /// <summary>
        /// Closes the tweet stream started by <see cref="NextTweetStreamAsync"/>. 
        /// </summary>
        /// <param name="force">If true, the stream will be closed immediately. With falls the thread had to wait for the next keep-alive signal (every 20 seconds)</param>
        private void CancelTweetStream(bool force = true)
        {
            _tweetStreamCancellationTokenSource?.Dispose();

            if (force)
            {
                _tweetStreamReader?.Close();
                _tweetStreamReader?.Dispose();
            }
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

            if (_bearerToken is null)
            {
                // No bearer token so log it and indicate it in the statistics data
                _logger.LogCritical(BearerTokenMissingMessage);
                _stats.Status = BearerTokenMissingMessage;
            }
            else
            {
                // Set up a cancellation token source for the Twitter stream reader
                _tweetStreamCancellationTokenSource = new();

                // Kick of the Twitter stream reader as a separate background task
                _ = Task.Run(async () =>
                {
                    await ReadTweetStreamsAsync(_bearerToken, _tweetStreamCancellationTokenSource);
                }, _tweetStreamCancellationTokenSource.Token);

                // Kick off the main background processing task
                await BackgroundProcessing(stoppingToken);
            }
        }

        /// <summary>
        /// Process an incoming tweet block from the queue
        /// </summary>
        /// <param name="tweetBlock">The tweet block</param>
        /// <returns>True for success</returns>
        private bool ProcessTweetBlockFromQueue(TweetBlock tweetBlock)
        {
            // Assume success
            bool success = true;

            // Prepare counts for the block
            ulong tweetCount = 0;
            ulong hashtagCount = 0;

            try
            {
                // Deserialize the tweet
                Tweet? tweet = JsonSerializer.Deserialize<Tweet>(tweetBlock.Contents, _jsonOptions);

                if ((tweet is not null) && (tweet.data is not null) && (tweet.data.text is not null))
                {
                    // It looks valid so use it
                    tweetCount++;

                    // Look for hash tags
                    var matches = HashtagRegex.Matches(tweet.data.text);

                    // Loop through the results
                    foreach (Match match in matches)
                    {
                        // Get the hashtag without the leading hash
                        var hashtag = match.Value[1..];
                        //_logger.LogInformation("{string}", hashtag);

                        // One more hashtag found
                        hashtagCount++;

                        // Increment the counter for this tag
                        var newCount = _hashtagDictionary.GetValueOrDefault(match.Value, 0U) + 1;
                        _hashtagDictionary[match.Value] = newCount;

                        // Update the list of top hashtags with a specified hashtag and count
                        _stats.UpdateTopHashtags(hashtag, newCount);
                    }
                }

                // Save the new statistics from the block
                _stats.SetBasicFields(_stats.TotalHashtags + hashtagCount, _stats.TotalTweets + tweetCount, _tweetQueue.GetCount());
            }
            catch (Exception ex)
            {
                _logger.LogCritical("An error occurred when processing tweets: Exception: {@Exception}", ex);
                _stats.Status = $"An error occurred when processing tweets: Exception: {ex}";
            }

            _logger.LogInformation("Have now processed {count} tweets", _stats.TotalTweets);

            return success;
        }

        /// <summary>
        /// Read tweets from the tweet stream and queue them
        /// </summary>
        /// <param name="bearerToken">The Twitter API bearer token</param>
        /// <returns>An async task for the tweet reader</returns>
        private async Task ReadTweetStreamsAsync(string bearerToken, CancellationTokenSource cancellationTokenSource)
        {
            // Set up the bearer token
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            // Initiate the connection with the Twitter stream
            var result = await _httpClient.GetAsync(TwitterApiUrl, HttpCompletionOption.ResponseHeadersRead, cancellationTokenSource.Token);

            // Set up a stream reader for it
            _tweetStreamReader = new(await result.Content.ReadAsStreamAsync(cancellationTokenSource.Token));

            try
            {
                // Loop until the end of the stream or cancelled
                while (!_tweetStreamReader.EndOfStream && !cancellationTokenSource.IsCancellationRequested)
                {
                    // Read a line from the Twitter stream
                    var str = await _tweetStreamReader.ReadLineAsync();

                    if (string.IsNullOrWhiteSpace(str))
                    {
                        // It is a keep-alive string so just ignore it
                        continue;
                    }

                    // Create a new block instance and enqueue it
                    _tweetQueue.Enqueue(new TweetBlock(str));
                }
            }
            catch (IOException ex)
            {
                // Check for a connection aborted socket exception
                if (!((ex.InnerException is SocketException se) && (se.SocketErrorCode == SocketError.ConnectionAborted)))
                {
                    // Not a connection aborted socket exception so rethrow it
                    throw;
                }
            }

            // Make sure to cancel the Tweet stream
            CancelTweetStream();
        }

        /// <summary>
        /// Triggered when the background processing should stop
        /// </summary>
        /// <param name="cancellationToken">Token used to indicate a forced stop</param>
        /// <returns>An async task for the cancellation</returns>
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogCritical(
                "{Type} is stopping due to a host shutdown so queued tweets will no longer be processed",
                nameof(BackgroundWorker)
            );

            // Cancel the tweet stream reader
            CancelTweetStream(true);

            // Return the async task
            return base.StopAsync(cancellationToken);
        }
    }
}
