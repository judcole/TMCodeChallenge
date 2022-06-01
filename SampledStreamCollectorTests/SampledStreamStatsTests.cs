using FluentAssertions;
using SampledStreamCommon;

namespace SampledStreamCollectorTests
{
    /// <summary>
    /// Class for SampleStreamStats xUnit tests
    /// </summary>
    public class SampledStreamStatsTests
    {
        /// <summary>
        /// Construct the tests object
        /// </summary>
        public SampledStreamStatsTests()
        {

        }

        /// <summary>
        /// Test that the creation of a new SampledStreamStats object is successful
        /// </summary>
        [Fact()]
        public void SampledStreamStats_Create_ReturnInstance()
        {
            // Create an instance
            var newStats = CreateStatsInstance();

            // Check the default values
            newStats.Should().NotBeNull();
            newStats.DailyTweets.Should().Be(0);
            newStats.HourlyTweets.Should().Be(0);
            newStats.LastUpdated.Date.Should().Be(DateTime.UtcNow.Date);
            newStats.Status.Should().BeNull();
            for (int i = 0; i < SampledStreamStats.TopHashtagsSize; i++)
            {
                newStats.TopHashtags[i].Should().BeNull();
                newStats.TopHashtagCounts[0].Should().Be(0);

            }
            newStats.TotalTweets.Should().Be(0);
            newStats.TweetQueueCount.Should().Be(0);
        }

        /// <summary>
        /// Create an instance of the SampledStreamStats class
        /// </summary>
        /// <returns>The new instance</returns>
        private static SampledStreamStats CreateStatsInstance()
        {
            return new SampledStreamStats();
        }
    }
}
