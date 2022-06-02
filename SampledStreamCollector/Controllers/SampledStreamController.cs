using Microsoft.AspNetCore.Mvc;
using SampledStreamCommon;

namespace SampledStreamCollector.Controllers
{
    /// <summary>
    /// Class for the controller to get the latest statistics
    /// </summary>
    [ApiController]
    [Route("api/Get[controller]")]
    public class SampledStreamController : ControllerBase
    {
        // Application start time
        private static readonly DateTime s_startTime = DateTime.UtcNow;

        // Total count of all tweets
        private static ulong s_tweetCount = 0;

        // Application logger
        private readonly ILogger<SampledStreamController> _logger;

        /// <summary>
        /// Construct the controller instance
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
        public async Task<ActionResult<SampledStreamStats>> GetStats()
        {
            // Advance the tweet count
            s_tweetCount += 5;

            // Log the call
            _logger.LogInformation("Returning a tweet count of {s_tweetCount}", s_tweetCount);

            SampledStreamStats? stats = null;

            stats = await Task.Run(() =>
                // Get the stats data
                GetSampledStreamStats()
            );

            if (stats is null)
            {
                // Something went wrong so return a Not Found status code
                return NotFound();
            }
            else
            {
                // Return the stats data with a 200 (Ok)
                return stats;
            }
        }

        private static SampledStreamStats GetSampledStreamStats()
        {
            // Create the stats data
            var stats = new SampledStreamStats
            {
                Status = "Good",
                TotalTweets = s_tweetCount
            };

            // Calculate and set all calculated fields
            stats.SetCalculatedFields(s_startTime);

            // Return the stats data
            return stats;
        }
    }
}