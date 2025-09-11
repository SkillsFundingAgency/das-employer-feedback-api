using SFA.DAS.EmployerFeedback.Application.Queries.GetSettings;
using NUnit.Framework;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetSettings
{
    public class GetSettingsQueryTests
    {
        [Test]
        public void CanInstantiate_GetSettingsQuery()
        {
            var query = new GetSettingsQuery();
            Assert.That(query, Is.Not.Null);
        }
    }
}
