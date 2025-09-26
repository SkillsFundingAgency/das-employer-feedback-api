using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetEmployerFeedbackResultForAcademicYear;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetEmployerFeedbackResultForAcademicYear
{
    public class GetEmployerFeedbackResultForAcademicYearQueryTests
    {
        [Test]
        public void CanInstantiate_GetEmployerFeedbackResultForAcademicYearQuery()
        {
            var query = new GetEmployerFeedbackResultForAcademicYearQuery();
            Assert.That(query, Is.Not.Null);
        }
    }
}
