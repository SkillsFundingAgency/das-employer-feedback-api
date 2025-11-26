using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Commands.UpdateFeedbackTransaction;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Commands.UpdateFeedbackTransaction
{
    [TestFixture]
    public class UpdateFeedbackTransactionCommandHandlerTests
    {
        private Mock<IFeedbackTransactionContext> _mockFeedbackTransactionContext;
        private Mock<ILogger<UpdateFeedbackTransactionCommandHandler>> _mockLogger;
        private UpdateFeedbackTransactionCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _mockFeedbackTransactionContext = new Mock<IFeedbackTransactionContext>();
            _mockLogger = new Mock<ILogger<UpdateFeedbackTransactionCommandHandler>>();
            _handler = new UpdateFeedbackTransactionCommandHandler(
                _mockFeedbackTransactionContext.Object,
                _mockLogger.Object);
        }

        [Test]
        public async Task Handle_WhenTransactionExists_ShouldUpdateTransaction()
        {
            var id = 123L;
            var templateId = Guid.NewGuid();
            var sentCount = 5;
            var sentDate = DateTime.UtcNow;

            var existingTransaction = new FeedbackTransaction
            {
                Id = id,
                AccountId = 456L,
                TemplateName = "Original Template",
                SendAfter = DateTime.UtcNow.AddDays(30),
                TemplateId = Guid.NewGuid(),
                SentCount = 1,
                SentDate = DateTime.UtcNow.AddDays(-1),
                CreatedOn = DateTime.UtcNow.AddDays(-10)
            };

            var command = new UpdateFeedbackTransactionCommand
            {
                Id = id,
                TemplateId = templateId,
                SentCount = sentCount,
                SentDate = sentDate
            };

            _mockFeedbackTransactionContext
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingTransaction);

            await _handler.Handle(command, CancellationToken.None);

            existingTransaction.TemplateId.Should().Be(templateId);
            existingTransaction.SentCount.Should().Be(sentCount);
            existingTransaction.SentDate.Should().Be(sentDate);

            _mockFeedbackTransactionContext.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        public async Task Handle_WhenTransactionDoesNotExist_ShouldThrowInvalidOperationException()
        {
            var id = 123L;
            var command = new UpdateFeedbackTransactionCommand
            {
                Id = id,
                TemplateId = Guid.NewGuid(),
                SentCount = 1,
                SentDate = DateTime.UtcNow
            };

            _mockFeedbackTransactionContext
                .Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((FeedbackTransaction)null);

            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"FeedbackTransaction with Id {id} not found");

            _mockFeedbackTransactionContext.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}