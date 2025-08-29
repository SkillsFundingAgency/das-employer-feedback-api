using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Api.Controllers;
using SFA.DAS.EmployerFeedback.Api.TaskQueue;
using SFA.DAS.EmployerFeedback.Application.Commands.GenerateFeedbackSummaries;
using System;

namespace SFA.DAS.EmployerFeedback.Api.UnitTests.Controllers
{
    public class DataLoadControllerTests
    {
        [Test]
        public void GenerateFeedbackSummaries_ReturnsNoContent_WhenQueuedSuccessfully()
        {
            var backgroundQueue = new Mock<IBackgroundTaskQueue>();
            var logger = new Mock<ILogger<DataLoadController>>();
            var controller = new DataLoadController(backgroundQueue.Object, logger.Object);

            var result = controller.GenerateFeedbackSummaries();

            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(204, noContentResult.StatusCode);
            backgroundQueue.Verify(q => q.QueueBackgroundRequest(
                It.IsAny<GenerateFeedbackSummariesCommand>(),
                It.IsAny<string>(),
                It.IsAny<Action<object, TimeSpan, ILogger<TaskQueueHostedService>>>()), Times.Once);
        }

        [Test]
        public void GenerateFeedbackSummaries_ReturnsInternalServerError_WhenExceptionThrown()
        {
            var backgroundQueue = new Mock<IBackgroundTaskQueue>();
            backgroundQueue.Setup(q => q.QueueBackgroundRequest(
                It.IsAny<GenerateFeedbackSummariesCommand>(),
                It.IsAny<string>(),
                It.IsAny<Action<object, TimeSpan, ILogger<TaskQueueHostedService>>>()))
                .Throws(new Exception("fail"));
            var logger = new Mock<ILogger<DataLoadController>>();
            var controller = new DataLoadController(backgroundQueue.Object, logger.Object);

            var result = controller.GenerateFeedbackSummaries();

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.IsTrue(objectResult.Value.ToString().Contains("Error generate feedback summaries"));
        }
    }
}
