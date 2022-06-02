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
        /// Construct the tests instance
        /// </summary>
        public SampledStreamStatsTests()
        {

        }

        /// <summary>
        /// Test that setting the Tweet Queue count is successful
        /// </summary>
        /// <param name="totalTweets">Total number of tweets</param>
        /// <param name="elapsedHours">Number of hours to have elapsed</param>
        /// <param name="expectedDaily">Expected daily tweet rate</param>
        /// <param name="expectedHourly">Expectde hourly tweet rate</param>
        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(1, 0, 1, 1)]
        [InlineData(99, 0, 99, 99)]
        [InlineData(0, 1, 0, 0)]
        [InlineData(1, 1, 1, 1)]
        [InlineData(99, 1, 99, 99)]
        [InlineData(0, 12345, 0, 0)]
        [InlineData(1, 12345, 0, 0)]
        [InlineData(98, 12345, 0, 0)]
        [InlineData(9999, 12345, 19, 0)]
        [InlineData(99999, 12345, 194, 8)]
        [InlineData(2000000, 12345, 3883, 162)]
        [InlineData(0, 54321, 0, 0)]
        [InlineData(1, 54321, 0, 0)]
        [InlineData(97, 54321, 0, 0)]
        [InlineData(9999, 54321, 4, 0)]
        [InlineData(99999, 54321, 44, 1)]
        [InlineData(2000000, 54321, 883, 36)]
        public void SetCalculatedFields_SetValues_ReturnsCorrect(ulong totalTweets, int elapsedHours, ulong expectedDaily, ulong expectedHourly)
        {
            // Create an instance and set the tweet count
            var stats = CreateStatsInstance();
            stats.TotalTweets = totalTweets;

            // Set a start date from the data allowing a margin of a second for the test
            var startDate = stats.LastUpdated.AddHours(-elapsedHours).AddSeconds(1);

            // Calculate and set the calculated fields
            stats.SetCalculatedFields(startDate);

            // Check the result
            stats.DailyTweets.Should().Be(expectedDaily);
            stats.HourlyTweets.Should().Be(expectedHourly);
        }

        /// <summary>
        /// Test that the creation of a new SampledStreamStats object is successful
        /// </summary>
        [Fact]
        public void SampledStreamStats_Create_ReturnInstance()
        {
            // Save the current time as the expected default time
            var expectedDate = DateTime.UtcNow;

            // Create an instance with the default date and time
            var stats = CreateStatsInstance();

            // Check the default values
            stats.Should().NotBeNull();
            stats.DailyTweets.Should().Be(0);
            stats.HourlyTweets.Should().Be(0);
            stats.LastUpdated.Date.Should().Be(expectedDate.Date);
            stats.Status.Should().BeNull();
            for (int i = 0; i < SampledStreamStats.TopHashtagsSize; i++)
            {
                stats.TopHashtags[i].Should().BeNull();
                stats.TopHashtagCounts[0].Should().Be(0);
            }
            stats.TotalHashtags.Should().Be(0);
            stats.TotalTweets.Should().Be(0);
            stats.TweetQueueCount.Should().Be(0);
        }

        /// <summary>
        /// Test that setting the Tweet Queue count is successful
        /// </summary>
        /// <param name="count">The value for the count</param>
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(99)]
        public void SampledStreamStats_SetQueueCount_Successful(ulong count)
        {
            // Create an instance
            var stats = CreateStatsInstance();

            // Set the Queue count
            stats.TweetQueueCount = count;

            // Check the result
            stats.TweetQueueCount.Should().Be(count);
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
