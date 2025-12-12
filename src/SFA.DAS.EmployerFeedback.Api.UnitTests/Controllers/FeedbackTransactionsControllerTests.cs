using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Api.Controllers;
using SFA.DAS.EmployerFeedback.Application.Queries.GetFeedbackTransaction;
using SFA.DAS.EmployerFeedback.Application.Queries.GetFeedbackTransactionsBatch;
using System;
using System.Collections.Generic;
using SFA.DAS.EmployerFeedback.Application.Commands.UpdateFeedbackTransaction;
using SFA.DAS.EmployerFeedback.Application.Models;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;

namespace SFA.DAS.EmployerFeedback.Api.UnitTests.Controllers
{
    [TestFixture]
    public class FeedbackTransactionsControllerTests
    {
        private Mock<IMediator> _mediator;
        private Mock<ILogger<FeedbackTransactionsController>> _logger;
        private FeedbackTransactionsController _controller;

        [SetUp]
        public void SetUp()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<FeedbackTransactionsController>>();
            _controller = new FeedbackTransactionsController(_mediator.Object, _logger.Object);
        }

        [Test]
        public async Task GetFeedbackTransactionsBatch_ReturnsOk_WhenSuccessful()
        {
            var batchSize = 5;
            var expectedResult = new GetFeedbackTransactionsBatchQueryResult
            {
                FeedbackTransactions = new List<long> { 1, 2, 3, 4, 5 }
            };
            _mediator.Setup(m => m.Send(It.IsAny<GetFeedbackTransactionsBatchQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResult);

            var result = await _controller.GetFeedbackTransactionsBatch(batchSize);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.AreEqual(expectedResult, okResult.Value);
            _mediator.Verify(m => m.Send(It.Is<GetFeedbackTransactionsBatchQuery>(q => q.BatchSize == batchSize), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetFeedbackTransactionsBatch_ReturnsEmptyList_WhenNoTransactionsAvailable()
        {
            var batchSize = 5;
            var expectedResult = new GetFeedbackTransactionsBatchQueryResult
            {
                FeedbackTransactions = new List<long>()
            };
            _mediator.Setup(m => m.Send(It.IsAny<GetFeedbackTransactionsBatchQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedResult);

            var result = await _controller.GetFeedbackTransactionsBatch(batchSize);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(StatusCodes.Status200OK, okResult.StatusCode);
            expectedResult.FeedbackTransactions.Should().BeEmpty();
            _mediator.Verify(m => m.Send(It.Is<GetFeedbackTransactionsBatchQuery>(q => q.BatchSize == batchSize), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetFeedbackTransactionsBatch_ReturnsBadRequest_WhenBatchSizeIsZero()
        {
            var result = await _controller.GetFeedbackTransactionsBatch(0);

            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequest.StatusCode);
            Assert.AreEqual("Batch size must be greater than zero.", badRequest.Value);
        }

        [Test]
        public async Task GetFeedbackTransactionsBatch_ReturnsBadRequest_WhenBatchSizeIsNegative()
        {
            var result = await _controller.GetFeedbackTransactionsBatch(-1);

            var badRequest = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual(StatusCodes.Status400BadRequest, badRequest.StatusCode);
            Assert.AreEqual("Batch size must be greater than zero.", badRequest.Value);
        }

        [Test]
        public async Task GetFeedbackTransactionsBatch_ReturnsInternalServerError_WhenExceptionThrown()
        {
            var batchSize = 5;
            _mediator.Setup(m => m.Send(It.IsAny<GetFeedbackTransactionsBatchQuery>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("fail"));

            var result = await _controller.GetFeedbackTransactionsBatch(batchSize);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.AreEqual("An unexpected error occurred.", objectResult.Value);
        }

        [Test]
        public async Task GetFeedbackTransaction_WhenFeedbackTransactionExists_ShouldReturnOkResult()
        {
            var id = 123;
            var accountId = 456;
            var accountName = "Test Account";
            var templateName = "Test Template";
            var queryResult = new GetFeedbackTransactionQueryResult
            {
                Id = id,
                AccountId = accountId,
                AccountName = accountName,
                TemplateName = templateName,
                CreatedOn = DateTime.UtcNow.AddDays(-10),
                SendAfter = DateTime.UtcNow.AddDays(30),
                TemplateId = Guid.NewGuid(),
                SentCount = 5,
                SentDate = DateTime.UtcNow.AddDays(-1)
            };

            _mediator.Setup(x => x.Send(It.Is<GetFeedbackTransactionQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var result = await _controller.GetFeedbackTransaction(id);

            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be(queryResult);
            _mediator.Verify(m => m.Send(It.Is<GetFeedbackTransactionQuery>(q => q.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetFeedbackTransaction_WhenFeedbackTransactionDoesNotExist_ShouldReturnBadRequest()
        {
            var id = 123;
            _mediator.Setup(x => x.Send(It.Is<GetFeedbackTransactionQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GetFeedbackTransactionQueryResult)null);

            var result = await _controller.GetFeedbackTransaction(id);

            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be($"Id {id} unknown");
            _mediator.Verify(m => m.Send(It.Is<GetFeedbackTransactionQuery>(q => q.Id == id), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetFeedbackTransaction_WhenIdIsZero_ShouldReturnBadRequest()
        {
            var id = 0;

            var result = await _controller.GetFeedbackTransaction(id);

            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be($"Id {id} unknown");

            _mediator.Verify(x => x.Send(It.IsAny<GetFeedbackTransactionQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task GetFeedbackTransaction_WhenIdIsNegative_ShouldReturnBadRequest()
        {
            var id = -1;

            var result = await _controller.GetFeedbackTransaction(id);

            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Value.Should().Be($"Id {id} unknown");

            _mediator.Verify(x => x.Send(It.IsAny<GetFeedbackTransactionQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task GetFeedbackTransaction_WhenExceptionOccurs_ShouldReturnInternalServerError()
        {
            var id = 123L;
            _mediator.Setup(x => x.Send(It.Is<GetFeedbackTransactionQuery>(q => q.Id == id), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            var result = await _controller.GetFeedbackTransaction(id);

            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
            objectResult.Value.Should().Be("An unexpected error occurred.");
        }

        [Test]
        public async Task UpdateFeedbackTransaction_ReturnsNoContent_WhenSuccessful()
        {
            var id = 123L;
            var request = new UpdateFeedbackTransactionRequest
            {
                TemplateId = Guid.NewGuid(),
                SentCount = 1,
                SentDate = DateTime.UtcNow
            };

            _mediator.Setup(m => m.Send(It.IsAny<UpdateFeedbackTransactionCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            var result = await _controller.UpdateFeedbackTransaction(id, request);

            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            Assert.AreEqual(204, noContentResult.StatusCode);

            _mediator.Verify(m => m.Send(It.Is<UpdateFeedbackTransactionCommand>(c =>
                c.Id == id &&
                c.TemplateId == request.TemplateId &&
                c.SentCount == request.SentCount &&
                c.SentDate == request.SentDate),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task UpdateFeedbackTransaction_ReturnsBadRequest_WhenValidationExceptionThrown()
        {
            var id = 123L;
            var request = new UpdateFeedbackTransactionRequest
            {
                TemplateId = Guid.NewGuid(),
                SentCount = 1,
                SentDate = DateTime.UtcNow
            };

            _mediator.Setup(m => m.Send(It.IsAny<UpdateFeedbackTransactionCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ValidationException("Validation failed"));

            var result = await _controller.UpdateFeedbackTransaction(id, request);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Validation failed", badRequestResult.Value);
        }

        [Test]
        public async Task UpdateFeedbackTransaction_ReturnsBadRequest_WhenTransactionNotFound()
        {
            var id = 123L;
            var request = new UpdateFeedbackTransactionRequest
            {
                TemplateId = Guid.NewGuid(),
                SentCount = 1,
                SentDate = DateTime.UtcNow
            };

            _mediator.Setup(m => m.Send(It.IsAny<UpdateFeedbackTransactionCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("FeedbackTransaction with Id 123 not found"));

            var result = await _controller.UpdateFeedbackTransaction(id, request);

            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("FeedbackTransaction with id 123 not found", badRequestResult.Value);
        }

        [Test]
        public async Task UpdateFeedbackTransaction_ReturnsInternalServerError_WhenUnexpectedExceptionThrown()
        {
            var id = 123L;
            var request = new UpdateFeedbackTransactionRequest
            {
                TemplateId = Guid.NewGuid(),
                SentCount = 1,
                SentDate = DateTime.UtcNow
            };

            _mediator.Setup(m => m.Send(It.IsAny<UpdateFeedbackTransactionCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            var result = await _controller.UpdateFeedbackTransaction(id, request);

            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.AreEqual("An unexpected error occurred.", objectResult.Value);
        }
    }
}