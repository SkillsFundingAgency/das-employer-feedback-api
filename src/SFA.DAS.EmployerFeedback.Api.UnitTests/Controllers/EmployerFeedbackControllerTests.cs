using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Api.Controllers;
using SFA.DAS.EmployerFeedback.Api.UnitTests.Extensions;
using SFA.DAS.EmployerFeedback.Application.Queries.GetLatestEmployerFeedbackResults;

namespace SFA.DAS.EmployerFeedback.Api.UnitTests.Controllers
{
    [TestFixture]
    public class EmployerFeedbackControllerTests
    {
        [Test]
        public async Task GetLatestResults_WhenMediatorSucceeds_ReturnsOkWithPayload()
        {
            // Arrange
            var accountId = 123456L;
            var userRef = Guid.NewGuid();

            var expected = new GetLatestEmployerFeedbackResultsQueryResult();

            var mediator = new Mock<IMediator>(MockBehavior.Strict);
            mediator.Setup(m => m.Send(
                    It.Is<GetLatestEmployerFeedbackResultsQuery>(q =>
                        q.AccountId == accountId && q.UserRef == userRef),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var logger = new Mock<ILogger<EmployerFeedbackController>>();
            var sut = new EmployerFeedbackController(mediator.Object, logger.Object);

            // Act
            var actionResult = await sut.GetLatestResults(accountId, userRef);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            var ok = (OkObjectResult)actionResult;
            Assert.That(ok.Value, Is.SameAs(expected));
        }

        [Test]
        public async Task GetLatestResults_WhenMediatorThrows_Returns500AndLogsError()
        {
            // Arrange
            var accountId = 42L;
            var userRef = Guid.NewGuid();
            var boom = new InvalidOperationException("boom");

            var mediator = new Mock<IMediator>(MockBehavior.Strict);
            mediator.Setup(m => m.Send(
                    It.Is<GetLatestEmployerFeedbackResultsQuery>(q =>
                        q.AccountId == accountId && q.UserRef == userRef),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(boom);

            var logger = new Mock<ILogger<EmployerFeedbackController>>();
            var sut = new EmployerFeedbackController(mediator.Object, logger.Object);

            // Act
            var actionResult = await sut.GetLatestResults(accountId, userRef);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<ObjectResult>());
            var obj = (ObjectResult)actionResult;
            Assert.That(obj.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(obj.Value, Is.EqualTo("An unexpected error occurred."));
            logger.VerifyLogErrorContains("Server error while retrieving latest employer feedback", boom, Times.Once());
        }
    }    
}
