using Microsoft.Extensions.Logging;
using FluentAssertions;
using Moq;
using SampledStreamCommon;

namespace SampledStreamCollector.Tests
{
    public class TweetCollectorTests
    {
        /// <summary>
        /// Test that the creation of a new TweetCollector is successful
        /// </summary>
        [Fact()]
        public void TweetCollector_Create_ReturnInstance()
        {
            // Create an instance
            var tweetCollector = CreateTweetCollectorInstance();

            TweetCollector.HashtagRegex.Should().NotBeNull();

            tweetCollector.Should().NotBeNull();
        }

        /// <summary>
        /// Test that the Async operations can be stopped successfully
        /// </summary>
        ///
        /// Todo: Improve this test to launch the task first
        [Fact()]
        public void StopAsyncTest()
        {
            // Create an instance
            var tweetCollector = CreateTweetCollectorInstance();

            // Stop the tasks
            CancellationToken stoppingToken = new();
            tweetCollector.StopAsync(stoppingToken);

            tweetCollector.Should().NotBeNull();
        }

        /// <summary>
        /// Test that the Hashtag Regular Expression successfully matches hashtags
        /// </summary>
        /// <param name="input"></param>
        /// <param name="count"></param>
        /// <param name="position"></param>
        /// <param name="expected"></param>
        [Theory]
        [InlineData("xxx", 0, null, null)]
        [InlineData("#xxx", 1, new int[] { 0 }, new string[] { "xxx" })]
        [InlineData("x#xxx", 0, null, null)]
        [InlineData("x+#abc", 1, new int[] { 2 }, new string[] { "abc" })]
        [InlineData("x4-#abc", 1, new int[] { 3 }, new string[] { "abc" })]
        [InlineData("x4 #abc", 1, new int[] { 3 }, new string[] { "abc" })]
        [InlineData("xq;#abc.#xyz123 ww", 2, new int[] { 3, 8 }, new string[] { "abc", "xyz123" })]
        [InlineData("abcd-12w #abc.#xyz123 ww", 2, new int[] { 9, 14 }, new string[] { "abc", "xyz123" })]
        [InlineData("wwwwwww~#wwwwww.wwwww.#xyz123 ww", 2, new int[] { 8, 22 }, new string[] { "wwwwww", "xyz123" })]

        public void HashtagRegex_Matches_Successful(string input, int count, int[] positions, string[] expecteds)
        {
            // Match on the input string
            var matches = TweetCollector.HashtagRegex.Matches(input);

            // Check the number of matches
            matches.Count.Should().Be(count);

            // Count the matches
            int matched = 0;

            // Loop through the results
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                // Get the hashtag without the leading hash
                var hashtag = match.Value[1..];

                // Check the result
                matched.Should().BeLessThan(count);
                match.Index.Should().Be(positions[matched]);
                hashtag.Should().Be(expecteds[matched]);

                // On to the next match
                matched++;
            }

            // There should not be another match
            matched.Should().Be(count);
        }

        /// <summary>
        /// Create an instance of the TweetCollector class
        /// </summary>
        /// <returns></returns>
        private static TweetCollector CreateTweetCollectorInstance()
        {
            var mockQueue = new Mock<IBackgroundQueue<TweetBlock>>();
            var mockLogger = new Mock<ILogger<TweetCollector>>();
            var stats = new SampledStreamStats(1);
            return new TweetCollector(mockQueue.Object, stats, mockLogger.Object);
        }

    }
}