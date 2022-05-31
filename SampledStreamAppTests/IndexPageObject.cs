using OpenQA.Selenium;

namespace SampledStreamAppTests
{
    // Class to represent the Index Web Page contents for unit testing
    public class IndexPageObject
    {
        // Saved test Web driver for accessing elements
        private IWebDriver _webDriver;

        // Construct the page object
        public IndexPageObject(IWebDriver webDriver)
        {
            _webDriver = webDriver;
        }

        // Date and time of the last stats update
        public IWebElement LastUpdated => _webDriver.FindElement(By.Id("last-updated"));

        // Total tweet count
        public IWebElement TotalTweets => _webDriver.FindElement(By.Id("total-tweets"));
    }
}
