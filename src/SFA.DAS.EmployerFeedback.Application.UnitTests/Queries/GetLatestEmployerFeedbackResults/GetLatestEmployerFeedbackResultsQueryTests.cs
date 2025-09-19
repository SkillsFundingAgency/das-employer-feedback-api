using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetLatestEmployerFeedbackResults;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetLatestEmployerFeedbackResults
{
    public class GetLatestEmployerFeedbackResultsQueryTest
    {
        [Test]
        public void CanInstantiate_GetLatestEmployerFeedbackResultsQuery()
        {
            var query = new GetLatestEmployerFeedbackResultsQuery();
            Assert.That(query, Is.Not.Null);
        }
    }
}
