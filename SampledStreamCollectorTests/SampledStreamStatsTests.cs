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
        /// <param name="totalTweets">The count for the count</param>
        /// <param name="elapsedHours">The number of hours to have elapsed</param>
        [Theory]
        [InlineData(0, 0, 0, 0)]
        [InlineData(1, 0, 0, 0)]
        [InlineData(99, 0, 0, 0)]
        [InlineData(0, 1, 0, 0)]
        [InlineData(1, 1, 1, 1)]
        [InlineData(99, 1, 99, 99)]
        [InlineData(0, 12345, 0, 0)]
        [InlineData(1, 12345, 0, 0)]
        [InlineData(98, 12345, 0, 0)]
        [InlineData(9999, 12345, 19, 0)]
        [InlineData(99999, 12345, 194, 8)]
        [InlineData(0, 54321, 0, 0)]
        [InlineData(1, 54321, 0, 0)]
        [InlineData(97, 54321, 0, 0)]
        [InlineData(9999, 54321, 4, 0)]
        [InlineData(99999, 54321, 44, 1)]
        public void SetCalculatedFields_SetValues_ReturnsCorrect(ulong totalTweets, int elapsedHours, ulong expectedDaily, ulong expectedHourly)
        {
            // Create an instance and set the tweet count
            var stats = CreateStatsInstance();
            stats.TotalTweets = totalTweets;

            // Set a start date from the data
            var startDate = stats.LastUpdated.AddHours(-elapsedHours);

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
            stats.TotalTweets.Should().Be(0);
            stats.TweetQueueCount.Should().Be(0);

            // Create an instance with a future date and time
            expectedDate = expectedDate.AddHours(12345);

            stats = new SampledStreamStats(expectedDate);

            // Check the last updated date
            stats.LastUpdated.Date.Should().Be(expectedDate.Date);
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
