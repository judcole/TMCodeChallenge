using Microsoft.AspNetCore.Mvc;
using SampledStreamCommon;

namespace SampledStreamCollector.Controllers
{
    /// <summary>
    /// Class for the controller to get the latest statistics
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class SampledStreamController : ControllerBase
    {
        // Total count of all tweets
        private static ulong s_tweetCount = 0;

        // Application logger
        private readonly ILogger<SampledStreamController> _logger;

        /// <summary>
        /// Construct the controller object
        /// </summary>
        /// <param name="logger"></param>
        public SampledStreamController(ILogger<SampledStreamController> logger)
        {
            // Save the application logger
            _logger = logger;
        }

        /// <summary>
        /// Get the latest statistics
        /// </summary>
        /// <returns>Statistics object containing the latest stats</returns>
        [HttpGet(Name = "GetSampledStreamStats")]
        public SampledStreamStats Get()
        {
            // Advance the tweet count
            s_tweetCount += 5;

            // Log the call
            _logger.LogInformation("Returning a tweet count of {s_tweetCount}", s_tweetCount);

            // Return the data
            return new SampledStreamStats
            {
                Status = "Good",
                TotalTweets = s_tweetCount
            };
        }
    }
}