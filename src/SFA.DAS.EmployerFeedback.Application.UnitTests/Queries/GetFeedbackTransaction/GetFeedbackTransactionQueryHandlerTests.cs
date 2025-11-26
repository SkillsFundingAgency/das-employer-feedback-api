using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetFeedbackTransaction;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetFeedbackTransaction
{
    [TestFixture]
    public class GetFeedbackTransactionQueryHandlerTests
    {
        private Mock<IFeedbackTransactionContext> _mockFeedbackTransactionContext;
        private Mock<ILogger<GetFeedbackTransactionQueryHandler>> _mockLogger;
        private GetFeedbackTransactionQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _mockFeedbackTransactionContext = new Mock<IFeedbackTransactionContext>();
            _mockLogger = new Mock<ILogger<GetFeedbackTransactionQueryHandler>>();

            _handler = new GetFeedbackTransactionQueryHandler(
                _mockFeedbackTransactionContext.Object,
                _mockLogger.Object);
        }

        [Test, AutoData]
        public async Task Handle_WhenFeedbackTransactionExists_ShouldReturnMappedResult(long feedbackTransactionId)
        {
            var account = new Account
            {
                Id = 456L,
                AccountName = "Test Account Name"
            };

            var feedbackTransaction = new FeedbackTransaction
            {
                Id = feedbackTransactionId,
                AccountId = 456L,
                Account = account,
                TemplateName = "Test Template",
                TemplateId = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow.AddDays(-10),
                SendAfter = DateTime.UtcNow.AddDays(30),
                SentCount = 3,
                SentDate = DateTime.UtcNow.AddDays(-1)
            };

            var query = new GetFeedbackTransactionQuery { Id = feedbackTransactionId };

            _mockFeedbackTransactionContext
                .Setup(x => x.GetByIdWithAccountAsync(feedbackTransactionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedbackTransaction);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Id.Should().Be(feedbackTransaction.Id);
            result.AccountId.Should().Be(feedbackTransaction.AccountId);
            result.AccountName.Should().Be(feedbackTransaction.Account.AccountName);
            result.TemplateName.Should().Be(feedbackTransaction.TemplateName);
            result.TemplateId.Should().Be(feedbackTransaction.TemplateId);
            result.CreatedOn.Should().Be(feedbackTransaction.CreatedOn);
            result.SendAfter.Should().Be(feedbackTransaction.SendAfter);
            result.SentCount.Should().Be(feedbackTransaction.SentCount);
            result.SentDate.Should().Be(feedbackTransaction.SentDate);
        }

        [Test, AutoData]
        public async Task Handle_WhenFeedbackTransactionNotFound_ShouldReturnNull(long feedbackTransactionId)
        {
            var query = new GetFeedbackTransactionQuery { Id = feedbackTransactionId };

            _mockFeedbackTransactionContext
                .Setup(x => x.GetByIdWithAccountAsync(feedbackTransactionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((FeedbackTransaction)null);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.Should().BeNull();
            _mockFeedbackTransactionContext.Verify(
                x => x.GetByIdWithAccountAsync(feedbackTransactionId, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test, AutoData]
        public void Handle_WhenExceptionThrown_ShouldLogErrorAndRethrow(long feedbackTransactionId)
        {
            var query = new GetFeedbackTransactionQuery { Id = feedbackTransactionId };
            var expectedException = new Exception("Database error");

            _mockFeedbackTransactionContext
                .Setup(x => x.GetByIdWithAccountAsync(feedbackTransactionId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(expectedException);

            var exception = Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
            exception.Should().NotBeNull();
            exception.Message.Should().Be("Database error");
        }
    }
}