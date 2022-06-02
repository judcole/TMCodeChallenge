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

        // Application logger
        private readonly ILogger<SampledStreamController> _logger;

        // Shared total statistics
        private readonly SampledStreamStats _stats;

        /// <summary>
        /// Construct the controller instance
        /// </summary>
        /// <param name="stats">Shared total statistics</param>
        /// <param name="logger"></param>
        public SampledStreamController(SampledStreamStats stats, ILogger<SampledStreamController> logger)
        {
            // Save the parameters
            _logger = logger;
            _stats = stats;
        }

        /// <summary>
        /// Get the latest statistics
        /// </summary>
        /// <returns>Statistics object containing the latest stats</returns>
        [HttpGet(Name = "GetSampledStreamStats")]
        public async Task<ActionResult<SampledStreamStats>> GetStats()
        {
            // Log the call
            _logger.LogInformation("Returning a tweet count of {count}", _stats.TotalTweets);

            var stats = await Task.Run(() =>
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

        private SampledStreamStats GetSampledStreamStats()
        {
            // Advance some counts for testing for now
            _stats.TotalHashtags += 10;
            _stats.TotalTweets += 5;

            // Calculate and set all calculated fields
            _stats.SetCalculatedFields(s_startTime);

            // Return the stats data
            return _stats;
        }
    }
}