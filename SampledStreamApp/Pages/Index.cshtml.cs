using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SampledStreamApp.Pages
{
    public class IndexModel : PageModel
    {
        // Tweet count
        private static ulong _totalTweets = 0;
        public static ulong TotalTweets { get { return _totalTweets; } }

        // Name of the current day for the heading
        public string? DayName { get; set; }

        public string? LastUpdated;

        // Application logger
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            // Save the application logger
            _logger = logger;

            // Set some home page values
        }

        public void OnGet()
        {
            ViewData["Title"] = "Twitter SampledStream Application";
            // Get the name of the current day for the heading
            DayName = DateTime.Now.ToString("dddd");

            LastUpdated = DateTime.Now.ToString("G");
            _totalTweets += 2;

            _logger.LogInformation("Page get on {DayName} with {TotalTweets} tweets", DayName, _totalTweets);
        }
    }
}
// Readonly vars