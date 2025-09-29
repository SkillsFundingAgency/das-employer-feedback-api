using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetYearlyEmployerFeedbackResult;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using SFA.DAS.EmployerFeedback.Domain.Models;
using SFA.DAS.EmployerFeedback.Domain.Entities;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetYearlyEmployerFeedbackResult
{
    public class GetYearlyEmployerFeedbackResultQueryHandlerTests
    {
        [Test]
        public async Task Handle_ReturnsMappedResults()
        {
            var ukprn = 1234L;
            var starsSummaries = new List<ProviderStarsSummary> {
                new ProviderStarsSummary { Ukprn = ukprn, Stars = 4, ReviewCount = 10, TimePeriod = "2023" }
            };
            var attributeSummaries = new List<ProviderAttributeSummary> {
                new ProviderAttributeSummary { Ukprn = ukprn, TimePeriod = "2023", Attribute = new Domain.Entities.Attribute { AttributeName = "Quality" }, Strength = 2, Weakness = 1 }
            };
            var mockStarsContext = new Mock<IProviderStarsSummaryContext>();
            var mockAttrContext = new Mock<IProviderAttributeSummaryContext>();
            mockStarsContext.Setup(x => x.GetProviderStarsSummaryByUkprnAsync(ukprn, It.IsAny<CancellationToken>())).ReturnsAsync(starsSummaries);
            mockAttrContext.Setup(x => x.GetProviderAttributeSummaryByUkprnAsync(ukprn, It.IsAny<CancellationToken>())).ReturnsAsync(attributeSummaries);
            var handler = new GetYearlyEmployerFeedbackResultQueryHandler(mockStarsContext.Object, mockAttrContext.Object);
            var result = await handler.Handle(new GetYearlyEmployerFeedbackResultQuery { Ukprn = ukprn }, CancellationToken.None);
            Assert.That(result.AnnualEmployerFeedbackDetails.Count, Is.EqualTo(1));
            Assert.That(result.AnnualEmployerFeedbackDetails[0].Ukprn, Is.EqualTo(ukprn));
            Assert.That(result.AnnualEmployerFeedbackDetails[0].Stars, Is.EqualTo(4));
            Assert.That(result.AnnualEmployerFeedbackDetails[0].ProviderAttribute.Count, Is.EqualTo(1));
            mockStarsContext.Verify(x => x.GetProviderStarsSummaryByUkprnAsync(ukprn, It.IsAny<CancellationToken>()), Times.Once);
            mockAttrContext.Verify(x => x.GetProviderAttributeSummaryByUkprnAsync(ukprn, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ReturnsEmptyList_WhenNoData()
        {
            var ukprn = 1234L;
            var mockStarsContext = new Mock<IProviderStarsSummaryContext>();
            var mockAttrContext = new Mock<IProviderAttributeSummaryContext>();
            mockStarsContext.Setup(x => x.GetProviderStarsSummaryByUkprnAsync(ukprn, It.IsAny<CancellationToken>())).ReturnsAsync(new List<ProviderStarsSummary>());
            mockAttrContext.Setup(x => x.GetProviderAttributeSummaryByUkprnAsync(ukprn, It.IsAny<CancellationToken>())).ReturnsAsync(new List<ProviderAttributeSummary>());
            var handler = new GetYearlyEmployerFeedbackResultQueryHandler(mockStarsContext.Object, mockAttrContext.Object);
            var result = await handler.Handle(new GetYearlyEmployerFeedbackResultQuery { Ukprn = ukprn }, CancellationToken.None);
            Assert.That(result.AnnualEmployerFeedbackDetails, Is.Empty);
        }

        [Test]
        public void Handle_ThrowsException_WhenContextThrows()
        {
            var ukprn = 1234L;
            var mockStarsContext = new Mock<IProviderStarsSummaryContext>();
            var mockAttrContext = new Mock<IProviderAttributeSummaryContext>();
            mockStarsContext.Setup(x => x.GetProviderStarsSummaryByUkprnAsync(ukprn, It.IsAny<CancellationToken>())).ThrowsAsync(new System.Exception("DB error"));
            var handler = new GetYearlyEmployerFeedbackResultQueryHandler(mockStarsContext.Object, mockAttrContext.Object);
            Assert.ThrowsAsync<System.Exception>(async () =>
                await handler.Handle(new GetYearlyEmployerFeedbackResultQuery { Ukprn = ukprn }, CancellationToken.None));
        }
    }
}
