using NUnit.Framework;
using SFA.DAS.EmployerFeedback.Application.Queries.GetAllEmployerFeedback;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetAllEmployerFeedback
{
    public class GetAllEmployerFeedbackQueryTests
    {
        [Test]
        public void CanInstantiate_GetAllEmployerFeedbackQuery()
        {
            var query = new GetAllEmployerFeedbackQuery();
            Assert.That(query, Is.Not.Null);
        }
    }
}
