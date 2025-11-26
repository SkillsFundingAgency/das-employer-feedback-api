using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Api.Controllers;
using SFA.DAS.EmployerFeedback.Application.Queries.GetFeedbackTransactionsBatch;

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
    }
}