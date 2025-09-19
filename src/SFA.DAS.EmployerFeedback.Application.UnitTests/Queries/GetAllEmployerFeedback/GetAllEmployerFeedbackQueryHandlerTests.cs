using NUnit.Framework;
using Moq;
using SFA.DAS.EmployerFeedback.Application.Queries.GetAllEmployerFeedback;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using SFA.DAS.EmployerFeedback.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetAllEmployerFeedback
{
    public class GetAllEmployerFeedbackQueryHandlerTests
    {
        [Test]
        public async Task Handle_ReturnsMappedFeedbacks()
        {
            var feedbacks = new List<AllEmployerFeedbackResults>
            {
                new AllEmployerFeedbackResults { Ukprn = 1, ProviderRating = "Good", ProviderAttributes = new List<ProviderAttributeResults>() }
            };
            var mockContext = new Mock<IEmployerFeedbackContext>();
            mockContext.Setup(x => x.GetAllEmployerFeedbackAsync(It.IsAny<CancellationToken>())).ReturnsAsync(feedbacks);
            var handler = new GetAllEmployerFeedbackQueryHandler(mockContext.Object);
            var result = await handler.Handle(new GetAllEmployerFeedbackQuery(), CancellationToken.None);
            Assert.That(result.Feedbacks, Is.EqualTo(feedbacks));
            mockContext.Verify(x => x.GetAllEmployerFeedbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ReturnsEmptyList_WhenNoFeedbacksExist()
        {
            var mockContext = new Mock<IEmployerFeedbackContext>();
            mockContext.Setup(x => x.GetAllEmployerFeedbackAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<AllEmployerFeedbackResults>());
            var handler = new GetAllEmployerFeedbackQueryHandler(mockContext.Object);
            var result = await handler.Handle(new GetAllEmployerFeedbackQuery(), CancellationToken.None);
            Assert.That(result.Feedbacks, Is.Empty);
            mockContext.Verify(x => x.GetAllEmployerFeedbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void Handle_ThrowsException_WhenContextThrows()
        {
            var mockContext = new Mock<IEmployerFeedbackContext>();
            mockContext.Setup(x => x.GetAllEmployerFeedbackAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new System.Exception("DB error"));
            var handler = new GetAllEmployerFeedbackQueryHandler(mockContext.Object);
            Assert.ThrowsAsync<System.Exception>(async () =>
                await handler.Handle(new GetAllEmployerFeedbackQuery(), CancellationToken.None));
            mockContext.Verify(x => x.GetAllEmployerFeedbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
