using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackRatingsResult;
using SFA.DAS.EmployerFeedback.Domain.Models;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetEmployerFeedbackRatingsResult
{
    public class GetEmployerFeedbackRatingsResultQueryResultTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var ratings = new List<EmployerFeedbackRatingsResult>();
            var result = new GetEmployerFeedbackRatingsResultQueryResult { EmployerFeedbackRatings = ratings };
            Assert.That(result.EmployerFeedbackRatings, Is.EqualTo(ratings));
        }
    }
}
