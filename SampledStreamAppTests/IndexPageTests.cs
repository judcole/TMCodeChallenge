using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using Xunit.Abstractions;

namespace SampledStreamApp.Pages.Tests
{
    /// <summary>
    /// Class for Index page model tests
    /// </summary>
    /// 
    /// Todo: Add testing of Blazor rendered pages using bUnit
    /// Todo: Enhance Web UI tests to also run in FireFox and Edge
    public class IndexPageTests : IDisposable
    {
        private readonly IndexPageObject _indexPageObject;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IWebDriver _webDriver;

        /// <summary>
        /// Construct the tests instance
        /// </summary>
        /// <param name="testOutputHelper"></param>
        public IndexPageTests(ITestOutputHelper testOutputHelper)
        {
            // Save the output helper for logging
            this._testOutputHelper = testOutputHelper;

            // Log that we are running the tests
            Console.WriteLine("Running tests (console)");
            testOutputHelper.WriteLine("Running tests (helper)");

            // Create Chrome web Driver
            _webDriver = new ChromeDriver();

            // Create Index Page Object for testing
            _indexPageObject = new IndexPageObject(_webDriver);
        }

        /// <summary>
        /// Dispose of the tests instance
        /// </summary>
        public void Dispose()
        {
            // Log that we are shutting down
            _testOutputHelper.WriteLine("Disposing IndexModelTests");

            // Close and free up the Web driver
            _webDriver.Quit();
            _webDriver.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Test that the IndexModel OnGet method returns valid statistics
        /// </summary>
        [Fact()]
        public async Task IndexModelOnGet_Request_ReturnStats()
        {
            _testOutputHelper.WriteLine("Starting IndexModelOnGet_Request_ReturnStats");

            // Set up a mocked response from the API
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    Content = new StringContent("{\"dailyTweets\":123,\"hourlyTweets\":456,\"totalTweets\":789,\"totalHashtags\":321,\"tweetQueueCount\":654}"),
                    StatusCode = HttpStatusCode.OK
                });
            var mockHttpClient = new HttpClient(mockHttpMessageHandler.Object);

            // Create an intance of the model
            var mockLogger = new Mock<ILogger<IndexModel>>();
            var expectedLastUpdated = DateTime.UtcNow;
            var indexModel = new IndexModel(mockHttpClient, mockLogger.Object);

            // Check the initial model state
            indexModel.Should().NotBeNull();
            indexModel.DayName.Should().Be(null);
            var dateDifference = indexModel.LastUpdated.Subtract(expectedLastUpdated);
            dateDifference.TotalSeconds.Should().BeInRange(-30, 30);

            // Call the OnGet method to populate the model
            await indexModel.OnGet();

            // Check the model state
            indexModel.DayName.Should().Be(DateTime.Now.ToString("dddd"));
            dateDifference = indexModel.LastUpdated.Subtract(expectedLastUpdated);
            dateDifference.TotalSeconds.Should().BeInRange(-30, 30);
            indexModel.DailyTweets.Should().Be(123);
            indexModel.HourlyTweets.Should().Be(456);
            indexModel.Status.Should().Be(null);
            indexModel.TotalHashtags.Should().Be(321);
            indexModel.TotalTweets.Should().Be(789);
            indexModel.TweetQueueCount.Should().Be(654);

            _testOutputHelper.WriteLine("IndexModelOnGet_Request_ReturnStats");
        }

        /// <summary>
        /// Test that the Index Page is properly rendered in the browser
        /// </summary>
        [Fact()]
        public void IndexPage_Get_RenderPage()
        {
            _testOutputHelper.WriteLine("Starting IndexPage_Get_RenderPage");

            var expectedLastUpdated = DateTime.UtcNow;

            // Navigate to the index page
            if (!_indexPageObject.NavigateToIndexPage()) {
                // WebDriver app is not running so just log it and return
                _testOutputHelper.WriteLine("Web App is not running on localhost so skipping Selenium tests");

                return;
            }

            // Check the last updated date and time using the hidden attribute with the time in UTC
            if (DateTime.TryParse(_indexPageObject.LastUpdated.GetAttribute("last-updated-utc"), out DateTime lastUpdated))
            {
                var dateDifference = expectedLastUpdated.Subtract(lastUpdated);
                dateDifference.TotalSeconds.Should().BeInRange(-30, 30);
            } else
            {
                Assert.True(false, $"Invalid last updated date {_indexPageObject.LastUpdated.Text}");
            }

            // Check the total tweets count
            var totalTweetsValue = _indexPageObject.TotalTweets.Text;
            Int32.Parse(totalTweetsValue).Should().BeInRange(100, 200);

            // Check the queued tweets count
            var tweetQueueCountValue = _indexPageObject.TweetQueueCount.Text;
            Int32.Parse(tweetQueueCountValue).Should().BeInRange(100, 200);

            // Check the daily tweets count
            var dailyTweetsValue = _indexPageObject.DailyTweets.Text;
            Int32.Parse(dailyTweetsValue).Should().BeInRange(100, 200);

            // Check the hourly tweets count
            var hourlyTweetsValue = _indexPageObject.HourlyTweets.Text;
            Int32.Parse(hourlyTweetsValue).Should().BeInRange(100, 200);

            // Check the top hashtag 1
            var topHashtag1Value = _indexPageObject.TopHashtag1.Text;
            topHashtag1Value.Should().Be("");

            // Check the top hashtag 1 count
            var topHashtag1CountValue = _indexPageObject.TopHashtagCount1.Text;
            Int32.Parse(topHashtag1CountValue).Should().Be(0);

            // Check the top hashtag 2
            var topHashtag2Value = _indexPageObject.TopHashtag2.Text;
            topHashtag2Value.Should().Be("");

            // Check the top hashtag 2 count
            var topHashtag2CountValue = _indexPageObject.TopHashtagCount2.Text;
            Int32.Parse(topHashtag2CountValue).Should().Be(0);

            _testOutputHelper.WriteLine("Ending IndexPage_Get_RenderPage");
        }
    }
}