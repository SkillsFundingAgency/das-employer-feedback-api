using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Api.Controllers;
using SFA.DAS.EmployerFeedback.Application.Commands.SubmitEmployerFeedback;
using SFA.DAS.EmployerFeedback.Application.Models;

namespace SFA.DAS.EmployerFeedback.Api.UnitTests.Controllers
{
    [TestFixture]
    public class EmployerFeedbackResultControllerTests
    {
        private Mock<IMediator> _mediator;
        private Mock<ILogger<EmployerFeedbackResultController>> _logger;
        private EmployerFeedbackResultController _controller;

        [SetUp]
        public void SetUp()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<EmployerFeedbackResultController>>();
            _controller = new EmployerFeedbackResultController(_mediator.Object, _logger.Object);
        }

        [Test]
        public async Task SubmitEmployerFeedback_ReturnsNoContent_WhenSuccessful()
        {
            var request = new SubmitEmployerFeedbackRequest
            {
                UserRef = Guid.NewGuid(),
                Ukprn = 1,
                AccountId = 2,
                ProviderRating = OverallRating.Good,
                FeedbackSource = 1,
                ProviderAttributes = null
            };
            _mediator.Setup(m => m.Send(It.IsAny<SubmitEmployerFeedbackCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new SubmitEmployerFeedbackCommandResponse());

            var result = await _controller.SubmitEmployerFeedback(request);

            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task SubmitEmployerFeedback_ReturnsBadRequest_WhenValidationExceptionThrown()
        {
            var request = new SubmitEmployerFeedbackRequest();
            _mediator.Setup(m => m.Send(It.IsAny<SubmitEmployerFeedbackCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("validation failed"));

            var result = await _controller.SubmitEmployerFeedback(request);

            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequest.StatusCode);
            Assert.AreEqual("validation failed", badRequest.Value);
        }

        [Test]
        public async Task SubmitEmployerFeedback_ReturnsInternalServerError_WhenExceptionThrown()
        {
            var request = new SubmitEmployerFeedbackRequest();
            _mediator.Setup(m => m.Send(It.IsAny<SubmitEmployerFeedbackCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("fail"));

            var result = await _controller.SubmitEmployerFeedback(request);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.AreEqual("An unexpected error occurred.", objectResult.Value);
        }
    }
}
