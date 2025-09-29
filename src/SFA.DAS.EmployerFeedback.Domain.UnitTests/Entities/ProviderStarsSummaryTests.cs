using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Domain.Entities;

namespace SFA.DAS.EmployerFeedback.Domain.UnitTests.Entities
{
    public class ProviderStarsSummaryTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var summary = new ProviderStarsSummary
            {
                Ukprn = 12345678,
                Stars = 5,
                ReviewCount = 20,
                TimePeriod = "AY2023"
            };
            Assert.That(summary.Ukprn, Is.EqualTo(12345678));
            Assert.That(summary.Stars, Is.EqualTo(5));
            Assert.That(summary.ReviewCount, Is.EqualTo(20));
            Assert.That(summary.TimePeriod, Is.EqualTo("AY2023"));
        }
    }
}
