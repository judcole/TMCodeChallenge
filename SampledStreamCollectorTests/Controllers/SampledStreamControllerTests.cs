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
        public void SampledStreamController_Get_ReturnStats()
        {
            _testOutputHelper.WriteLine("Starting SampledStreamController_Get_ReturnStats");

            // Create an instance
            var newController = CreateControllerInstance();

            // Call the Get method
            var stats = newController.Get();

            // Check the result
            stats.Should().NotBeNull();
            stats.TotalTweets.Should().BeGreaterThan(0);
            stats.Status.Should().Be("Good");

            _testOutputHelper.WriteLine("Ending SampledStreamController_Get_ReturnStats");
        }

        /// <summary>
        /// Create an instance of the SampledStreamController class
        /// </summary>
        /// <returns></returns>
        private static SampledStreamController CreateControllerInstance()
        {
            var mockLogger = new Mock<ILogger<SampledStreamController>>();
            return new SampledStreamController(mockLogger.Object);
        }
    }
}