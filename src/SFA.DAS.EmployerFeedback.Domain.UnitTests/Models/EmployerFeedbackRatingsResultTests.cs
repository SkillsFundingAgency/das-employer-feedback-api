using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Domain.Models;

namespace SFA.DAS.EmployerFeedback.Domain.UnitTests.Models
{
    public class EmployerFeedbackRatingsResultTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var model = new EmployerFeedbackRatingsResult
            {
                Ukprn = 12345678,
                Stars = 5,
                ReviewCount = 10
            };
            Assert.That(model.Ukprn, Is.EqualTo(12345678));
            Assert.That(model.Stars, Is.EqualTo(5));
            Assert.That(model.ReviewCount, Is.EqualTo(10));
        }
    }
}
