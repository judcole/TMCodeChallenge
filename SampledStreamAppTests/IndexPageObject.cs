using OpenQA.Selenium;

namespace SampledStreamApp.Pages.Tests
{
    // Class to represent the Index Web Page contents for unit testing
    public class IndexPageObject
    {
        // The local URL of the Index page
        private const string _indexPageUrl = "https://localhost:44324/";

        // Saved test Web driver for accessing elements
        private readonly IWebDriver _webDriver;

        // Daily tweet count
        public IWebElement DailyTweets => _webDriver.FindElement(By.Id("daily-tweets"));

        // Hourly tweet count
        public IWebElement HourlyTweets => _webDriver.FindElement(By.Id("hourly-tweets"));

        // Date and time of the last stats update
        public IWebElement LastUpdated => _webDriver.FindElement(By.Id("last-updated"));

        // Top hashtag 1
        public IWebElement TopHashtag1 => _webDriver.FindElement(By.Id("hashtag-1"));

        // Top hashtag 1 count
        public IWebElement TopHashtagCount1 => _webDriver.FindElement(By.Id("hashtag-count-1"));

        // Top hashtag 2
        public IWebElement TopHashtag2 => _webDriver.FindElement(By.Id("hashtag-2"));

        // Top hashtag 1 count
        public IWebElement TopHashtagCount2 => _webDriver.FindElement(By.Id("hashtag-count-2"));

        // Total tweet count
        public IWebElement TotalTweets => _webDriver.FindElement(By.Id("total-tweets"));

        // Construct the page object
        public IndexPageObject(IWebDriver webDriver)
        {
            // Save the Web driver
            _webDriver = webDriver;
        }

        // Reset the Web driver URL to the Index page
        public void ResetIndexPage()
        {
            if (_webDriver.Url != _indexPageUrl)
            {
                _webDriver.Url = _indexPageUrl;
            }
        }
    }
}
