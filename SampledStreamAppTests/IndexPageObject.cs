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

        // Date and time of the last stats update
        public IWebElement LastUpdated => _webDriver.FindElement(By.Id("last-updated"));

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
