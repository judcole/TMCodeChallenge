using Microsoft.Extensions.Logging;
using FluentAssertions;
using Moq;
using Xunit.Abstractions;
using SampledStreamCommon;

namespace SampledStreamCollector.Controllers.Tests
{
    /// <summary>
    /// Class for SampledStreamController xUnit tests
    /// </summary>
    public class SampledStreamControllerTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        /// <summary>
        /// Construct the tests instance
        /// </summary>
        /// <param name="testOutputHelper">Where to send the test output</param>
        public SampledStreamControllerTests(ITestOutputHelper testOutputHelper)
        {
            // Save the output helper for logging
            this._testOutputHelper = testOutputHelper;
        }

        /// <summary>
        /// Test that the creation of a new controller is successful
        /// </summary>
        [Fact()]
        public void SampledStreamController_Create_ReturnInstance()
        {
            _testOutputHelper.WriteLine("SampledStreamController_Create_ReturnInstance");

            // Create an instance
            var newController = CreateControllerInstance();

            // Check the initial controller state
            newController.Should().NotBeNull();

            _testOutputHelper.WriteLine("Ending SampledStreamController_Create_ReturnInstance");
        }

        /// <summary>
        /// Test that the SampledStreamController Get method returns valid statistics
        /// </summary>
        [Fact()]
        public async Task SampledStreamController_GetStats_ReturnStats()
        {
            _testOutputHelper.WriteLine("Starting SampledStreamController_GetStats_ReturnStats");

            // Create an instance
            var controller = CreateControllerInstance();

            // Call the Get method
            var result = await controller.GetStats();

            // Check that we have a result
            var stats = result.Value;
            stats.Should().NotBeNull();

            // Include explicit check to remove 'could be null' warning message
            if (stats is not null)
            {
                // Check the basic results
                stats.TotalTweets.Should().Be(0);
                stats.Status.Should().BeNull();

                // Check the calculated results
                stats.DailyTweets.Should().Be(stats.TotalTweets);
                stats.HourlyTweets.Should().Be(stats.TotalTweets);
            }

            _testOutputHelper.WriteLine("Ending SampledStreamController_GetStats_ReturnStats");
        }

        /// <summary>
        /// Create an instance of the SampledStreamController class
        /// </summary>
        /// <returns></returns>
        private static SampledStreamController CreateControllerInstance()
        {
            var mockLogger = new Mock<ILogger<SampledStreamController>>();
            var stats = new SampledStreamStats(1);
            return new SampledStreamController(stats, mockLogger.Object);
        }
    }
}