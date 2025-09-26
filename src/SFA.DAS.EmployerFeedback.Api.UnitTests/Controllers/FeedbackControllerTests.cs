using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using ESFA.DAS.EmployerProvideFeedback.Api.Controllers;
using SFA.DAS.EmployerFeedback.Application.Queries.GetAllEmployerFeedback;
using SFA.DAS.EmployerFeedback.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Api.UnitTests.Controllers
{
    public class FeedbackControllerTests
    {
        [Test]
        public async Task GetAll_ReturnsOk_WhenMediatorReturnsFeedback()
        {
            var feedbacks = new List<AllEmployerFeedbackResults>
            {
                new AllEmployerFeedbackResults { Ukprn = 1, ProviderRating = "Good", ProviderAttributes = new List<ProviderAttributeResults>() }
            };
            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<GetAllEmployerFeedbackQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetAllEmployerFeedbackQueryResult { Feedbacks = feedbacks });
            var logger = new Mock<ILogger<FeedbackController>>();
            var controller = new FeedbackController(mediator.Object, logger.Object);

            var result = await controller.GetAll();
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(feedbacks, okResult.Value);
            mediator.Verify(m => m.Send(It.IsAny<GetAllEmployerFeedbackQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetAll_ReturnsNoContent_WhenNoFeedback()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<GetAllEmployerFeedbackQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetAllEmployerFeedbackQueryResult { Feedbacks = new List<AllEmployerFeedbackResults>() });
            var logger = new Mock<ILogger<FeedbackController>>();
            var controller = new FeedbackController(mediator.Object, logger.Object);

            var result = await controller.GetAll();
            Assert.IsInstanceOf<NoContentResult>(result);
            mediator.Verify(m => m.Send(It.IsAny<GetAllEmployerFeedbackQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetAll_ReturnsInternalServerError_WhenMediatorThrows()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<GetAllEmployerFeedbackQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("fail"));
            var logger = new Mock<ILogger<FeedbackController>>();
            var controller = new FeedbackController(mediator.Object, logger.Object);

            var result = await controller.GetAll();
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual("An unexpected error occurred.", objectResult.Value);
            mediator.Verify(m => m.Send(It.IsAny<GetAllEmployerFeedbackQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
