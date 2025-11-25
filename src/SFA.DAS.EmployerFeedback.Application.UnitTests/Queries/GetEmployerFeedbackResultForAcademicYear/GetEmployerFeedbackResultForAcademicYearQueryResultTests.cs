using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackResultForAcademicYear;
using SFA.DAS.EmployerFeedback.Domain.Models;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetEmployerFeedbackResultForAcademicYear
{
    public class GetEmployerFeedbackResultForAcademicYearQueryResultTests
    {
        [Test]
        public void Properties_SetAndGet_ShouldReturnExpectedValues()
        {
            var model = new EmployerFeedbackForAcademicYearResult();
            var result = new GetEmployerFeedbackResultForAcademicYearQueryResult { EmployerFeedbackForAcademicYear = model };
            Assert.That(result.EmployerFeedbackForAcademicYear, Is.EqualTo(model));
        }
    }
}
