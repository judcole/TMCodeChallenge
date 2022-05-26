using Microsoft.AspNetCore.Mvc;

namespace SampledStreamCollector.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampledStreamController : ControllerBase
    {
        // Total count of all tweets
        private static ulong TweetCount = 0;

        // Application logger
        private readonly ILogger<SampledStreamController> _logger;

        public SampledStreamController(ILogger<SampledStreamController> logger)
        {
            _logger = logger;
        }

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