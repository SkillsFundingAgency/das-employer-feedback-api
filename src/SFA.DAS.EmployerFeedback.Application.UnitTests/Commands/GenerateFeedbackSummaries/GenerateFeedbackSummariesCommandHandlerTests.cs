using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Commands.GenerateFeedbackSummaries;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Commands.GenerateFeedbackSummaries
{
    public class GenerateFeedbackSummariesCommandHandlerTests
    {
        [Test]
        public void Handle_CallsGenerateFeedbackSummaries_CompletesSuccessfully()
        {
            var mockContext = new Mock<IProviderRatingSummaryContext>();
            mockContext.Setup(x => x.GenerateFeedbackSummaries()).Returns(Task.CompletedTask);

            var mockLogger = new Mock<ILogger<GenerateFeedbackSummariesCommandHandler>>();

            var handler = new GenerateFeedbackSummariesCommandHandler(
                mockContext.Object,
                mockLogger.Object);

            var command = new GenerateFeedbackSummariesCommand();
            var cancellationToken = new CancellationToken();

            Assert.DoesNotThrowAsync(async () =>
                await handler.Handle(command, cancellationToken));
            mockContext.Verify(x => x.GenerateFeedbackSummaries(), Times.Once);
        }

        [Test]
        public void Handle_ThrowsException_WhenContextThrows()
        {
            var mockContext = new Mock<IProviderRatingSummaryContext>();
            mockContext.Setup(x => x.GenerateFeedbackSummaries()).ThrowsAsync(new Exception("DB error"));
            var mockLogger = new Mock<ILogger<GenerateFeedbackSummariesCommandHandler>>();
            var handler = new GenerateFeedbackSummariesCommandHandler(
                mockContext.Object,
                mockLogger.Object);
            Assert.ThrowsAsync<Exception>(async () =>
                await handler.Handle(new GenerateFeedbackSummariesCommand(), CancellationToken.None));
        }
    }
}