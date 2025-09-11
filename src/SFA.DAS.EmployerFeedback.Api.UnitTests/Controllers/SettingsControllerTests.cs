using System;
using System.Collections.Generic;
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
using SFA.DAS.EmployerFeedback.Application.Commands.UpsertSettings;
using SFA.DAS.EmployerFeedback.Application.Models;
using SFA.DAS.EmployerFeedback.Application.Queries.GetSettings;

namespace SFA.DAS.EmployerFeedback.Api.UnitTests.Controllers
{
    [TestFixture]
    public class SettingsControllerTests
    {
        private Mock<IMediator> _mediator;
        private Mock<ILogger<SettingsController>> _logger;
        private SettingsController _controller;

        [SetUp]
        public void SetUp()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<SettingsController>>();
            _controller = new SettingsController(_mediator.Object, _logger.Object);
        }

        [Test]
        public async Task GetSettings_Should_Return_Ok_With_Settings()
        {
            var settings = new List<Settings> { new Settings { Name = "Test", Value = "Val" } };
            _mediator.Setup(x => x.Send(It.IsAny<GetSettingsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetSettingsQueryResult { Settings = settings });

            var result = await _controller.GetSettings();
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(settings, okResult.Value);
            _mediator.Verify(x => x.Send(It.IsAny<GetSettingsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetSettings_Should_Return_InternalServerError_On_Exception()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetSettingsQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("fail"));
            var result = await _controller.GetSettings();
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.AreEqual("An unexpected error occurred.", objectResult.Value);
        }

        [Test]
        public async Task UpsertSettings_Should_Send_Command_And_Return_NoContent()
        {
            var req = new List<SettingRequest> { new SettingRequest { Name = "Test", Value = "Val" } };
            _mediator.Setup(x => x.Send(It.IsAny<UpsertSettingsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);

            var result = await _controller.UpsertSettings(req);
            Assert.IsInstanceOf<NoContentResult>(result);
            _mediator.Verify(x => x.Send(It.IsAny<UpsertSettingsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task UpsertSettings_Should_Return_BadRequest_On_ValidationException()
        {
            var req = new List<SettingRequest> { new SettingRequest { Name = "Invalid Name!", Value = "Val" } };
            _mediator.Setup(x => x.Send(It.IsAny<UpsertSettingsCommand>(), It.IsAny<CancellationToken>())).ThrowsAsync(new ValidationException("validation failed"));
            var result = await _controller.UpsertSettings(req);
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequest.StatusCode);
            Assert.AreEqual("validation failed", badRequest.Value);
        }

        [Test]
        public async Task UpsertSettings_Should_Return_InternalServerError_On_Exception()
        {
            var req = new List<SettingRequest> { new SettingRequest { Name = "Test", Value = "Val" } };
            _mediator.Setup(x => x.Send(It.IsAny<UpsertSettingsCommand>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("fail"));
            var result = await _controller.UpsertSettings(req);
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }
    }
}
