using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetOverallEmployerFeedbackResult;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetOverallEmployerFeedbackResult
{
    public class GetOverallEmployerFeedbackResultQueryTests
    {
        [Test]
        public void CanInstantiate_GetOverallEmployerFeedbackResultQuery()
        {
            var query = new GetOverallEmployerFeedbackResultQuery();
            Assert.That(query, Is.Not.Null);
        }
    }
}
