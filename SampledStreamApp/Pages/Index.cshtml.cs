using Microsoft.AspNetCore.Mvc.RazorPages;
using SampledStreamCommon;

namespace SampledStreamApp.Pages
{
    // Class for Index page data model
    public class IndexModel : PageModel
    {
        // Shared statistics object to provide values for display
        private static readonly SampledStreamStats s_stats = new();

        // Average daily number of tweets received
        public ulong DailyTweets { get { return s_stats.DailyTweets; } }

        // Name of the current day for the heading
        public string? DayName { get; set; }

        // Average hourly number of tweets received
        public ulong HourlyTweets { get { return s_stats.HourlyTweets; } }

        // Date and time of the last stats update
        public DateTime LastUpdated { get { return s_stats.LastUpdated; } }

        // Extra status information
        public string? Status { get { return s_stats.Status; } }

        // Top Hashtags
        public ulong TopHashtagCounts (int index) { return s_stats.TopHashtagCounts[index]; }

        // Top Hashtags
        public string TopHashtags(int index) { return s_stats.TopHashtags[index]; }

        // Total hashtag count
        public ulong TotalHashtags { get { return s_stats.TotalHashtags; } }

        // Total tweet count
        public ulong TotalTweets { get { return s_stats.TotalTweets; } }

        // Application logger
        private readonly ILogger<IndexModel> _logger;

        // Construct the model instance
        public IndexModel(ILogger<IndexModel> logger)
        {
            // Save the application logger
            _logger = logger;
        }

        // Construct and return the page on a GET action
        public PageResult OnGet()
        {
            // Get the name of the current day for the heading
            DayName = DateTime.Now.ToString("dddd");

            // Set some home page values just for now
            s_stats.Status = "Good";
            s_stats.TotalHashtags += 200;
            s_stats.TotalTweets += 100;
            s_stats.SetCalculatedFields(DateTime.UtcNow.AddMinutes(-1));

            _logger.LogInformation("Page get on {DayName} with {TotalTweets} tweets", DayName, s_stats.TotalTweets);

            return Page();
        }
    }
}
