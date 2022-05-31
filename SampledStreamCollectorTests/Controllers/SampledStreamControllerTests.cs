using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace SampledStreamCollector.Controllers.Tests
{
    /// <summary>
    /// Class for SampledStreamController xUnit tests
    /// </summary>
    public class SampledStreamControllerTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        /// <summary>
        /// Construct the tests object
        /// </summary>
        /// <param name="testOutputHelper">Where to send the test output</param>
        public SampledStreamControllerTests(ITestOutputHelper testOutputHelper)
        {
            // Save the output helper for logging
            this._testOutputHelper = testOutputHelper;
        }

        /// <summary>
        /// Test the creation of a new controller
        /// </summary>
        [Fact()]
        public void SampledStreamControllerTest()
        {
            _testOutputHelper.WriteLine("Starting SampledStreamController Test");

            var mockLogger = new Mock<ILogger<SampledStreamController>>();
            var newController = new SampledStreamController(mockLogger.Object);

            // Check the initial controller state
            newController.Should().NotBeNull();

            _testOutputHelper.WriteLine("Ending SampledStreamController Test");
        }

        /// <summary>
        /// Test the SampledStreamController Get method
        /// </summary>
        [Fact()]
        public void GetTest()
        {
            _testOutputHelper.WriteLine("Starting SampledStreamController Get Test");

            var mockLogger = new Mock<ILogger<SampledStreamController>>();
            var newController = new SampledStreamController(mockLogger.Object);

            // Call the Get method
            var stats = newController.Get();

            // Check the result
            stats.Should().NotBeNull();
            stats.TotalTweets.Should().BeGreaterThan(0);
            stats.Status.Should().Be("Good");

            _testOutputHelper.WriteLine("Ending SampledStreamController Get Test");
        }
    }
}