using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetYearlyEmployerFeedbackResult;
using System.Collections.Generic;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetYearlyEmployerFeedbackResult
{
    public class GetYearlyEmployerFeedbackResultQueryResultTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var details = new List<Domain.Models.GetYearlyEmployerFeedbackResult>();
            var result = new GetYearlyEmployerFeedbackResultQueryResult { AnnualEmployerFeedbackDetails = details };
            Assert.That(result.AnnualEmployerFeedbackDetails, Is.EqualTo(details));
        }
    }
}
