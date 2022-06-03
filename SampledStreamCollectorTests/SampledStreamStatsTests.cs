using FluentAssertions;
using SampledStreamCommon;

namespace SampledStreamCollectorTests
{
    /// <summary>
    /// Class for SampleStreamStats xUnit tests
    /// </summary>
    public class SampledStreamStatsTests
    {
        // Some test values
        private const string hashtag1 = "abc";
        private const string hashtag2 = "abc1";
        private const string hashtag3 = "abc12";
        private const string hashtag4 = "xyz";
        private const string hashtag5 = "alonghashtag";

        /// <summary>
        /// Construct the tests instance
        /// </summary>
        public SampledStreamStatsTests()
        {

        }

        /// <summary>
        /// Set new values for the basic fields (concurrent safe)
        /// </summary>
        /// <param name="totalHashtags"></param>
        /// <param name="totalTweets"></param>
        /// <param name="tweetQueueCount"></param>
        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(1, 0, 0)]
        [InlineData(0, 1, 0)]
        [InlineData(0, 0, 1)]
        [InlineData(10, 20, 0)]
        [InlineData(10, 0, 20)]
        [InlineData(20, 30, 40)]
        public void SetBasicFields_SetValues_ReturnsCorrect(ulong totalHashtags, ulong totalTweets, int tweetQueueCount)
        {
            // Create an instance
            var stats = CreateStatsInstance(1);

            // Set the basic fields
            stats.SetBasicFields(totalHashtags, totalTweets, tweetQueueCount);

            // Check the result
            stats.TotalHashtags.Should().Be(totalHashtags);
            stats.TotalTweets.Should().Be(totalTweets);
            stats.TweetQueueCount.Should().Be(tweetQueueCount);
        }

        /// <summary>
        /// Test that calculating the calculated fields from the total tweet count is successful
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
            // Create an instance and set the total tweet count
            var stats = CreateStatsInstance(1);
            stats.SetBasicFields(0, totalTweets, 0);

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
        /// <param name="topHashtagsSize">Size of the list for the top Hashtags</param>
        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(9)]
        [InlineData(100)]
        public void SampledStreamStats_Create_ReturnInstance(int topHashtagsSize)
        {
            // Save the current time as the expected default time
            var expectedDate = DateTime.UtcNow;

            // Create an instance with the default date and time
            var stats = CreateStatsInstance(topHashtagsSize);

            // Check the default values
            stats.Should().NotBeNull();
            stats.DailyTweets.Should().Be(0);
            stats.HourlyTweets.Should().Be(0);
            stats.LastUpdated.Date.Should().Be(expectedDate.Date);
            stats.Status.Should().BeNull();

            stats.TopHashtagsSize.Should().Be(topHashtagsSize);
            for (int i = 0; i < stats.TopHashtagsSize; i++)
            {
                stats.TopHashtags[i].Should().BeNull();
                stats.TopHashtagCounts[0].Should().Be(0);
            }

            stats.TotalHashtags.Should().Be(0);
            stats.TotalTweets.Should().Be(0);
            stats.TweetQueueCount.Should().Be(0);
        }

        /// <summary>
        /// Test that setting the Status is successful
        /// </summary>
        /// <param name="status">The value for the status</param>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("Good")]
        [InlineData("A very bad status")]
        public void SampledStreamStats_SetStatus_Successful(string status)
        {
            // Create an instance
            var stats = CreateStatsInstance(1);

            // Set the status
            stats.Status = status;

            // Check the result
            stats.Status.Should().Be(status);
        }

        /// <summary>
        /// Test that hashtags and their counts are properly added to a list of 1 top hashtags
        /// </summary>
        [Fact]
        public void UpdateTopHashtags_AddTo1Top_Successful()
        {
            // Create an instance with one top hashtag
            var stats = CreateStatsInstance(1);

            // Add a hashtag and check the result
            stats.UpdateTopHashtags(hashtag1, 1);
            CheckTopHashtag(stats, 0, hashtag1, 1);

            // Add it again and check the result
            stats.UpdateTopHashtags(hashtag1, 2);
            CheckTopHashtag(stats, 0, hashtag1, 2);

            // Add it again and check the result
            stats.UpdateTopHashtags(hashtag1, 10);
            CheckTopHashtag(stats, 0, hashtag1, 10);

            // Add another hashtag and check the result
            stats.UpdateTopHashtags(hashtag2, 9);
            CheckTopHashtag(stats, 0, hashtag1, 10);

            // Add it again and check the result
            stats.UpdateTopHashtags(hashtag2, 10);
            CheckTopHashtag(stats, 0, hashtag2, 10);

            // Add it again and check the result
            stats.UpdateTopHashtags(hashtag2, 11);
            CheckTopHashtag(stats, 0, hashtag2, 11);

            // Add another hashtag and check the result
            stats.UpdateTopHashtags(hashtag3, 10);
            CheckTopHashtag(stats, 0, hashtag2, 11);

            // Add it again and check the result
            stats.UpdateTopHashtags(hashtag3, 20);
            CheckTopHashtag(stats, 0, hashtag3, 20);
        }

        /// <summary>
        /// Test that hashtags and their counts are properly added to a list of 2 top hashtags
        /// </summary>
        [Fact]
        public void UpdateTopHashtags_AddTo2Top_Successful()
        {
            // Create an instance with two top hashtags
            var stats = CreateStatsInstance(2);

            // Add a hashtag and check the result
            stats.UpdateTopHashtags(hashtag1, 1);
            CheckTopHashtag(stats, 0, hashtag1, 1);
            CheckTopHashtag(stats, 1, null, 0);

            // Add it again and check the result
            stats.UpdateTopHashtags(hashtag1, 2);
            CheckTopHashtag(stats, 0, hashtag1, 2);
            CheckTopHashtag(stats, 1, null, 0);

            // Add it again and check the result
            stats.UpdateTopHashtags(hashtag1, 10);
            CheckTopHashtag(stats, 0, hashtag1, 10);
            CheckTopHashtag(stats, 1, null, 0);

            // Add another hashtag and check the result
            stats.UpdateTopHashtags(hashtag2, 9);
            CheckTopHashtag(stats, 0, hashtag1, 10);
            CheckTopHashtag(stats, 1, hashtag2, 9);

            // Add it again and check the result
            stats.UpdateTopHashtags(hashtag2, 10);
            CheckTopHashtag(stats, 0, hashtag2, 10);
            CheckTopHashtag(stats, 1, hashtag1, 10);

            // Add it again and check the result
            stats.UpdateTopHashtags(hashtag2, 11);
            CheckTopHashtag(stats, 0, hashtag2, 11);
            CheckTopHashtag(stats, 1, hashtag1, 10);

            // Add the first one again and check the result
            stats.UpdateTopHashtags(hashtag1, 12);
            CheckTopHashtag(stats, 0, hashtag1, 12);
            CheckTopHashtag(stats, 1, hashtag2, 11);
        }

        /// <summary>
        /// Test that hashtags and their counts are properly added to a list of 3 top hashtags
        /// </summary>
        [Fact]
        public void UpdateTopHashtags_AddTo3Top_Successful()
        {
            // Create an instance with 3 top hashtags
            var stats = CreateStatsInstance(3);

            // Add a hashtag and check the result
            stats.UpdateTopHashtags(hashtag1, 1);
            CheckTopHashtag(stats, 0, hashtag1, 1);
            CheckTopHashtag(stats, 1, null, 0);
            CheckTopHashtag(stats, 2, null, 0);

            // Add it again and check the result
            stats.UpdateTopHashtags(hashtag1, 2);
            CheckTopHashtag(stats, 0, hashtag1, 2);
            CheckTopHashtag(stats, 1, null, 0);
            CheckTopHashtag(stats, 2, null, 0);

            // Add it again and check the result
            stats.UpdateTopHashtags(hashtag1, 10);
            CheckTopHashtag(stats, 0, hashtag1, 10);
            CheckTopHashtag(stats, 1, null, 0);
            CheckTopHashtag(stats, 2, null, 0);

            // Add another hashtag and check the result
            stats.UpdateTopHashtags(hashtag2, 9);
            CheckTopHashtag(stats, 0, hashtag1, 10);
            CheckTopHashtag(stats, 1, hashtag2, 9);
            CheckTopHashtag(stats, 2, null, 0);

            // Add it again and check the result
            stats.UpdateTopHashtags(hashtag2, 10);
            CheckTopHashtag(stats, 0, hashtag2, 10);
            CheckTopHashtag(stats, 1, hashtag1, 10);
            CheckTopHashtag(stats, 2, null, 0);

            // Add it again and check the result
            stats.UpdateTopHashtags(hashtag2, 11);
            CheckTopHashtag(stats, 0, hashtag2, 11);
            CheckTopHashtag(stats, 1, hashtag1, 10);
            CheckTopHashtag(stats, 2, null, 0);

            // Add another hashtag and check the result
            stats.UpdateTopHashtags(hashtag3, 9);
            CheckTopHashtag(stats, 0, hashtag2, 11);
            CheckTopHashtag(stats, 1, hashtag1, 10);
            CheckTopHashtag(stats, 2, hashtag3, 9);

            // Add it again and check the result
            stats.UpdateTopHashtags(hashtag3, 10);
            CheckTopHashtag(stats, 0, hashtag2, 11);
            CheckTopHashtag(stats, 1, hashtag3, 10);
            CheckTopHashtag(stats, 2, hashtag1, 10);

            // Add it again and check the result
            stats.UpdateTopHashtags(hashtag3, 11);
            CheckTopHashtag(stats, 0, hashtag3, 11);
            CheckTopHashtag(stats, 1, hashtag2, 11);
            CheckTopHashtag(stats, 2, hashtag1, 10);

            // Add it again and check the result
            stats.UpdateTopHashtags(hashtag3, 12);
            CheckTopHashtag(stats, 0, hashtag3, 12);
            CheckTopHashtag(stats, 1, hashtag2, 11);
            CheckTopHashtag(stats, 2, hashtag1, 10);
        }

        /// <summary>
        /// Test that hashtags and their counts are properly added to a list of 9 top hashtags
        /// </summary>
        [Fact]
        public void UpdateTopHashtags_AddTo9Top_Successful()
        {
            // Create an instance with 9 top hashtags
            const int size = 9;
            var stats = CreateStatsInstance(size);

            // Add a hashtag and check the result
            stats.UpdateTopHashtags(hashtag1, 1);
            CheckTopHashtag(stats, 0, hashtag1, 1);
            for (int i = 1; i < size; i++) { CheckTopHashtag(stats, i, null, 0); }

            // Add it again and check the result
            stats.UpdateTopHashtags(hashtag1, 2);
            CheckTopHashtag(stats, 0, hashtag1, 2);
            for (int i = 1; i < size; i++) { CheckTopHashtag(stats, i, null, 0); }

            // Add it again and check the result
            stats.UpdateTopHashtags(hashtag1, 10);
            CheckTopHashtag(stats, 0, hashtag1, 10);
            for (int i = 1; i < size; i++) { CheckTopHashtag(stats, i, null, 0); }

            // Add another hashtag and check the result
            stats.UpdateTopHashtags(hashtag2, 9);
            CheckTopHashtag(stats, 0, hashtag1, 10);
            CheckTopHashtag(stats, 1, hashtag2, 9);
            for (int i = 2; i < size; i++) { CheckTopHashtag(stats, i, null, 0); }

            // Add it again and check the result
            stats.UpdateTopHashtags(hashtag2, 10);
            CheckTopHashtag(stats, 0, hashtag2, 10);
            CheckTopHashtag(stats, 1, hashtag1, 10);
            for (int i = 2; i < size; i++) { CheckTopHashtag(stats, i, null, 0); }

            // Add it again and check the result
            stats.UpdateTopHashtags(hashtag2, 11);
            CheckTopHashtag(stats, 0, hashtag2, 11);
            CheckTopHashtag(stats, 1, hashtag1, 10);
            for (int i = 2; i < size; i++) { CheckTopHashtag(stats, i, null, 0); }

            // Add another hashtag and check the result
            stats.UpdateTopHashtags(hashtag3, 9);
            CheckTopHashtag(stats, 0, hashtag2, 11);
            CheckTopHashtag(stats, 1, hashtag1, 10);
            CheckTopHashtag(stats, 2, hashtag3, 9);
            for (int i = 3; i < size; i++) { CheckTopHashtag(stats, i, null, 0); }

            // Add it again and check the result
            stats.UpdateTopHashtags(hashtag3, 10);
            CheckTopHashtag(stats, 0, hashtag2, 11);
            CheckTopHashtag(stats, 1, hashtag3, 10);
            CheckTopHashtag(stats, 2, hashtag1, 10);
            for (int i = 3; i < size; i++) { CheckTopHashtag(stats, i, null, 0); }

            // Add it again and check the result
            stats.UpdateTopHashtags(hashtag3, 11);
            CheckTopHashtag(stats, 0, hashtag3, 11);
            CheckTopHashtag(stats, 1, hashtag2, 11);
            CheckTopHashtag(stats, 2, hashtag1, 10);
            for (int i = 3; i < size; i++) { CheckTopHashtag(stats, i, null, 0); }

            // Add it again and check the result
            stats.UpdateTopHashtags(hashtag3, 12);
            CheckTopHashtag(stats, 0, hashtag3, 12);
            CheckTopHashtag(stats, 1, hashtag2, 11);
            CheckTopHashtag(stats, 2, hashtag1, 10);
            for (int i = 3; i < size; i++) { CheckTopHashtag(stats, i, null, 0); }

            // Add another hashtag and check the result
            stats.UpdateTopHashtags(hashtag4, 11);
            CheckTopHashtag(stats, 0, hashtag3, 12);
            CheckTopHashtag(stats, 1, hashtag4, 11);
            CheckTopHashtag(stats, 2, hashtag2, 11);
            CheckTopHashtag(stats, 3, hashtag1, 10);
            for (int i = 4; i < size; i++) { CheckTopHashtag(stats, i, null, 0); }

            // Add another hashtag and check the result
            stats.UpdateTopHashtags(hashtag5, 12);
            CheckTopHashtag(stats, 0, hashtag5, 12);
            CheckTopHashtag(stats, 1, hashtag3, 12);
            CheckTopHashtag(stats, 2, hashtag4, 11);
            CheckTopHashtag(stats, 3, hashtag2, 11);
            CheckTopHashtag(stats, 4, hashtag1, 10);
            for (int i = 5; i < size; i++) { CheckTopHashtag(stats, i, null, 0); }
        }

        /// <summary>
        /// Check that a top hashtags entry matches an expected hashtag and count
        /// </summary>
        /// <param name="stats">Statistics instance to check</param>
        /// <param name="index">Index in top hashtag table</param>
        /// <param name="hashtag">Expected hashtag</param>
        /// <param name="count">Expected count</param>
        private static void CheckTopHashtag(SampledStreamStats stats, int index, string? hashtag, ulong count)
        {
            // Check the entry
            stats.TopHashtags[index].Should().Be(hashtag);
            stats.TopHashtagCounts[index].Should().Be(count);
        }

        /// <summary>
        /// Create an instance of the SampledStreamStats class
        /// </summary>
        /// <returns>The new instance</returns>
        /// <param name="topHashtagsSize">Size of the list for the top Hashtags</param>
        private static SampledStreamStats CreateStatsInstance(int topHashtagsSize)
        {
            return new SampledStreamStats(topHashtagsSize);
        }
    }
}
