using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SampledStreamApp.Pages
{
    // Class for Index page data model
    public class IndexModel : PageModel
    {
        // Total tweet count
        private static ulong _totalTweets = 0;
        public static ulong TotalTweets { get { return _totalTweets; } }

        // Name of the current day for the heading
        public string? DayName { get; set; }

        // Date and time of the last stats update
        public string? LastUpdated;

        // Application logger
        private readonly ILogger<IndexModel> _logger;

        // Construct the model object
        public IndexModel(ILogger<IndexModel> logger)
        {
            // Save the application logger
            _logger = logger;
        }

        // Construct the page on a GET action
        public void OnGet()
        {
            // Get the name of the current day for the heading
            DayName = DateTime.Now.ToString("dddd");

            // Set some home page values
            LastUpdated = DateTime.Now.ToString("G");
            _totalTweets += 2;

            _logger.LogInformation("Page get on {DayName} with {TotalTweets} tweets", DayName, _totalTweets);
        }
    }
}
