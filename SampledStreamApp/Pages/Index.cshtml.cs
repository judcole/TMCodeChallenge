using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SampledStreamApp.Pages
{
#pragma warning disable IDE1006 // Disable warnings for unconventional field names
    /// <summary>
    /// Simplified version of the statistics class for deserializaing from the API call
    /// </summary>
    /// 
    /// ToDo: Get the JSON deserializer working with the full SampledStreamStats class instead of this simplified version
    public class RawStats
    {
        public ulong dailyTweets { get; set; }
        public ulong hourlyTweets { get; set; }
        public DateTime lastUpdated { get; set; } = DateTime.UtcNow;
        public string? status { get; set; }
        public List<ulong> topHashtagCounts { get; set; } = new();
        public List<string> topHashtags { get; set; } = new();
        public int topHashtagsSize { get; set; }
        public ulong totalHashtags { get; set; }
        public ulong totalTweets { get; set; }
        public int tweetQueueCount { get; set; }
    }
#pragma warning restore IDE1006 // Restore warnings for unconventional field names

    /// <summary>
    /// Statistics Json serialization context class for performance and to allow executable trimming
    /// </summary>
    [JsonSerializable(typeof(RawStats))]
    internal partial class RawStatsJsonContext : JsonSerializerContext
    {
    }

    /// <summary>
    /// Class for Index page data model 
    /// </summary>
    public class IndexModel : PageModel
    {
        // Statistics object to provide values for display
        private RawStats _stats = new();

        // Average daily number of tweets received
        public ulong DailyTweets { get { return _stats.dailyTweets; } }

        // Name of the current day for the heading
        public string? DayName { get; set; }

        // Average hourly number of tweets received
        public ulong HourlyTweets { get { return _stats.hourlyTweets; } }

        // Date and time of the last stats update
        public DateTime LastUpdated { get { return _stats.lastUpdated; } }

        // Extra status information
        public string? Status { get { return _stats.status; } }

        // Top Hashtags
        public ulong TopHashtagCounts(int index) { return _stats.topHashtagCounts[index]; }

        // Top Hashtags
        public string? TopHashtags(int index) { return _stats.topHashtags[index]; }

        // Size of top hashtags list
        public int TopHashtagsSize { get { return _stats.topHashtagsSize; } }

        // Total hashtag count
        public ulong TotalHashtags { get { return _stats.totalHashtags; } }

        // Total tweet count
        public ulong TotalTweets { get { return _stats.totalTweets; } }

        // Number of tweets in tweet queue
        public int TweetQueueCount { get { return _stats.tweetQueueCount; } }

        // URL of the statistics API
        private const string SampledStreamStatsApiUrl = "https://localhost:44355/api/GetSampledStream";

        // HTTP client for accessing the Web API
        private readonly HttpClient _httpClient;

        // Application logger
        private readonly ILogger<IndexModel> _logger;

        /// <summary>
        /// Construct the model instance
        /// </summary>
        /// <param name="httpClient">HTTP client for Web API calls</param>
        /// <param name="logger">Application logger</param>
        public IndexModel(HttpClient httpClient, ILogger<IndexModel> logger)
        {
            // Save the application logger
            _logger = logger;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Async construct and return the page on a GET action 
        /// </summary>
        /// <returns>The configured page</returns>
        public async Task<PageResult> OnGet()
        {
            // Get the name of the current day for the heading
            DayName = DateTime.Now.ToString("dddd");

            _stats = await GetLatestStats();

            _logger.LogInformation("Page get on {DayName} with {TotalTweets} tweets", DayName, _stats.totalTweets);

            return Page();
        }

        /// <summary>
        /// Async get latest statistics from the API
        /// </summary>
        /// <returns></returns>
        private async Task<RawStats> GetLatestStats()
        {
            RawStats? stats = null;
            string? status = null;

            try
            {
                // Set up the client headers
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Call the API
                HttpResponseMessage response = await _httpClient.GetAsync(SampledStreamStatsApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    // Get the JSON response and parse it into a stats object
                    stats = await response.Content.ReadFromJsonAsync<RawStats>(RawStatsJsonContext.Default.RawStats);

                    if (stats is not null)
                    {
                        // Successfully created an object
                        _logger.LogInformation("{string}", stats.ToString());
                    }
                    else
                    {
                        // Indicate the parse failure
                        status = $"Could not deserialize JSON from statistics API at {SampledStreamStatsApiUrl}: {response.Content}";
                    }
                }
                else
                {
                    status = $"Get statistics API at {SampledStreamStatsApiUrl} returned {response}";
                }

            }
            catch (Exception ex)
            {
                if ((ex.InnerException is SocketException se) && (se.SocketErrorCode == SocketError.ConnectionRefused))
                {
                    // A connection refused socket exception so indicate that the API is not available
                    status = $"Get statistics API is not currently available at {SampledStreamStatsApiUrl}";
                }
                else
                {
                    status = $"An error occurred when requesting the statistics: Exception: {ex}";
                }
            }

            if (stats is null)
            {
                // There was an error so create a dummy instance
                stats = new RawStats();
            }

            if (status is not null)
            {
                // There was an error message so save it in the instance
                _logger.LogCritical("{string}", status);
                stats.status = status;
            }

            // Return the statistics
            return stats;
        }
    }
}
