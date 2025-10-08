using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Api.Controllers;
using SFA.DAS.EmployerFeedback.Application.Commands.UpsertAccounts;
using SFA.DAS.EmployerFeedback.Application.Commands.UpsertFeedbackTransaction;
using SFA.DAS.EmployerFeedback.Application.Models;
using SFA.DAS.EmployerFeedback.Application.Queries.GetAccountsBatch;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

        [Test]
        public async Task GetAccountsBatch_ReturnsOk_WhenSuccessful()
        {
            var batchSize = 5;
            var expectedResult = new GetAccountsBatchQueryResult
            {
                AccountIds = new List<long> { 1, 2, 3, 4, 5 }
            };
            _mediator.Setup(m => m.Send(It.IsAny<GetAccountsBatchQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResult);

            var result = await _controller.GetAccountsBatch(batchSize);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(expectedResult, okResult.Value);
            _mediator.Verify(m => m.Send(It.Is<GetAccountsBatchQuery>(q => q.BatchSize == batchSize), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetAccountsBatch_ReturnsBadRequest_WhenBatchSizeIsZero()
        {
            var result = await _controller.GetAccountsBatch(0);

            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequest.StatusCode);
            Assert.AreEqual("Batch size must be greater than zero.", badRequest.Value);
        }

        [Test]
        public async Task GetAccountsBatch_ReturnsBadRequest_WhenBatchSizeIsNegative()
        {
            var result = await _controller.GetAccountsBatch(-1);

            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequest.StatusCode);
            Assert.AreEqual("Batch size must be greater than zero.", badRequest.Value);
        }

        [Test]
        public async Task GetAccountsBatch_ReturnsInternalServerError_WhenExceptionThrown()
        {
            var batchSize = 5;
            _mediator.Setup(m => m.Send(It.IsAny<GetAccountsBatchQuery>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("fail"));

            var result = await _controller.GetAccountsBatch(batchSize);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.AreEqual("An unexpected error occurred.", objectResult.Value);
        }

        [Test]
        public async Task UpsertFeedbackTransaction_WhenRequestIsValid_ShouldReturnNoContent()
        {
            long accountId = 12345;
            var request = new UpsertFeedbackTransactionRequest();
            _mediator.Setup(x => x.Send(It.IsAny<UpsertFeedbackTransactionCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            var result = await _controller.UpsertFeedbackTransaction(accountId, request);

            result.Should().BeOfType<NoContentResult>();
            _mediator.Verify(x => x.Send(
                It.Is<UpsertFeedbackTransactionCommand>(c => c.AccountId == accountId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task UpsertFeedbackTransaction_WhenValidationExceptionOccurs_ShouldReturnBadRequest()
        {
            long accountId = 12345;
            var request = new UpsertFeedbackTransactionRequest();
            string validationMessage = "Validation failed";
            _mediator.Setup(x => x.Send(It.IsAny<UpsertFeedbackTransactionCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException(validationMessage));

            var result = await _controller.UpsertFeedbackTransaction(accountId, request);

            result.Should().BeOfType<BadRequestObjectResult>();
            ((BadRequestObjectResult)result).Value.Should().Be(validationMessage);
        }

        [Test]
        public async Task UpsertFeedbackTransaction_WhenUnexpectedExceptionOccurs_ShouldReturnInternalServerError()
        {
            long accountId = 12345;
            var request = new UpsertFeedbackTransactionRequest();
            _mediator.Setup(x => x.Send(It.IsAny<UpsertFeedbackTransactionCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            var result = await _controller.UpsertFeedbackTransaction(accountId, request);

            result.Should().BeOfType<ObjectResult>();
            ((ObjectResult)result).StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            ((ObjectResult)result).Value.Should().Be("An unexpected error occurred.");
        }

        [Test]
        public async Task UpsertFeedbackTransaction_WhenRequestHasEmptyArrays_ShouldStillPassToCommand()
        {
            long accountId = 12345;
            var request = new UpsertFeedbackTransactionRequest
            {
                Active = new List<ProviderCourseDto>(),
                Completed = new List<ProviderCourseDto>(),
                NewStart = new List<ProviderCourseDto>()
            };

            _mediator.Setup(x => x.Send(It.IsAny<UpsertFeedbackTransactionCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            var result = await _controller.UpsertFeedbackTransaction(accountId, request);

            result.Should().BeOfType<NoContentResult>();
            _mediator.Verify(x => x.Send(
                It.Is<UpsertFeedbackTransactionCommand>(c =>
                    c.AccountId == accountId &&
                    c.Active.Count == 0 &&
                    c.Completed.Count == 0 &&
                    c.NewStart.Count == 0
                ),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
