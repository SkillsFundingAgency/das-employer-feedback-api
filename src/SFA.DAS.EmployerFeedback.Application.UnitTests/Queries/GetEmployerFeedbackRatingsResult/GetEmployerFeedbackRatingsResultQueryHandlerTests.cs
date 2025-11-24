using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackRatingsResult;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using SFA.DAS.EmployerFeedback.Domain.Entities;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetEmployerFeedbackRatingsResult
{
    public class GetEmployerFeedbackRatingsResultQueryHandlerTests
    {
        [Test]
        public async Task Handle_ReturnsMappedResult()
        {
            var timePeriod = "AY2023";
            var starsSummaries = new List<ProviderStarsSummary>
            {
                new ProviderStarsSummary { Ukprn = 12345678, Stars = 5, ReviewCount = 10, TimePeriod = timePeriod }
            };
            var mockContext = new Mock<IProviderStarsSummaryContext>();
            mockContext.Setup(x => x.GetProviderStarsSummaryByTimePeriodAsync(timePeriod, It.IsAny<CancellationToken>())).ReturnsAsync(starsSummaries);
            var handler = new GetEmployerFeedbackRatingsResultQueryHandler(mockContext.Object);
            var result = await handler.Handle(new GetEmployerFeedbackRatingsResultQuery { TimePeriod = timePeriod }, CancellationToken.None);
            Assert.That(result.EmployerFeedbackRatings.Count, Is.EqualTo(1));
            Assert.That(result.EmployerFeedbackRatings[0].Ukprn, Is.EqualTo(12345678));
            Assert.That(result.EmployerFeedbackRatings[0].Stars, Is.EqualTo(5));
            Assert.That(result.EmployerFeedbackRatings[0].ReviewCount, Is.EqualTo(10));
            mockContext.Verify(x => x.GetProviderStarsSummaryByTimePeriodAsync(timePeriod, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ReturnsEmptyList_WhenNoData()
        {
            var timePeriod = "AY2023";
            var mockContext = new Mock<IProviderStarsSummaryContext>();
            mockContext.Setup(x => x.GetProviderStarsSummaryByTimePeriodAsync(timePeriod, It.IsAny<CancellationToken>())).ReturnsAsync(new List<ProviderStarsSummary>());
            var handler = new GetEmployerFeedbackRatingsResultQueryHandler(mockContext.Object);
            var result = await handler.Handle(new GetEmployerFeedbackRatingsResultQuery { TimePeriod = timePeriod }, CancellationToken.None);
            Assert.That(result.EmployerFeedbackRatings, Is.Empty);
            mockContext.Verify(x => x.GetProviderStarsSummaryByTimePeriodAsync(timePeriod, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
