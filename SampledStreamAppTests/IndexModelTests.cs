using Xunit.Abstractions;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace SampledStreamApp.Pages.Tests
{
    // Class for Index page model tests
    public class IndexModelTests : IDisposable
    {
        private readonly ITestOutputHelper testOutputHelper;
        private readonly IWebDriver webDriver;

        // Construct the tests object
        public IndexModelTests(ITestOutputHelper testOutputHelper)
        {
            // Save the output helper for logging
            this.testOutputHelper = testOutputHelper;
            webDriver = new ChromeDriver();

            // Log that we are running the tests
            Console.WriteLine("Running tests (console)");
            testOutputHelper.WriteLine("Running tests (helper)");
        }

        // Dispoase of the tests object
        public void Dispose()
        {
            // Log that we are shutting down
            testOutputHelper.WriteLine("Disposing IndexModelTests");

            // Close and free up the Web driver
            webDriver.Quit();
            webDriver.Dispose();
            GC.SuppressFinalize(this);
        }

        // Test the rendered Index Page in the browser
        [Fact()]
        public void IndexPageTest()
        {
            testOutputHelper.WriteLine("Starting IndexPageTest");

            // Navigate to the index page
            webDriver.Navigate().GoToUrl("https://localhost:44324/");

            // Check the last updated date and time
            var lastUpdated = webDriver.FindElement(By.Id("last-updated"));
            var lastUpdatedValue = lastUpdated.Text[..10];
            lastUpdatedValue.Should().Be(DateTime.Now.AddSeconds(-1).ToString("G")[..10]);

            // Check the total tweets count
            var totalTweets = webDriver.FindElement(By.Id("total-tweets"));
            var totalTweetsValue = totalTweets.Text;
            totalTweetsValue.Should().Be("4");

            testOutputHelper.WriteLine("Ending IndexPageTest");
        }

        // Test the 
        [Fact()]
        public void TestIndexModelOnGet()
        {
            testOutputHelper.WriteLine("Starting IndexModel.OnGet Test");

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

            testOutputHelper.WriteLine("Ending IndexModel.OnGet Test");
        }
    }
}