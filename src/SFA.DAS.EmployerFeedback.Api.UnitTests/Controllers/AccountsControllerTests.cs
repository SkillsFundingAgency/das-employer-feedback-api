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
using SFA.DAS.EmployerFeedback.Application.Commands.UpsertAccounts;
using SFA.DAS.EmployerFeedback.Application.Models;

namespace SFA.DAS.EmployerFeedback.Api.UnitTests.Controllers
{
    [TestFixture]
    public class AccountsControllerTests
    {
        private Mock<IMediator> _mediator;
        private Mock<ILogger<AccountsController>> _logger;
        private AccountsController _controller;

        [SetUp]
        public void SetUp()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<AccountsController>>();
            _controller = new AccountsController(_mediator.Object, _logger.Object);
        }

        [Test]
        public async Task UpsertAccounts_ReturnsNoContent_WhenSuccessful()
        {
            var request = new UpsertAccountsRequest { Accounts = new List<AccountUpsertDto> { new AccountUpsertDto { AccountId = 1, AccountName = "Test" } } };
            _mediator.Setup(m => m.Send(It.IsAny<UpsertAccountsCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(Unit.Value);

            var result = await _controller.UpsertAccounts(request);

            Assert.IsInstanceOf<NoContentResult>(result);
            _mediator.Verify(m => m.Send(It.IsAny<UpsertAccountsCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task UpsertAccounts_ReturnsBadRequest_WhenValidationExceptionThrown()
        {
            var request = new UpsertAccountsRequest();
            _mediator.Setup(m => m.Send(It.IsAny<UpsertAccountsCommand>(), It.IsAny<CancellationToken>())).ThrowsAsync(new ValidationException("validation failed"));

            var result = await _controller.UpsertAccounts(request);

            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequest.StatusCode);
            Assert.AreEqual("validation failed", badRequest.Value);
        }

        [Test]
        public async Task UpsertAccounts_ReturnsInternalServerError_WhenExceptionThrown()
        {
            var request = new UpsertAccountsRequest();
            _mediator.Setup(m => m.Send(It.IsAny<UpsertAccountsCommand>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("fail"));

            var result = await _controller.UpsertAccounts(request);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.AreEqual("An unexpected error occurred.", objectResult.Value);
        }
    }
}
