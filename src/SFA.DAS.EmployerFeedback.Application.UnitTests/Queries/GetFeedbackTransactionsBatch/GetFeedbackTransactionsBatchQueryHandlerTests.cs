using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetFeedbackTransactionsBatch;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetFeedbackTransactionsBatch
{
    [TestFixture]
    public class GetFeedbackTransactionsBatchQueryHandlerTests
    {
        private Mock<IFeedbackTransactionContext> _feedbackTransactionContext;
        private GetFeedbackTransactionsBatchQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _feedbackTransactionContext = new Mock<IFeedbackTransactionContext>();
            _handler = new GetFeedbackTransactionsBatchQueryHandler(_feedbackTransactionContext.Object);
        }

        [Test]
        public async Task Handle_ReturnsFeedbackTransactionIds_WhenSuccessful()
        {
            var batchSize = 5;
            var expectedFeedbackTransactionIds = new List<long> { 1, 2, 3, 4, 5 };
            var query = new GetFeedbackTransactionsBatchQuery { BatchSize = batchSize };

            _feedbackTransactionContext.Setup(x => x.GetFeedbackTransactionsBatchAsync(batchSize, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedFeedbackTransactionIds);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.FeedbackTransactions.Should().BeEquivalentTo(expectedFeedbackTransactionIds);
            _feedbackTransactionContext.Verify(x => x.GetFeedbackTransactionsBatchAsync(batchSize, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ReturnsEmptyList_WhenNoFeedbackTransactionsAvailable()
        {
            var batchSize = 5;
            var query = new GetFeedbackTransactionsBatchQuery { BatchSize = batchSize };

            _feedbackTransactionContext.Setup(x => x.GetFeedbackTransactionsBatchAsync(batchSize, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<long>());

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.FeedbackTransactions.Should().BeEmpty();
            _feedbackTransactionContext.Verify(x => x.GetFeedbackTransactionsBatchAsync(batchSize, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}