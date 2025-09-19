using SFA.DAS.EmployerFeedback.Application.Queries.GetRefreshALELastRunDateSetting;
using NUnit.Framework;

namespace SFA.DAS.EmployerFeedback.Application.UnitTests.Queries.GetRefreshALELastRunDateSetting
{
    public class GetRefreshALELastRunDateSettingQueryTests
    {
        [Test]
        public void CanInstantiate_GetRefreshALELastRunDateSettingQuery()
        {
            var query = new GetRefreshALELastRunDateSettingQuery();
            Assert.That(query, Is.Not.Null);
        }
    }
}
