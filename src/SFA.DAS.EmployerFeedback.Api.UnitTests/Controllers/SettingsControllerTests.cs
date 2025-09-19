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
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

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
        public async Task GetSettings_Should_Return_Ok_With_Setting()
        {
            var now = DateTime.UtcNow;
            var setting = new GetSettingsQueryResult { Value = now };
            _mediator.Setup(x => x.Send(It.IsAny<GetSettingsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(setting);

            var result = await _controller.GetSettings();
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            var returned = okResult.Value as GetSettingsQueryResult;
            Assert.IsNotNull(returned);
            Assert.AreEqual(now, returned.Value);
            _mediator.Verify(x => x.Send(It.IsAny<GetSettingsQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetSettings_Should_Return_Null_When_No_Record()
        {
            var setting = new GetSettingsQueryResult { Value = null };
            _mediator.Setup(x => x.Send(It.IsAny<GetSettingsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(setting);
            var result = await _controller.GetSettings();
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            var returned = okResult.Value as GetSettingsQueryResult;
            Assert.IsNotNull(returned);
            Assert.IsNull(returned.Value);
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
            var req = new SettingRequest { Value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) };
            _mediator.Setup(x => x.Send(It.Is<UpsertRefreshALELastRunDateSettingCommand>(c => c.Value == req.Value), It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);

            var result = await _controller.UpsertRefreshALELastRunDateSetting(req);
            Assert.IsInstanceOf<NoContentResult>(result);
            _mediator.Verify(x => x.Send(It.IsAny<UpsertRefreshALELastRunDateSettingCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task UpsertSettings_Should_Return_BadRequest_On_ValidationException()
        {
            var req = new SettingRequest { Value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) };
            _mediator.Setup(x => x.Send(It.IsAny<UpsertRefreshALELastRunDateSettingCommand>(), It.IsAny<CancellationToken>())).ThrowsAsync(new ValidationException("validation failed"));
            var result = await _controller.UpsertRefreshALELastRunDateSetting(req);
            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequest.StatusCode);
            Assert.AreEqual("validation failed", badRequest.Value);
        }

        [Test]
        public async Task UpsertSettings_Should_Return_InternalServerError_On_Exception()
        {
            var req = new SettingRequest { Value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) };
            _mediator.Setup(x => x.Send(It.IsAny<UpsertRefreshALELastRunDateSettingCommand>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("fail"));
            var result = await _controller.UpsertRefreshALELastRunDateSetting(req);
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        }
    }
}
