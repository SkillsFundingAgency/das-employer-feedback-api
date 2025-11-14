using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetYearlyEmployerFeedbackResult;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetYearlyEmployerFeedbackResult
{
    public class GetYearlyEmployerFeedbackResultQueryTests
    {
        [Test]
        public void CanInstantiate_GetYearlyEmployerFeedbackResultQuery()
        {
            var query = new GetYearlyEmployerFeedbackResultQuery();
            Assert.That(query, Is.Not.Null);
        }
    }
}
