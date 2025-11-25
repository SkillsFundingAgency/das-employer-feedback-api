using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackResultForAcademicYear;
using SFA.DAS.EmployerFeedback.Domain.Interfaces;
using SFA.DAS.EmployerFeedback.Domain.Models;
using SFA.DAS.EmployerFeedback.Domain.Entities;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetEmployerFeedbackResultForAcademicYear
{
    public class GetEmployerFeedbackResultForAcademicYearQueryHandlerTests
    {
        [Test]
        public async Task Handle_ReturnsMappedResult()
        {
            var ukprn = 1234L;
            var timePeriod = "AY2023";
            var starsSummaries = new List<ProviderStarsSummary> {
                new ProviderStarsSummary { Ukprn = ukprn, Stars = 4, ReviewCount = 10, TimePeriod = timePeriod }
            };
            var attributeSummaries = new List<ProviderAttributeSummary> {
                new ProviderAttributeSummary { Ukprn = ukprn, TimePeriod = timePeriod, Attribute = new Domain.Entities.Attribute { AttributeName = "Quality" }, Strength = 2, Weakness = 1 }
            };
            var mockStarsContext = new Mock<IProviderStarsSummaryContext>();
            var mockAttrContext = new Mock<IProviderAttributeSummaryContext>();
            mockStarsContext.Setup(x => x.GetProviderStarsSummaryByUkprnAndTimePeriodAsync(ukprn, timePeriod, It.IsAny<CancellationToken>())).ReturnsAsync(starsSummaries);
            mockAttrContext.Setup(x => x.GetProviderAttributeSummaryByUkprnAndTimePeriodAsync(ukprn, timePeriod, It.IsAny<CancellationToken>())).ReturnsAsync(attributeSummaries);
            var handler = new GetEmployerFeedbackResultForAcademicYearQueryHandler(mockStarsContext.Object, mockAttrContext.Object);
            var result = await handler.Handle(new GetEmployerFeedbackResultForAcademicYearQuery { Ukprn = ukprn, TimePeriod = timePeriod }, CancellationToken.None);
            Assert.That(result.EmployerFeedbackForAcademicYear.Ukprn, Is.EqualTo(ukprn));
            Assert.That(result.EmployerFeedbackForAcademicYear.Stars, Is.EqualTo(4));
            Assert.That(result.EmployerFeedbackForAcademicYear.ProviderAttribute.Count, Is.EqualTo(1));
            mockStarsContext.Verify(x => x.GetProviderStarsSummaryByUkprnAndTimePeriodAsync(ukprn, timePeriod, It.IsAny<CancellationToken>()), Times.Once);
            mockAttrContext.Verify(x => x.GetProviderAttributeSummaryByUkprnAndTimePeriodAsync(ukprn, timePeriod, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_ReturnsEmptyResult_WhenNoData()
        {
            var ukprn = 1234L;
            var timePeriod = "AY2023";
            var mockStarsContext = new Mock<IProviderStarsSummaryContext>();
            var mockAttrContext = new Mock<IProviderAttributeSummaryContext>();
            mockStarsContext.Setup(x => x.GetProviderStarsSummaryByUkprnAndTimePeriodAsync(ukprn, timePeriod, It.IsAny<CancellationToken>())).ReturnsAsync(new List<ProviderStarsSummary>());
            mockAttrContext.Setup(x => x.GetProviderAttributeSummaryByUkprnAndTimePeriodAsync(ukprn, timePeriod, It.IsAny<CancellationToken>())).ReturnsAsync(new List<ProviderAttributeSummary>());
            var handler = new GetEmployerFeedbackResultForAcademicYearQueryHandler(mockStarsContext.Object, mockAttrContext.Object);
            var result = await handler.Handle(new GetEmployerFeedbackResultForAcademicYearQuery { Ukprn = ukprn, TimePeriod = timePeriod }, CancellationToken.None);
            Assert.That(result.EmployerFeedbackForAcademicYear.Ukprn, Is.EqualTo(ukprn));
            Assert.That(result.EmployerFeedbackForAcademicYear.TimePeriod, Is.EqualTo(timePeriod));
            Assert.That(result.EmployerFeedbackForAcademicYear.ProviderAttribute, Is.Empty);
        }

        [Test]
        public void Handle_ThrowsException_WhenContextThrows()
        {
            var ukprn = 1234L;
            var timePeriod = "AY2023";
            var mockStarsContext = new Mock<IProviderStarsSummaryContext>();
            var mockAttrContext = new Mock<IProviderAttributeSummaryContext>();
            mockStarsContext.Setup(x => x.GetProviderStarsSummaryByUkprnAndTimePeriodAsync(ukprn, timePeriod, It.IsAny<CancellationToken>())).ThrowsAsync(new System.Exception("DB error"));
            var handler = new GetEmployerFeedbackResultForAcademicYearQueryHandler(mockStarsContext.Object, mockAttrContext.Object);
            Assert.ThrowsAsync<System.Exception>(async () =>
                await handler.Handle(new GetEmployerFeedbackResultForAcademicYearQuery { Ukprn = ukprn, TimePeriod = timePeriod }, CancellationToken.None));
        }
    }
}
