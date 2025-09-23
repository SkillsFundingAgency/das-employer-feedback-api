using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetOverallEmployerFeedbackResult;
using SFA.DAS.EmployerFeedback.Domain.Models;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetOverallEmployerFeedbackResult
{
    public class GetOverallEmployerFeedbackResultQueryResultTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var model = new OverallEmployerFeedbackResult { Ukprn = 123, Stars = 5, ReviewCount = 10, TimePeriod = "All", ProviderAttribute = null };
            var result = new GetOverallEmployerFeedbackResultQueryResult { OverallEmployerFeedback = model };
            Assert.That(result.OverallEmployerFeedback, Is.EqualTo(model));
        }
    }
}
