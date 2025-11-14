using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackRatingsResult;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetEmployerFeedbackRatingsResult
{
    public class GetEmployerFeedbackRatingsResultQueryTests
    {
        [Test]
        public void CanInstantiate_GetEmployerFeedbackRatingsResultQuery()
        {
            var query = new GetEmployerFeedbackRatingsResultQuery();
            Assert.That(query, Is.Not.Null);
        }
    }
}
