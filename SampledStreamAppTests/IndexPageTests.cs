using Xunit.Abstractions;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace SampledStreamApp.Pages.Tests
{
    // Class for Index page model tests
    public class IndexPageTests : IDisposable
    {
        private readonly IndexPageObject _indexPageObject;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IWebDriver _webDriver;

        // Construct the tests object
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

        // Dispoase of the tests object
        public void Dispose()
        {
            // Log that we are shutting down
            _testOutputHelper.WriteLine("Disposing IndexModelTests");

            // Close and free up the Web driver
            _webDriver.Quit();
            _webDriver.Dispose();
            GC.SuppressFinalize(this);
        }

        // Test the rendered Index Page in the browser
        [Fact()]
        public void IndexPageTest()
        {
            _testOutputHelper.WriteLine("Starting IndexPageTest");

            // Navigate to the index page
            _indexPageObject.ResetIndexPage();

            // Check the last updated date and time
            var lastUpdatedValue = _indexPageObject.LastUpdated.Text[..10];
            lastUpdatedValue.Should().Be(DateTime.Now.AddSeconds(-1).ToString("G")[..10]);

            // Check the total tweets count
            var totalTweetsValue = _indexPageObject.TotalTweets.Text;
            totalTweetsValue.Should().Be("4");

            _testOutputHelper.WriteLine("Ending IndexPageTest");
        }

        // Test the IndexPage OnGet method
        [Fact()]
        public void IndexModelOnGetTest()
        {
            _testOutputHelper.WriteLine("Starting IndexModel.OnGet Test");

            var mockLogger = new Mock<ILogger<IndexModel>>();
            var indexModel = new IndexModel(mockLogger.Object);

            // Check the initial model state
            indexModel.Should().NotBeNull();
            indexModel.DayName.Should().Be(null);
            indexModel.LastUpdated.Should().Be(null);

            // Call the method
            indexModel.OnGet();

            // Check the model state
            indexModel.DayName.Should().Be(DateTime.Now.ToString("dddd"));
            indexModel.LastUpdated.Should().Be(DateTime.Now.ToString("G"));

            _testOutputHelper.WriteLine("Ending IndexModel.OnGet Test");
        }
    }
}