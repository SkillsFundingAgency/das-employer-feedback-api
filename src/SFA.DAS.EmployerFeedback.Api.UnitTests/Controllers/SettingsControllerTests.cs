using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using FluentAssertions;
using SFA.DAS.EmployerFeedback.Api.Controllers;
using SFA.DAS.EmployerFeedback.Application.Commands.UpsertRefreshALELastRunDateSetting;
using SFA.DAS.EmployerFeedback.Application.Models;
using SFA.DAS.EmployerFeedback.Application.Queries.GetRefreshALELastRunDateSetting;
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
        public async Task GetRefreshALELastRunDateSetting_Should_Return_Ok_With_Setting()
        {
            var nowString = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
            var setting = new GetRefreshALELastRunDateSettingQueryResult { Value = nowString };
            _mediator.Setup(x => x.Send(It.IsAny<GetRefreshALELastRunDateSettingQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(setting);

            var result = await _controller.GetRefreshALELastRunDateSetting();
            
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            var returned = okResult.Value as GetRefreshALELastRunDateSettingQueryResult;
            returned.Should().NotBeNull();
            returned.Value.Should().Be(nowString);
            _mediator.Verify(x => x.Send(It.IsAny<GetRefreshALELastRunDateSettingQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetRefreshALELastRunDateSetting_Should_Return_Null_When_No_Record()
        {
            var setting = new GetRefreshALELastRunDateSettingQueryResult { Value = null };
            _mediator.Setup(x => x.Send(It.IsAny<GetRefreshALELastRunDateSettingQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(setting);
            var result = await _controller.GetRefreshALELastRunDateSetting();
            
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
            var returned = okResult.Value as GetRefreshALELastRunDateSettingQueryResult;
            returned.Should().NotBeNull();
            returned.Value.Should().BeNull();
        }

        [Test]
        public async Task GetRefreshALELastRunDateSetting_Should_Return_InternalServerError_On_Exception()
        {
            _mediator.Setup(x => x.Send(It.IsAny<GetRefreshALELastRunDateSettingQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("fail"));
            var result = await _controller.GetRefreshALELastRunDateSetting();
            
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            objectResult.Value.Should().Be("An unexpected error occurred.");
        }

        [Test]
        public async Task UpsertSetting_Should_Send_Command_And_Return_NoContent()
        {
            var req = new SettingRequest { Value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) };
            _mediator.Setup(x => x.Send(It.Is<UpsertRefreshALELastRunDateSettingCommand>(c => c.Value == req.Value), It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);

            var result = await _controller.UpsertRefreshALELastRunDateSetting(req);
            
            result.Should().BeOfType<NoContentResult>();
            _mediator.Verify(x => x.Send(It.IsAny<UpsertRefreshALELastRunDateSettingCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task UpsertRefreshALELastRunDateSetting_Should_Return_BadRequest_On_ValidationException()
        {
            var req = new SettingRequest { Value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) };
            _mediator.Setup(x => x.Send(It.IsAny<UpsertRefreshALELastRunDateSettingCommand>(), It.IsAny<CancellationToken>())).ThrowsAsync(new ValidationException("validation failed"));
            var result = await _controller.UpsertRefreshALELastRunDateSetting(req);
            
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequest = result as BadRequestObjectResult;
            badRequest.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            badRequest.Value.Should().Be("validation failed");
        }

        [Test]
        public async Task UpsertRefreshALELastRunDateSetting_Should_Return_InternalServerError_On_Exception()
        {
            var req = new SettingRequest { Value = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) };
            _mediator.Setup(x => x.Send(It.IsAny<UpsertRefreshALELastRunDateSettingCommand>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("fail"));
            var result = await _controller.UpsertRefreshALELastRunDateSetting(req);
            
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        }
    }
}
