using Microsoft.AspNetCore.Mvc;

namespace SampledStreamCollector.Controllers
{
    // Class for the controller to get the latest statistics
    [ApiController]
    [Route("[controller]")]
    public class SampledStreamController : ControllerBase
    {
        // Total count of all tweets
        private static ulong TweetCount = 0;

        // Application logger
        private readonly ILogger<SampledStreamController> _logger;

        // Construct the controller object
        public SampledStreamController(ILogger<SampledStreamController> logger)
        {
            // Save the application logger
            _logger = logger;
        }

        // Get the latest statistics
        [HttpGet(Name = "GetSampledStreamStats")]
        public SampledStreamStats Get()
        {
            // Advance the tweet count
            TweetCount += 5;

            // Log the call
            _logger.LogInformation("Returning a tweet count of {TweetCount}", TweetCount);

            // Return the data
            return new SampledStreamStats
            {
                Date = DateTime.Now,
                TotalTweets = TweetCount,
                Status = "Good"
            };
        }
    }
}