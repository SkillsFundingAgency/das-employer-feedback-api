using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetOverallEmployerFeedbackResult;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using SFA.DAS.EmployerFeedback.Domain.Entities;
using ESFA.DAS.ProvideFeedback.Data.Constants;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetOverallEmployerFeedbackResult
{
    public class GetOverallEmployerFeedbackResultQueryHandlerTests
    {
        [Test]
        public async Task Handle_ReturnsMappedResult()
        {
            var ukprn = 1234L;
            var attributeSummaries = new List<ProviderAttributeSummary> {
                new ProviderAttributeSummary { Attribute = new Domain.Entities.Attributes { AttributeName = "Quality" }, Strength = 2, Weakness = 1 }
            };
            var starsSummaries = new List<ProviderStarsSummary> {
                new ProviderStarsSummary { Ukprn = ukprn, Stars = 4, ReviewCount = 10, TimePeriod = "All" }
            };
            var mockAttrContext = new Mock<IProviderAttributeSummaryContext>();
            var mockStarsContext = new Mock<IProviderStarsSummaryContext>();
            mockAttrContext.Setup(x => x.GetProviderAttributeSummaryByUkprnAndTimePeriodAsync(
                ukprn, ReviewDataPeriod.All, It.IsAny<CancellationToken>())).ReturnsAsync(attributeSummaries);
            mockStarsContext.Setup(x => x.GetProviderStarsSummaryByUkprnAndTimePeriodAsync(
                ukprn, ReviewDataPeriod.All, It.IsAny<CancellationToken>())).ReturnsAsync(starsSummaries);
            var handler = new GetOverallEmployerFeedbackResultQueryHandler(mockStarsContext.Object, mockAttrContext.Object);
            var result = await handler.Handle(new GetOverallEmployerFeedbackResultQuery { Ukprn = ukprn }, CancellationToken.None);
            Assert.That(result.OverallEmployerFeedback.Ukprn, Is.EqualTo(ukprn));
            Assert.That(result.OverallEmployerFeedback.Stars, Is.EqualTo(4));
            Assert.That(result.OverallEmployerFeedback.ReviewCount, Is.EqualTo(10));
            Assert.That(result.OverallEmployerFeedback.ProviderAttribute.Count, Is.EqualTo(1));
            mockAttrContext.Verify(x => x.GetProviderAttributeSummaryByUkprnAndTimePeriodAsync(
                ukprn, ReviewDataPeriod.All, It.IsAny<CancellationToken>()), Times.Once);
            mockStarsContext.Verify(x => x.GetProviderStarsSummaryByUkprnAndTimePeriodAsync(
                ukprn, ReviewDataPeriod.All, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ReturnsEmptyResult_WhenNoData()
        {
            var ukprn = 1234L;
            var mockAttrContext = new Mock<IProviderAttributeSummaryContext>();
            var mockStarsContext = new Mock<IProviderStarsSummaryContext>();
            mockAttrContext.Setup(x => x.GetProviderAttributeSummaryByUkprnAndTimePeriodAsync(
                ukprn, ReviewDataPeriod.All, It.IsAny<CancellationToken>())).ReturnsAsync(new List<ProviderAttributeSummary>());
            mockStarsContext.Setup(x => x.GetProviderStarsSummaryByUkprnAndTimePeriodAsync(
                ukprn, ReviewDataPeriod.All, It.IsAny<CancellationToken>())).ReturnsAsync(new List<ProviderStarsSummary>());
            var handler = new GetOverallEmployerFeedbackResultQueryHandler(mockStarsContext.Object, mockAttrContext.Object);
            var result = await handler.Handle(new GetOverallEmployerFeedbackResultQuery { Ukprn = ukprn }, CancellationToken.None);
            Assert.That(result.OverallEmployerFeedback.Ukprn, Is.EqualTo(ukprn));
            Assert.That(result.OverallEmployerFeedback.ProviderAttribute, Is.Empty);
            mockAttrContext.Verify(x => x.GetProviderAttributeSummaryByUkprnAndTimePeriodAsync(
                ukprn, ReviewDataPeriod.All, It.IsAny<CancellationToken>()), Times.Once);
            mockStarsContext.Verify(x => x.GetProviderStarsSummaryByUkprnAndTimePeriodAsync(
                ukprn, ReviewDataPeriod.All, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void Handle_ThrowsException_WhenContextThrows()
        {
            var ukprn = 1234L;
            var mockAttrContext = new Mock<IProviderAttributeSummaryContext>();
            var mockStarsContext = new Mock<IProviderStarsSummaryContext>();
            mockAttrContext.Setup(x => x.GetProviderAttributeSummaryByUkprnAndTimePeriodAsync(
                ukprn, ReviewDataPeriod.All, It.IsAny<CancellationToken>())).ThrowsAsync(new System.Exception("DB error"));
            var handler = new GetOverallEmployerFeedbackResultQueryHandler(mockStarsContext.Object, mockAttrContext.Object);
            Assert.ThrowsAsync<System.Exception>(async () =>
                await handler.Handle(new GetOverallEmployerFeedbackResultQuery { Ukprn = ukprn }, CancellationToken.None));
        }
    }
}
