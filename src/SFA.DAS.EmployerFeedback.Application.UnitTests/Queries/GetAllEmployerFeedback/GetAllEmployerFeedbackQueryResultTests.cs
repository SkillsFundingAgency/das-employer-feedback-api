using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetAllEmployerFeedback;
using SFA.DAS.EmployerFeedback.Domain.Models;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetAllEmployerFeedback
{
    public class GetAllEmployerFeedbackQueryResultTests
    {
        [Test]
        public void CanInstantiate_GetAllEmployerFeedbackQueryResult()
        {
            var feedbacks = new List<AllEmployerFeedbackResults>();
            var result = new GetAllEmployerFeedbackQueryResult { Feedbacks = feedbacks };
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Feedbacks, Is.EqualTo(feedbacks));
        }
    }
}
