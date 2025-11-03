using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using FluentAssertions;
using SFA.DAS.EmployerFeedback.Api.Controllers;
using SFA.DAS.EmployerFeedback.Application.Queries.GetAttributes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Api.UnitTests.Controllers
{
    public class AttributesControllerTests
    {
        [Test]
        public async Task GetAttributes_ReturnsOk_WhenMediatorReturnsAttributes()
        {
            var attributes = new List<SFA.DAS.EmployerFeedback.Domain.Models.Attributes> {
                new SFA.DAS.EmployerFeedback.Domain.Models.Attributes { AttributeId = 1, AttributeName = "Test" }
            };
            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<GetAttributesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetAttributesQueryResult { Attributes = attributes });
            var logger = new Mock<ILogger<AttributesController>>();
            var controller = new AttributesController(mediator.Object, logger.Object);

            var result = await controller.GetAttributes();
            
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(attributes);
        }

        [Test]
        public async Task GetAttributes_ReturnsInternalServerError_WhenMediatorThrows()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(m => m.Send(It.IsAny<GetAttributesQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("fail"));
            var logger = new Mock<ILogger<AttributesController>>();
            var controller = new AttributesController(mediator.Object, logger.Object);

            var result = await controller.GetAttributes();
            
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(500);
            objectResult.Value.Should().Be("An unexpected error occurred.");
        }
    }
}
